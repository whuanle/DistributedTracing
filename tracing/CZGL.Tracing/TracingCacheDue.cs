using CZGL.Tracing.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CZGL.Tracing
{
    /// <summary>
    /// 缓存过期时处理
    /// </summary>
    public class TracingCacheDue : ITracingCacheDue
    {
        private readonly IMongoDatabase database;
        public TracingCacheDue(MongoClient mongoClient)
        {
            database = mongoClient.GetDatabase(TracingOption.Options.DataName);
        }
        public async Task InvokeAsync(TracingObject tracingObject)
        {
            var collection = database.GetCollection<BsonDocument>(TracingOption.Options.DocumentName);
            await collection.InsertOneAsync(tracingObject.ToBsonDocument());
        }
    }
}
