using AutoMapper;
using Permissions.BL.Data;
using Permissions.BL.DTOs;
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
    public class ModifyPermissionService : GenericWriteService<Permission>, IModifyPermissionService
    {
        private readonly PermissionsContext _context;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public ModifyPermissionService(IPermissionRepository permissionRepository, IUnitOfWork unitOfWork, PermissionsContext context, ILogger logger) : base(permissionRepository)
        {
            _context = context;
            _unitOfWork = unitOfWork;
            _permissionRepository = permissionRepository;
            _logger = logger;
        }

        public async Task<Permission> ModifyPermission(int permissionId, Permission modifiedPermission)
        {
            try
            {
                // Verificar si el permiso existe
                var existingPermission = await _permissionRepository.GetById(permissionId);
                if (existingPermission == null)
                {
                    _logger.Information("El permiso con ID {PermissionId} no existe.", permissionId);
                    throw new InvalidOperationException("El permiso especificado no existe.");
                }

                // Realizar las modificaciones necesarias en el permiso existente
                existingPermission.EmployeeForename = modifiedPermission.EmployeeForename;
                existingPermission.EmployeeSurname = modifiedPermission.EmployeeSurname;
                existingPermission.PermissionType = modifiedPermission.PermissionType;
                existingPermission.PermissionDate = modifiedPermission.PermissionDate;

                // Actualizar el permiso en la base de datos
                var updatedPermission = await _permissionRepository.Update(existingPermission);

                _logger.Information("Permiso modificado en Elasticsearch: {@Permission}", updatedPermission);

                // Commit de la transacción utilizando la Unidad de Trabajo
                await _unitOfWork.CommitAsync();

                return updatedPermission;
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                _logger.Error(ex, "Error al modificar el permiso con ID {PermissionId}.", permissionId);
                throw ;
            }
        }

    }

}
