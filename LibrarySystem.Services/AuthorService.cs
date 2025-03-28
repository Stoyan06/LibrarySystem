using LibrarySystem.Data.Repository;
using LibrarySystem.Models;
using LibrarySystem.Services.IService;
using System.Linq.Expressions;

namespace LibrarySystem.Services
{
    public class AuthorService : IAuthorService
    {
        private IRepository<Author> _repository;

        public AuthorService(IRepository<Author> repository)
        {
            _repository = repository;
        }
        public async Task AddAsync(Author entity)
        {
            await _repository.AddAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }

        public async Task DeleteWhere(Expression<Func<Author, bool>> predicate)
        {
            await _repository.DeleteWhere(predicate);
        }

        public async Task<bool> ExistsAuthor(string name)
        {
            IEnumerable<Author> authors = await _repository.GetAllAsync();
            Author aut = authors.Where(x => x.FullName == name).FirstOrDefault();

            if (aut != null) return true;
            else return false;
        }

        public async Task<IEnumerable<Author>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Author> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public IEnumerable<Author> GetWhere(Expression<Func<Author, bool>> predicate)
        {
            return _repository.GetWhere(predicate);
        }

        public async Task UpdateAsync(Author entity)
        {
            await _repository.UpdateAsync(entity);
        }
    }
}