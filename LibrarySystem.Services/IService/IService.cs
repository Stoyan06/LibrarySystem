using System.Linq.Expressions;

namespace LibrarySystem.Services.IService
{
    public interface IService<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);

        Task DeleteWhere(Expression<Func<T, bool>> predicate);
    }
}