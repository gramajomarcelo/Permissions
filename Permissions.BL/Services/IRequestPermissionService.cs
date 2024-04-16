using Permissions.BL.DTOs;
using Permissions.BL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Permissions.BL.Services
{
    public interface IRequestPermissionService : IGenericWriteService<Permission>
    {
        Task<Permission> RequestPermission(Permission permission);
    }
}
