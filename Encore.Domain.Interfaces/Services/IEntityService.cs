namespace Encore.Domain.Interfaces.Services
{
    using Encore.Domain.Entities;
    using System;

    public interface IEntityService<T> where T : EntityBase
    {
        T Get(Guid id);

        IPagedListResult<T> Search(
           IRequestedPage requestedPage,
           Func<T, bool> predicate = null,
           ISortCriteria sortCriteria = null,
           ISearchTerms searchTerms = null);

        T Add(T entity);

        T Update(Guid id, T entity);

        bool Delete(Guid id);
    }
}
