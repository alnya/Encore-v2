namespace Encore.DataStore.Migrations
{
    using MongoDB.Bson;
    using MongoDB.Driver;
    using MongoMigrations;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class MakeAllUsersAdmin : CollectionMigration
    {
        public MakeAllUsersAdmin() : base("1.0.1", "SystemUser")
        {
            Description = "Make all current users Admin and add empty project passwords collection";
        }

        public override void UpdateDocument(MongoCollection<BsonDocument> collection, BsonDocument document)
        {
            BsonElement userRoleElement;

            if (!document.TryGetElement("UserRole", out userRoleElement))
            {
                document.Add("UserRole", 0);
            }

            document.Add("ProjectPasswords", new BsonArray());

            collection.Save(document);
        }
    }
}
