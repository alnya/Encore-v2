namespace Encore.DataStore
{
    using MongoDB.Bson.Serialization;
    
    public class MapBsonClass<T>
    {
        public static void IgnoreNull()
        {
            BsonClassMap.RegisterClassMap<T>(cm =>
            {
                cm.AutoMap();
                foreach (var bsonMemberMap in cm.DeclaredMemberMaps)
                {
                    bsonMemberMap.SetIgnoreIfNull(true);
                }
            });
        }
    }
}
