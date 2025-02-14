using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LibrarySystem.Data.Repository
{
        public class Repository<T> : IRepository<T> where T : class
        {
            private readonly ApplicationDbContext _context;
            private readonly DbSet<T> _dbSet;

            public Repository(ApplicationDbContext context)
            {
                _context = context;
                _dbSet = _context.Set<T>();
            }
            public async Task<IEnumerable<T>> GetAllAsync()
            {
                return await _dbSet.ToListAsync();
            }

            public async Task<T> GetByIdAsync(int id)
            {
                return await _dbSet.FindAsync(id);
            }

            public async Task AddAsync(T entity)
            {
                await _dbSet.AddAsync(entity);
                await _context.SaveChangesAsync();
            }

            public async Task UpdateAsync(T entity)
            {
                _dbSet.Update(entity);
                await _context.SaveChangesAsync();
            }
            public async Task DeleteAsync(int id)
            {
                _dbSet.Remove(await _dbSet.FindAsync(id));
                await _context.SaveChangesAsync();
            }

            public IQueryable<T> GetAll()
            {
                return _dbSet.AsQueryable();
            }

        public async Task DeleteWhere(Expression<Func<T, bool>> predicate)
        {
            _dbSet.RemoveRange(_dbSet.Where(predicate));
            await _context.SaveChangesAsync();
        }

        public IEnumerable<T> GetWhere(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Where(predicate);
        }
    }
    }