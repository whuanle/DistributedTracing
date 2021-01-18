using CZGL.Tracing.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;

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

        public class TracingResponseServices<T>
        {
            public T[] Data { get; set; }
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
        public async Task<TracingResponseServices<string>> Services()
        {
            var collection = database.GetCollection<TracingObject>(TracingBuilder.Option.DocumentName);
            var result = await collection.Distinct(a => a.Process.ServiceName, EmptyFilter).ToListAsync();
            return new TracingResponseServices<string>
            {
                Data = result.ToArray()
            };
        }

        /// <summary>
        /// 查询一个 service 中的 Operation 
        /// </summary>
        /// <returns></returns>
        [HttpGet("services/{service}/operations")]
        public async Task<TracingResponseServices<string>> ServiceOperation(string service)
        {
            var operation = service;

            var collection = database.GetCollection<TracingObject>(TracingBuilder.Option.DocumentName);

            // 查询条件 
            var filter = Builders<TracingObject>.Filter.Eq("Process.ServiceName", operation);

            ProjectionDefinition<TracingObject> projection = Builders<TracingObject>.Projection.Include("Spans.OperationName");

            var result = await collection.Find(filter).Limit(1).Project(projection).FirstAsync();
            return new TracingResponseServices<string>
            {
                Data = result.GetElement("Spans")
                .Value.AsBsonArray
                .Select(x => x.AsBsonDocument.GetElement("OperationName").Value.AsString)
                .ToArray()
            };
        }

        /// <summary>
        /// 聚合搜索
        /// </summary>
        /// <returns></returns>
        [HttpGet("traces")]
        public async Task<TracingResponseServices<QueryTracingObject>> Traces(
            [Required] string service,
            string operation,
            string tags,
            int? minDuration,
            int? maxDuration,
            [Required] long start,
            [Required] long end,
            string lookback,
            int limit = 10
            )
        {
            if (string.IsNullOrWhiteSpace(service))
                return new TracingResponseServices<QueryTracingObject>();

            var collection = database.GetCollection<TracingObject>(TracingBuilder.Option.DocumentName);

            var filterBuilder = Builders<TracingObject>.Filter;
            filterBuilder.Eq("Process.ServiceName", service);
            if (!string.IsNullOrEmpty(operation))
                filterBuilder.Eq("Spans.OperationName", operation);

            if (!string.IsNullOrWhiteSpace(tags))
            {
                Dictionary<string, object> dic = JsonSerializer.Deserialize<Dictionary<string, object>>(tags);
                foreach (var item in dic)
                {
                    filterBuilder.Eq("Spans.Tags.Key", item.Key);
                    filterBuilder.Eq("Spans.Tags.Value", item.Value);
                }
            }

            if (minDuration.HasValue)
            {
                filterBuilder.Gt("Spans.Duration", minDuration.Value);
            }


            if (maxDuration.HasValue)
            {
                filterBuilder.Lt("Spans.Duration", maxDuration.Value);
            }

            filterBuilder.Gt("Spans.StartTime", start);
            filterBuilder.Lt("Spans.StartTime", end);

            var filter = filterBuilder.Empty;
            var result = await collection.Find(filter).Limit(limit).ToListAsync();
            


            return null;
        }
    }
}
