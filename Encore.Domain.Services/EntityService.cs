namespace Encore.Domain.Services
{
    using Encore.Domain.Entities;
    using Encore.Domain.Interfaces.DataStore;
    using Encore.Domain.Interfaces.Services;
    using Encore.Domain.Services.Search;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class EntityService<T> : IEntityService<T> where T : EntityBase
    {
        private readonly IRepositoryContext context;

        public EntityService(IRepositoryContext context)
        {
            this.context = context;
        }

        public virtual T Get(Guid id)
        {
            var entityRepo = context.GetRepository<T>();
            return entityRepo.Get(id);
        }

        public virtual IPagedListResult<T> Search(IRequestedPage requestedPage, Func<T, bool> predicate = null, ISortCriteria sortCriteria = null, ISearchTerms searchTerms = null)
        {
            var entityRepo = context.GetRepository<T>();

            var results = entityRepo.Search(predicate, sortCriteria, searchTerms, requestedPage);
            var count = entityRepo.Count(searchTerms);

            return new PagedListResult<T>(results, count);
        }

        public virtual T Add(T entity)
        {
            var entityRepo = context.GetRepository<T>();
            entityRepo.Save(entity);

            return entity;
        }

        public virtual T Update(Guid id, T entity)
        {
            entity.Id = id;

            var entityRepo = context.GetRepository<T>();
            entityRepo.Merge(id, entity);

            return entity;
        }

        public virtual bool Delete(Guid id)
        {
            var entityRepo = context.GetRepository<T>();
            entityRepo.DeleteWhere(x => x.Id == id);

            return true;
        }
    }
}
