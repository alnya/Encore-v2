namespace Encore.Domain.Entities
{
    using MongoDB.Bson.Serialization.Attributes;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class ProjectToken
    {
        [BsonId]
        public Guid ProjectId { get; set; }

        public string UserName { get; set; }

        public string Token { get; set; }
    }
}
