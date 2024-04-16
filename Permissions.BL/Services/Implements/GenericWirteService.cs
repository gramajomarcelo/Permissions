using Permissions.BL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Permissions.BL.Services.Implements
{
    public class GenericWriteService<TEntity> : IGenericWriteService<TEntity> where TEntity : class
    {
        private readonly IGenericWriteRepository<TEntity> _genericRepository;

        public GenericWriteService(IGenericWriteRepository<TEntity> genericRepository)
        {
            this._genericRepository = genericRepository;
        }

        public async Task DeleteById(int id)
        {
            await _genericRepository.DeleteById(id);
        }

        public async Task<TEntity> Insert(TEntity entity)
        {
            return await _genericRepository.Insert(entity);
        }

        public async Task<TEntity> Update(TEntity entity)
        {
            return await _genericRepository.Update(entity);
        }
    }
}
