using Grpc.Core;
using Jaeger.ApiV2;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CZGL.Tracing.Services
{
    public class TraceCollector : CollectorService.CollectorServiceBase
    {
        private readonly IMongoDatabase database;
        private readonly ILogger<TraceCollector> logger;
        public TraceCollector(MongoClient mongoClient, ILoggerFactory loggerFactory)
        {
            database = mongoClient.GetDatabase(TracingOption.Options.DataName);
            logger = loggerFactory.CreateLogger<TraceCollector>();
        }

        public override async Task<PostSpansResponse> PostSpans(PostSpansRequest request, ServerCallContext context)
        {
            var collection = database.GetCollection<BsonDocument>(TracingOption.Options.DocumentName);

            var traceObject = request.BuildTracingObject();
            logger.LogInformation($"get one trace message,trace id:{traceObject.Spans.FirstOrDefault().TraceId}");
            logger.LogInformation($"spans id:");
            logger.LogInformation(string.Join("\r\n", traceObject.Spans.Select(x => x.SpanId)));

            await collection.InsertOneAsync(traceObject.ToBsonDocument());

            PostSpansResponse postSpans = new PostSpansResponse();
            return postSpans;
        }
    }
}
