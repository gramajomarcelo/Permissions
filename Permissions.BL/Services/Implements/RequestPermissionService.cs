using Permissions.BL.Data;
using Permissions.BL.Models;
using Permissions.BL.Repositories;
using Permissions.BL.Repositories.UnitOfWork;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Permissions.BL.Services.Implements
{
    public class RequestPermissionService : GenericWriteService<Permission>, IRequestPermissionService
    {
        private readonly PermissionsContext _context;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public RequestPermissionService(IPermissionRepository permissionRepository, IUnitOfWork unitOfWork, PermissionsContext context, ILogger logger) : base(permissionRepository)
        {
            _context = context;
            _unitOfWork = unitOfWork;
            _permissionRepository = permissionRepository;
            _logger = logger;
        }

        public async Task<Permission> RequestPermission(Permission permission)
        {
            try
            {

                var newPermission = await _permissionRepository.Insert(permission);
                
                _logger.Information("Nuevo permiso registrado en Elasticsearch: {@Permission}", newPermission);

                await _unitOfWork.CommitAsync();

                return newPermission;
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                _logger.Error(ex, "Error al solicitar un nuevo permiso.");
                throw; 
            }
        }
    }

}
