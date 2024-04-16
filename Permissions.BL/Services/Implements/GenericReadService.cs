using Permissions.BL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Permissions.BL.Services.Implements
{
    public class GenericReadService<TEntity> : IGenericReadService<TEntity> where TEntity : class
    {
        private IGenericReadRepository<TEntity> _genericRepository;

        public GenericReadService(IGenericReadRepository<TEntity> genericRepository)
        {
            this._genericRepository = genericRepository;
        }

        public async Task<IEnumerable<TEntity>> GetAll()
        {
            return await _genericRepository.GetAll();
        }

        public async Task<TEntity> GetById(int id)
        {
            return await _genericRepository.GetById(id);
        }
    }
}
