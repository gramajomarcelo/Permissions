using Permissions.BL.Data;
using Permissions.BL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Permissions.BL.Repositories.Implements
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly IGenericReadRepository<Permission> _readRepository;
        private readonly IGenericWriteRepository<Permission> _writeRepository;

        public PermissionRepository( IGenericReadRepository<Permission> readRepository, IGenericWriteRepository<Permission> writeRepository)
        {
            _readRepository = readRepository;
            _writeRepository = writeRepository;
        }

        public async Task<IEnumerable<Permission>> GetAll()
        {
            return await _readRepository.GetAll();
        }

        public async Task<Permission> GetById(int id)
        {
            return await _readRepository.GetById(id);
        }

        public async Task<Permission> Insert(Permission entity)
        {
            return await _writeRepository.Insert(entity);
        }

        public async Task<Permission> Update(Permission entity)
        {
            return await _writeRepository.Update(entity);
        }

        public async Task DeleteById(int id)
        {
            await _writeRepository.DeleteById(id);
        }
    }
}
