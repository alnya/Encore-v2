namespace Encore.DataStore
{
    using Encore.Domain.Entities;
    using Encore.Domain.Interfaces.DataStore;
    using MongoDB.Driver;
    using MongoDB.Driver.Builders;
    using MongoMigrations;
    using System;

    public class MongoContext : IRepositoryContext
    {
        private readonly MongoClient client;
        private readonly MongoServer server;
        private readonly MongoDatabase database;

        public MongoContext(string connectionString, string dbName)
        {
            client = new MongoClient(connectionString);
            server = client.GetServer();
            database = server.GetDatabase(dbName);

            MapBsonClass<Project>.IgnoreNull();
            var collection = database.GetCollection<ReportResultRow>("ReportResultRow");
            collection.CreateIndex(IndexKeys<ReportResultRow>.Ascending(x => x.ReportResultId));

            var assembly = System.Reflection.Assembly.GetAssembly(typeof(MongoContext));
            var runner = new MigrationRunner("mongodb://localhost", dbName);
            runner.MigrationLocator.LookForMigrationsInAssembly(assembly);

            if(runner.DatabaseStatus.IsNotLatestVersion())
            {
                runner.UpdateToLatest();
            }
        }

        public void TryConnect()
        {
            server.Connect();
        }

        public IRepository<T> GetRepository<T>() where T : EntityBase
        {
            return new MongoRepository<T>(database);
        }

        public void DatabaseExists()
        {
            throw new NotImplementedException();
        }
    }
}
