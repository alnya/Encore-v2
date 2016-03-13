namespace Encore.Domain.Entities
{
    using MongoDB.Bson.Serialization.Attributes;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class ProjectPassword
    {
        [BsonId]
        public Guid ProjectId { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }
    }
}
