﻿using System.Linq.Expressions;

namespace LibrarySystem.Data.Repository
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);

        Task DeleteWhere(Expression<Func<T, bool>> predicate);

        IEnumerable<T> GetWhere(Expression<Func<T, bool>> predicate);
    }
}