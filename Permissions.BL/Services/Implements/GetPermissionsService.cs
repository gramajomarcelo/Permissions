using Permissions.BL.Data;
using Permissions.BL.Models;
using Permissions.BL.Repositories;
using Permissions.BL.Repositories.UnitOfWork;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Permissions.BL.Services.Implements
{
    public class GetPermissionsService : GenericReadService<Permission>, IGetPermissionsService
    {
        private readonly PermissionsContext _context;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public GetPermissionsService(IPermissionRepository permissionRepository, IUnitOfWork unitOfWork, PermissionsContext context, ILogger logger) : base(permissionRepository)
        {
            _context = context;
            _unitOfWork = unitOfWork;
            _permissionRepository = permissionRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Permission>> GetPermissions()
        {
            try
            {
                // Recupera todos los permisos desde el repositorio
                var permissions = await _permissionRepository.GetAll();

                _logger.Information("Permisos obtenidos de la base de datos: {@Permissions}", permissions);

                return permissions;
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                _logger.Error(ex, "Error al obtener los permisos desde la base de datos.");
                throw; 
            }
        }

        public async Task<Permission> GetPermissionById(int permissionId)
        {
            try
            {
                // Recupera un permiso por su ID desde el repositorio
                var permission = await _permissionRepository.GetById(permissionId);

                if (permission == null)
                {
                    _logger.Information("El permiso con ID {PermissionId} no fue encontrado.", permissionId);
                    throw new InvalidOperationException($"El permiso con ID {permissionId} no fue encontrado.");
                }

                _logger.Information("Permiso obtenido de la base de datos: {@Permission}", permission);

                return permission;
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                _logger.Error(ex, "Error al obtener el permiso con ID {ID}.", permissionId);
                throw;
            }
        }

    }

}
