using Leave_Management.Contracts;
using Leave_Management.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Leave_Management.Repository
{
    public class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {

        private readonly ApplicationDbContext _db;
        internal DbSet<T> dbSet;

        public RepositoryBase(ApplicationDbContext db)
        {
            _db = db;
            this.dbSet = _db.Set<T>();
        }

        /// <summary>
        /// Generic Added Entity
        /// </summary>
        /// <param name="entity"></param>
        public async void Create(T entity)
        {
            await dbSet.AddAsync(entity);
        }
        public async Task<T> Get(int id)
        {
            return await dbSet.FindAsync(id);
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = null, string includeProperty = null, string includeProperte = null)
        {
            IQueryable<T> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (includeProperties != null)
            {
                foreach (var item in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(item);
                }
            }


            if (includeProperte != null)
            {
                foreach (var item in includeProperte.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(item);
                }
            }

            if (includeProperty != null)
            {
                foreach (var item in includeProperty.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(item);
                }
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            return query.ToList();
        }


        public async Task<T> GetFirstOrDefault(Expression<Func<T, bool>> filter = null, string includeProperties = null)
        {
            IQueryable<T> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (includeProperties != null)
            {
                foreach (var item in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(item);
                }
            }
            return await query.FirstOrDefaultAsync();
        }


        public void Delete(T entity)
        {
            dbSet.Remove(entity);
        }

        public void Update(T entity)
        {
            dbSet.Update(entity);
        }

        public async Task<T> GetAllWithTwoEntity(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = null, string includeProperty = null)
        {
            IQueryable<T> query = dbSet;
            if (includeProperties != null)
            {
                foreach (var item in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(item);
                }
            }
            if (includeProperty != null)
            {
                foreach (var item in includeProperty.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(item);
                }
            }

            return await query.FirstOrDefaultAsync(filter);
        }
        public async Task<T> GetAllWithThreeEntity(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = null, string includeProperty = null, string includeProperte = null)
        {
            IQueryable<T> query = dbSet;
            if (includeProperties != null)
            {
                foreach (var item in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(item);
                }
            }
            if (includeProperty != null)
            {
                foreach (var item in includeProperty.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(item);
                }
            }
            if (includeProperte != null)
            {
                foreach (var item in includeProperte.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(item);
                }
            }

            return await query.FirstOrDefaultAsync(filter);
        }
    }
}
