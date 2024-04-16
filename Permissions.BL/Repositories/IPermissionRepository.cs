using Permissions.BL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Permissions.BL.Repositories
{
    public interface IPermissionRepository : IGenericReadRepository<Permission>, IGenericWriteRepository<Permission>
    {

    }
}
