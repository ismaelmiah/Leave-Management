using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Leave_Management.Contracts
{
    public interface IRepositoryBase<T> where T : class
    {
        Task<T> Get(int id);
        IEnumerable<T> GetAll(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = null, string includeProperty = null, string includeProperte = null);
        Task<T> GetAllWithTwoEntity(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = null, string includeProperty = null);
        Task<T> GetAllWithThreeEntity(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = null, string includeProperty = null, string includeProperte = null);

        Task<T> GetFirstOrDefault(Expression<Func<T, bool>> filter = null,
            string includeProperties = null);
        void Create(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
