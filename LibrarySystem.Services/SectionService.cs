using LibrarySystem.Data.Repository;
using LibrarySystem.Models;
using LibrarySystem.Services.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LibrarySystem.Services
{
    public class SectionService : ISectionService
    {
        private IRepository<Section> _repository;

        public SectionService(IRepository<Section> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Section>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Section> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task AddAsync(Section entity)
        {
            await _repository.AddAsync(entity);
        }

        public async Task UpdateAsync(Section entity)
        {
            await _repository.UpdateAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }

        public async Task DeleteWhere(Expression<Func<Section, bool>> predicate)
        {
            await _repository.DeleteWhere(predicate);
        }

        public async Task<bool> ExisisSection(string name)
        {
            IEnumerable<Section> sections = await _repository.GetAllAsync();
            Section sec = sections.Where(x => x.Name == name).FirstOrDefault();
            if (sec != null) return true;
            else return false;
        }
    }
}
