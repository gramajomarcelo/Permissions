using Permissions.BL.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Permissions.BL.Repositories.UnitOfWork.Implements
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly PermissionsContext _context;


        public UnitOfWork(PermissionsContext context)
        {
            _context = context;
        }

        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Rollback()
        {
            // Implementar rollback en caso de error
        }
    }
}
