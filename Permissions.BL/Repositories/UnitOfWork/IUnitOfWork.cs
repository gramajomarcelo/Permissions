using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Permissions.BL.Repositories.UnitOfWork
{
    public interface IUnitOfWork
    {
        Task<int> CommitAsync();
        void Rollback();
    }
}
