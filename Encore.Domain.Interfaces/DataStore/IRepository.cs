namespace Encore.Domain.Interfaces.DataStore
{
    using Encore.Domain.Entities;
    using Encore.Domain.Interfaces.Services;
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    
    public interface IRepository<T> where T : EntityBase
    {
        T Get(Guid id);

        IEnumerable<T> GetWhere<TValue>(Expression<Func<T, TValue>> func, TValue value, IRequestedPage requestedPage = null);

        IEnumerable<T> GetWhere(Func<T, bool> predicate = null, IRequestedPage requestedPage = null);

        IEnumerable<T> Search(ISearchTerms searchTerms, Func<T, bool> predicate, ISortCriteria sortCriteria, IRequestedPage requestedPage);

        void Save(T entity);

        void Merge(Guid id, T entity);

        void Insert(IEnumerable<T> entities);

        void DeleteWhere(Expression<Func<T, bool>> expression);

        void DeleteAll();

        int Count(ISearchTerms searchTerms, Func<T, bool> predicate = null);

        int Count<TValue>(Expression<Func<T, TValue>> func, TValue value);

        bool Exists(Func<T, bool> predicate);
    }
}
