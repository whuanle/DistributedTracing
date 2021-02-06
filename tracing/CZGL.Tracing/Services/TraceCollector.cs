using CZGL.Tracing.Extensions;
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

        private readonly TraceingCache _traceingCache;
        public TraceCollector(TraceingCache traceingCache, ILoggerFactory loggerFactory)
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
#warning gRPC 推送的 traceid 有问题
            long traceId = TracingUtil.GetTraceLongId(request.Batch.Spans.First().TraceId);

            // 第一次添加
            if (!_traceingCache.TryGetValue(traceId, out var oldTracingObject))
            {
                var traceObject = request.BuildTracingObject(1);

                // 如果为 true，说明是第一个 trace，此时任务逻辑完成，可直接打印日志然后结束函数
                // 如果为 false，说明此时有相同的 trace id 并发请求，被抢先一步
                if (!_traceingCache.TryAddOrGet(traceObject, out var oldObj))
                {
                    lock (oldObj.ObjLock)
                    {
                        oldObj.Index += 1;

                        var newObj = traceObject.UpdateIndex(oldTracingObject.Index);

                        oldObj.Spans.AddRange(newObj.Spans);
                        oldObj.Process.AddRange(newObj.Process);
                    }
                }
            }
            // 已经存在相同的 trace id，需要在原来的基础上加上新的 span 和 process
            else
            {
                lock (oldTracingObject.ObjLock)
                {
                    oldTracingObject.Index += 1;

                    var traceObject = request.BuildTracingObject(oldTracingObject.Index);

                    oldTracingObject.Spans.AddRange(traceObject.Spans);
                    oldTracingObject.Process.AddRange(traceObject.Process);
                }
            }

            logger.LogInformation($"get one trace message,trace id:{traceId}");

            await Task.CompletedTask;
            PostSpansResponse postSpans = new PostSpansResponse();
            return postSpans;
        }
    }
}
