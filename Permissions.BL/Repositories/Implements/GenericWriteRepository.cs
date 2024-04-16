using Microsoft.EntityFrameworkCore;
using Permissions.BL.Data;

namespace Permissions.BL.Repositories.Implements
{
    public class GenericWriteRepository<TEntity> : IGenericWriteRepository<TEntity> where TEntity : class
    {
        private readonly PermissionsContext permissionsDataBaseContext;

        public GenericWriteRepository(PermissionsContext permissionsContext) 
        { 
            this.permissionsDataBaseContext = permissionsContext;
        }

        public async Task DeleteById(int id)
        {
            var entity = await permissionsDataBaseContext.Set<TEntity>().FindAsync(id);

            if (entity == null)
                throw new Exception($"Entity with id {id} not found.");

            permissionsDataBaseContext.Set<TEntity>().Remove(entity);
            await permissionsDataBaseContext.SaveChangesAsync();
        }


        public async Task<TEntity> Insert(TEntity entity)
        {
            permissionsDataBaseContext.Set<TEntity>().Add(entity);
            await permissionsDataBaseContext.SaveChangesAsync();
            return entity;
        }

        public async Task<TEntity> Update(TEntity entity)
        {
            permissionsDataBaseContext.Entry(entity).State = EntityState.Modified;
            await permissionsDataBaseContext.SaveChangesAsync();
            return entity;
        }
    }
}
