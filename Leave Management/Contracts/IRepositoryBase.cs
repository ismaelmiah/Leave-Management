using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Leave_Management.Contracts
{
    public interface IRepositoryBase<T> where T : class
    {
        T Get(int id);
        IEnumerable<T> GetAll(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = null);
        T GetAllWithTwoEntity(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = null, string includeProperty = null);

        T GetFirstOrDefault(Expression<Func<T, bool>> filter = null,
            string includeProperties = null);
        void Create(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
