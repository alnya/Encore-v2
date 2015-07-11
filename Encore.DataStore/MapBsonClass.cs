using MongoDB.Bson.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encore.DataStore
{
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
