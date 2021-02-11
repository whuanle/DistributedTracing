using CZGL.Tracing.Caches;
using CZGL.Tracing.Extensions;
using CZGL.Tracing.Models;
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
    /// <summary>
    /// Jaeger Collector
    /// </summary>
    public class TraceCollector : CollectorService.CollectorServiceBase
    {
        private readonly ILogger<TraceCollector> logger;

        private readonly ConcurrentCache _traceingCache;
        public TraceCollector(ConcurrentCache traceingCache, ILoggerFactory loggerFactory)
        {
            _traceingCache = traceingCache;
            logger = loggerFactory.CreateLogger<TraceCollector>();
        }

        /// <summary>
        /// 接收 Jaeger agent 通过 gRPC 推送来的数据
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<PostSpansResponse> PostSpans(PostSpansRequest request, ServerCallContext context)
        {
            long traceId = TracingUtil.GetTraceLongId(request.Batch.Spans.First().TraceId);

            // 第一次添加，此时 entry.Data 为 null
            if (_traceingCache.TryRegister(traceId, out var entry))
            {
                var traceObject = request.BuildTracingObject(1);
                entry.Data = traceObject;
            }
            // 已经存在相同的 trace id，需要在原来的基础上加上新的 span 和 process
            else
            {
                lock (entry.Data.ObjLock)
                {
                    entry.Data.Index += 1;

                    var traceObject = request.BuildTracingObject(entry.Data.Index);

                    entry.Data.Spans.AddRange(traceObject.Spans);
                    entry.Data.Process.AddRange(traceObject.Process);
                }
            }

            logger.LogInformation($"get one trace message,trace id:{traceId}");

            await Task.CompletedTask;
            PostSpansResponse postSpans = new PostSpansResponse();
            return postSpans;
        }
    }
}
