namespace Encore.Domain.Entities
{
    using MongoDB.Bson.Serialization.Attributes;
    using System;
    
    public abstract class EntityBase
    {
        [BsonId]
        public Guid Id { get; set; }
    }
}
