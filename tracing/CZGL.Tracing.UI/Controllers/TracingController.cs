using CZGL.Tracing.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CZGL.Tracing.UI.Controllers
{
    /*
     * https://github.com/jaegertracing/jaeger-idl/blob/master/swagger/zipkin2-api.yaml
     */

    /// <summary>
    /// 实现 Zipkin API
    /// </summary>
    [ApiController]
    [Route("/api")]
    public class TracingController : ControllerBase
    {
        private readonly IMongoDatabase database;
        private readonly ILogger<TracingController> logger;
        public TracingController(MongoClient mongoClient, ILoggerFactory loggerFactory)
        {
            database = mongoClient.GetDatabase(TracingBuilder.Option.DataName);
            logger = loggerFactory.CreateLogger<TracingController>();
        }

        // 空过滤
        private static readonly FilterDefinition<TracingObject> EmptyFilter = Builders<TracingObject>.Filter.Empty;
        // 只查询 Process.ServiceName
        private static readonly ProjectionDefinition<TracingObject> ServiceNameProjection = Builders<TracingObject>.Projection.Include("Process.ServiceName");

        public class TracingResponseServices
        {
            public string[] Data { get; set; }
            public int Total => Data.Count();
            public int Limit { get; set; }
            public int Offset { get; set; }
            public string Errors { get; set; } = null;
        }

        /// <summary>
        /// 查询所有的服务
        /// </summary>
        /// <returns></returns>
        [HttpGet("services")]
        public async Task<TracingResponseServices> Services()
        {
            var collection = database.GetCollection<TracingObject>(TracingBuilder.Option.DocumentName);
            var result = await collection.Distinct(a => a.Process.ServiceName, EmptyFilter).ToListAsync();
            return new TracingResponseServices
            {
                Data = result.ToArray()
            };
        }

        /// <summary>
        /// 查询一个 service 中的 Operation 
        /// </summary>
        /// <returns></returns>
        [HttpGet("services/{service}/operations")]
        public async Task<TracingResponseServices> ServiceOperation(string service)
        {
            var operation = service;

            var collection = database.GetCollection<TracingObject>(TracingBuilder.Option.DocumentName);

            // 查询条件 
            var filter = Builders<TracingObject>.Filter.Eq("Process.ServiceName", operation);

            ProjectionDefinition<TracingObject> projection = Builders<TracingObject>.Projection.Include("Spans.OperationName");

            var result = await collection.Find(filter).Limit(1).Project(projection).FirstAsync();
            return new TracingResponseServices
            {
                Data=result.GetElement("Spans")
                .Value.AsBsonArray
                .Select(x=>x.AsBsonDocument.GetElement("OperationName").Value.AsString)
                .ToArray()
            };
        }
    }
}
