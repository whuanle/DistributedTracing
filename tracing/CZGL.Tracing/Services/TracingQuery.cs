using CZGL.Tracing.Extensions;
using CZGL.Tracing.Models;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Grpc.Core;
using Jaeger;
using Jaeger.ApiV2;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CZGL.Tracing.Services
{
    public class TracingQuery : QueryService.QueryServiceBase
    {
        private readonly IMongoDatabase database;
        private readonly ILogger<TracingQuery> logger;
        private readonly TracingQueryService _queryService;
        public TracingQuery(TracingQueryService queryService, MongoClient mongoClient, ILoggerFactory loggerFactory)
        {
            _queryService = queryService;
            database = mongoClient.GetDatabase(TracingOption.Options.DataName);
            logger = loggerFactory.CreateLogger<TracingQuery>();
        }
        public override Task<ArchiveTraceResponse> ArchiveTrace(ArchiveTraceRequest request, ServerCallContext context)
        {
            return base.ArchiveTrace(request, context);
        }
        public override Task FindTraces(FindTracesRequest request, IServerStreamWriter<SpansResponseChunk> responseStream, ServerCallContext context)
        {
            return base.FindTraces(request, responseStream, context);
        }

        public override async Task<GetDependenciesResponse> GetDependencies(GetDependenciesRequest request, ServerCallContext context)
        {
            QueryResponseServices<SpanReference> result = await _queryService
                .Dependencies(TracingUtil.GetLongTime(request.StartTime), TracingUtil.GetLongTime(request.EndTime));

            var response = new GetDependenciesResponse();

            RepeatedField<DependencyLink> links = response.Dependencies;
            foreach (var item in result.Data)
            {
                links.Add(new DependencyLink
                {
                    Parent = null,
                    Child = null
                });
            }

            return response;
        }

        public override async Task<GetOperationsResponse> GetOperations(GetOperationsRequest request, ServerCallContext context)
        {
            QueryResponseServices<string> result = await _queryService.ServiceOperation(request.Service);

            GetOperationsResponse response = new GetOperationsResponse();

            var operationName = response.OperationNames;
            operationName.Add(result.Data);

            var operations = response.Operations;

            return response;
        }


        public override async Task<GetServicesResponse> GetServices(GetServicesRequest request, ServerCallContext context)
        {
            var result = await _queryService.GetServices();

            GetServicesResponse response = new GetServicesResponse();
            var fields = response.Services;
            fields.Add(result.Data);
            return response;
        }

        public override async Task GetTrace(GetTraceRequest request, IServerStreamWriter<SpansResponseChunk> responseStream, ServerCallContext context)
        {
            var result = await _queryService.GetService(TracingUtil.GetTraceId(request.TraceId));
            SpansResponseChunk response = new SpansResponseChunk();
            RepeatedField<Jaeger.ApiV2.Span> spans = response.Spans;

            foreach (var item in result.Data.First().Spans)
            {
                var tmp = new Jaeger.ApiV2.Span
                {
                    TraceId = ByteString.CopyFrom(TraceId.FromString(item.TraceId).ToByteArray()),
                    SpanId = ByteString.CopyFrom(SpanId.FromString(item.SpanId).ToByteArray()),
                    OperationName = item.OperationName,
                    // References
                    Flags = item.Flags,
                    StartTime = TracingUtil.GetTraceTimestamp(item.StartTime),
                    Duration = Google.Protobuf.WellKnownTypes.Duration.FromTimeSpan(TimeSpan.FromMilliseconds(item.Duration)),
                    ProcessId = item.ProcessId,
                    // Tags

                };

                var re = tmp.References;
                re.Add(item.References.ToSpanTref());

                var tags = tmp.Tags;
                tags.Add(item.Tags.BuildTags());

                var logs = tmp.Logs;
                logs.Add(item.Logs.BuildLogs());

                var warns = tmp.Warnings;
                warns.Add(item.Warnings);

                spans.Add(tmp);
            }

            await responseStream.WriteAsync(response);
        }
    }
}
