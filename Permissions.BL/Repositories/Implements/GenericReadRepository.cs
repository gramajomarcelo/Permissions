using Microsoft.EntityFrameworkCore;
using Permissions.BL.Data;

namespace Permissions.BL.Repositories.Implements
{
    public class GenericReadRepository<TEntity> : IGenericReadRepository<TEntity> where TEntity : class
    {
        private readonly PermissionsContext permissionsDataBaseContext;

        public GenericReadRepository(PermissionsContext permissionsContext) 
        { 
            this.permissionsDataBaseContext = permissionsContext;
        }

        public async Task<IEnumerable<TEntity>> GetAll()
        {
            return await permissionsDataBaseContext.Set<TEntity>().ToListAsync();
        }

        public async Task<TEntity> GetById(int id)
        {
            return await permissionsDataBaseContext.Set<TEntity>().FindAsync(id) ?? throw new Exception($"Entity with id {id} not found.");
        }
    }
}
