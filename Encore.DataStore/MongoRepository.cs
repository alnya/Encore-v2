namespace Encore.DataStore
{
    using MongoDB.Driver;
    using MongoDB.Driver.Builders;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;
    using MongoDB.Driver.Linq;
    using Encore.Domain.Interfaces.DataStore;
    using Encore.Domain.Entities;
    using Encore.Domain.Interfaces.Services;
    using MongoDB.Bson;
    
    public class MongoRepository<T> : IRepository<T> where T : EntityBase
    {
        private readonly string collectionName = typeof(T).Name;
        private readonly MongoDatabase database;

        public MongoRepository(MongoDatabase database)
        {
            this.database = database;
        }

        public IEnumerable<T> GetWhere<TValue>(Expression<Func<T, TValue>> func, TValue value, IRequestedPage requestedPage = null)
        {
            var cursor = GetCollection().FindAs<T>(Query<T>.EQ(func, value));

            List<T> entities;

            if (requestedPage != null)
            {
                int skip = requestedPage.Page > 1 ? (requestedPage.Page - 1) * requestedPage.PageSize : 0;

                entities = cursor.SetSkip(skip).SetLimit(requestedPage.PageSize).ToList<T>();
            }
            else
            {
                entities = cursor.ToList<T>();
            }

            return entities;
        }

        public IEnumerable<T> GetWhere(Func<T, bool> predicate, IRequestedPage requestedPage = null)
        {
            var queryable = predicate == null ? GetQueryable() : GetQueryable().Where(predicate);

            if (requestedPage != null)
            {
                int skip = requestedPage.Page > 1 ? (requestedPage.Page - 1) * requestedPage.PageSize : 0;
                queryable = queryable.Skip(skip).Take(requestedPage.PageSize);
            }

            return queryable.ToList();
        }

        public T Get(Guid id)
        {
            return GetQueryable().Where(x => x.Id == id).FirstOrDefault();
        }

        public int Count(ISearchTerms searchTerms)
        {
            return GetQueryable().Where(searchTerms).Count();
        }

        public int Count<TValue>(Expression<Func<T, TValue>> func, TValue value)
        {
            var cursor = GetCollection().FindAs<T>(Query<T>.EQ(func, value));
            return cursor.Count<T>();
        }

        public bool Exists(Func<T, bool> predicate)
        {
            return GetQueryable().Any(predicate);
        }

        public IEnumerable<T> Search(Func<T, bool> predicate, ISortCriteria sortCriteria, ISearchTerms searchTerms, IRequestedPage requestedPage)
        {
            var queryable = GetQueryable();

            if (predicate != null)
            {
                queryable = queryable.Where(predicate).AsQueryable();
            }

            queryable = queryable.Where(searchTerms).OrderBy(sortCriteria);

            if (requestedPage != null)
            {
                int skip = requestedPage.Page > 1 ? (requestedPage.Page - 1) * requestedPage.PageSize : 0;
                queryable = queryable.Skip(skip).Take(requestedPage.PageSize);
            }

            return queryable.ToList();
        }

        public void Insert(IEnumerable<T> entities)
        {
            if (entities.Any())
            {
                GetCollection().InsertBatch(entities);
            }
        }

        public void Save(T entity)
        {
            GetCollection().Save(entity);
        }

        public void Merge(Guid id, T entity)
        {
            MergeDocument(id, entity.ToBsonDocument());
        }

        public void DeleteWhere(Expression<Func<T, bool>> expression)
        {
            GetCollection().Remove(Query<T>.Where(expression));
        }

        public void DeleteAll()
        {
            GetCollection().RemoveAll();
        }

        private MongoCollection<T> GetCollection()
        {
            return database.GetCollection<T>(collectionName);
        }

        private IQueryable<T> GetQueryable()
        {
            return GetCollection().AsQueryable();
        }

        private void MergeDocument(Guid id, BsonDocument bsonDocument)
        {
            bsonDocument.Remove("_id");
            var updateDocument = new UpdateDocument { { "$set", bsonDocument } };
            GetCollection().Update(Query.EQ("_id", BsonValue.Create(id)), updateDocument);
        }
    }
}
