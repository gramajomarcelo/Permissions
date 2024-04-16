using Permissions.BL.DTOs;
using Permissions.BL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Permissions.BL.Services
{
    public interface IModifyPermissionService : IGenericWriteService<Permission>
    {
        Task<Permission> ModifyPermission(int permissionId, Permission modifiedPermission);
    }
}
