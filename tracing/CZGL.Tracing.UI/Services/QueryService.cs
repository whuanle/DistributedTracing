using CZGL.Tracing.Models;
using CZGL.Tracing.UI.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CZGL.Tracing.UI.Services
{
    public class QueryService
    {
        private static Option Options = new Option();
        private class Option : TracingOption { }


        // 空过滤
        private static readonly FilterDefinition<TracingObject> EmptyFilter = Builders<TracingObject>.Filter.Empty;

        private readonly IMongoDatabase database;
        private readonly ILogger<QueryService> logger;
        public QueryService(MongoClient mongoClient, ILoggerFactory loggerFactory)
        {
            database = mongoClient.GetDatabase(Options.DataName);
            logger = loggerFactory.CreateLogger<QueryService>();
        }

        /// <summary>
        /// 查询链路追踪数据中所有类型的服务
        /// <para>Process.ServiceName</para>
        /// </summary>
        /// <returns></returns>
        public async Task<QueryResponseServices<string>> GetServices()
        {
            var collection = database.GetCollection<TracingObject>(Options.DocumentName);
            var result = await collection.Distinct(a => a.Process.ServiceName, EmptyFilter).ToListAsync();
            return new QueryResponseServices<string>
            {
                Data = result.ToArray()
            };
        }

        /// <summary>
        /// 查询其中一个服务
        /// </summary>
        /// <param name="serviceId"></param>
        /// <returns></returns>
        public async Task<QueryResponseServices<QueryTracingObject>> GetService(string serviceId)
        {
            var collection = database.GetCollection<BsonDocument>(Options.DocumentName);

            // 构建筛选规则
            FilterDefinitionBuilder<BsonDocument> filterBuilder = Builders<BsonDocument>.Filter;
            // 从 MongoDB 按照条件查询结果
            FilterDefinition<BsonDocument> filter = filterBuilder.Eq("Spans.TraceId",serviceId);
            var queryResult = await collection
                .Find(filter)
                .ToListAsync();


            // 反序列化对象
            var objects = queryResult.Select(x => BsonSerializer.Deserialize<TracingObject>(x)).ToArray();

            // 获得查询结果
            var response = new QueryResponseServices<QueryTracingObject>
            {
                Data = objects.ToQuery()
            };

            return response;
        }

        public async Task<QueryResponseServices<SpanReference>> Dependencies(long endTs, long lookback)
        {
            var collection = database.GetCollection<BsonDocument>(Options.DocumentName);


            // 查询字段
            ProjectionDefinition<BsonDocument> projection = Builders<BsonDocument>.Projection.Include("Spans.References");

            var filterBuilder = Builders<BsonDocument>.Filter;
            var f_start = filterBuilder.Gt("Spans.StartTime", endTs- lookback);
            var f_end = filterBuilder.Lt("Spans.StartTime", endTs);
            var filter = filterBuilder.And(f_start, f_end);
            var result = await collection
                .Find(filter)
                .Project(projection).ToListAsync();

            var objects = result.Select(x => BsonSerializer.Deserialize<SpanReference>(x)).ToArray();

            return new QueryResponseServices<SpanReference>
            {
                Data = objects
            };
        }

        /// <summary>
        /// 查询一个服务中有哪些操作
        /// <para>Spans.OperationName</para>
        /// </summary>
        /// <param name="serviceName">服务名称</param>
        /// <returns></returns>
        public async Task<QueryResponseServices<string>> ServiceOperation(string serviceName)
        {
            var operation = serviceName;
            var collection = database.GetCollection<TracingObject>(Options.DocumentName);

            // 查询条件 
            var filter = Builders<TracingObject>.Filter.Eq("Process.ServiceName", operation);

            // 查询字段
            ProjectionDefinition<TracingObject> projection = Builders<TracingObject>.Projection.Include("Spans.OperationName");


            var result = await collection.Find(filter)
                .Limit(1)
                .Project(projection)
                .FirstAsync();

            if (result.Count() == 0)
                return new QueryResponseServices<string>();

            return new QueryResponseServices<string>
            {
                Data = result.GetElement("Spans")
                .Value.AsBsonArray
                .Select(x => x.AsBsonDocument.GetElement("OperationName").Value.AsString)
                .ToArray()
            };
        }


        /// <summary>
        /// 聚合搜索，查找符合条件的
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<QueryResponseServices<QueryTracingObject>> SearchTraces(SearchTrace model)
        {
            if (string.IsNullOrWhiteSpace(model.ServiceName))
                return new QueryResponseServices<QueryTracingObject>();

            var collection = database.GetCollection<BsonDocument>(Options.DocumentName);

            // 筛选条件列表
            List<FilterDefinition<BsonDocument>> filterList = new List<FilterDefinition<BsonDocument>>();
            // 构建筛选规则
            FilterDefinitionBuilder<BsonDocument> filterBuilder = Builders<BsonDocument>.Filter;

            var f_service = filterBuilder.Eq("Process.ServiceName", model.ServiceName);
            filterList.Add(f_service);


            if (!string.IsNullOrEmpty(model.Operation))
            {
                FilterDefinition<BsonDocument> f_opearation = filterBuilder.Eq("Spans.OperationName", model.Operation);
                filterList.Add(f_opearation);
            }

            //排序约束   Ascending 正序    Descending 倒序
            SortDefinitionBuilder<BsonDocument> builderSort = Builders<BsonDocument>.Sort;
            SortDefinition<BsonDocument> sort = builderSort.Descending("Spans.StartTime");

            if (model.Tags != null && model.Tags.Any())
            {
                foreach (var item in model.Tags)
                {
                    var tmp = filterBuilder.And(filterBuilder.Eq("Spans.Tags.Key", item.Key), filterBuilder.Eq("Spans.Tags.Value", item.Value));
                    filterList.Add(tmp);
                }
            }

            if (model.MinDuration.HasValue)
            {
                var f_duration = filterBuilder.Gt("Spans.Duration", model.MinDuration.Value);
                filterList.Add(f_duration);
            }


            if (model.MaxDuration.HasValue)
            {
                var f_duration=filterBuilder.Lt("Spans.Duration", model.MaxDuration.Value);
                filterList.Add(f_duration);
            }

            var f_start = filterBuilder.Gt("Spans.StartTime", model.Start);
            var f_end = filterBuilder.Lt("Spans.StartTime", model.End);

            filterList.Add(filterBuilder.And(f_service, f_start, f_end));

            // 从 MongoDB 按照条件查询结果
            FilterDefinition<BsonDocument> filter = filterBuilder.And(filterList);
            var queryResult = await collection
                .Find(filter)
                .Sort(sort)
                .Limit(model.Limit)
                .ToListAsync();

            // 反序列化对象
            var objects = queryResult.Select(x => BsonSerializer.Deserialize<TracingObject>(x)).ToArray();

            // 获得查询结果
            var response = new QueryResponseServices<QueryTracingObject>
            {
                Data = objects.ToQuery()
            };

            return response;
        }
    }
}
