using Permissions.BL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Permissions.BL.Services
{
    public interface IGetPermissionsService : IGenericReadService<Permission>
    {
        Task<IEnumerable<Permission>> GetPermissions();
        Task<Permission> GetPermissionById(int permissionId);
    }
}
