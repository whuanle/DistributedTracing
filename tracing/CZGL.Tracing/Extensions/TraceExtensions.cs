using CZGL.Tracing.Models;
using Google.Protobuf;
using Jaeger.ApiV2;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace CZGL.Tracing.Extensions
{
    /// <summary>
    /// Trace 拓展
    /// </summary>
    public static class TraceExtensions
    {

        /// <summary>
        /// 将 gRPC 传递的 Tracing 消息转成对象
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static TracingObject BuildTracingObject(this PostSpansRequest request, int index)
        {
            TracingObject tracingObject = new TracingObject();
            Batch batch = request.Batch;

            tracingObject.TraceId = TracingUtil.GetTraceLongId(batch.Spans.FirstOrDefault().TraceId);

            tracingObject.Spans = batch.Spans.BuildTracingSpan(index);

            tracingObject.Process = new List<TracingProcess> { batch.Process.BuildProcess(index) };

            return tracingObject;
        }

        /// <summary>
        /// 更新索引
        /// </summary>
        /// <param name="tracingObject"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static TracingObject UpdateIndex(this TracingObject tracingObject, int index)
        {
            foreach (var item in tracingObject.Spans)
            {
                item.ProcessId = "p" + index;
            }
            foreach (var item in tracingObject.Process)
            {
                item.ProcessId = "p" + index;
            }
            return tracingObject;
        }

        /// <summary>
        /// TracingObject 转 QueryTracingObject
        /// </summary>
        /// <param name="tracingObject"></param>
        /// <returns></returns>
        public static QueryTracingObject BuildQuery(this TracingObject tracingObject)
        {
            QueryTracingObject queryTracingObject = new QueryTracingObject
            {
                Spans = tracingObject.Spans.ToArray(),
                Processes = tracingObject.Process.ToDictionary(x => x.ProcessId, x => x)
            };
            return queryTracingObject;
        }


        /// <summary>
        /// TracingObject 转 QueryTracingObject
        /// </summary>
        /// <param name="objects"></param>
        /// <returns></returns>
        public static IEnumerable<QueryTracingObject> ToQuery(this IEnumerable<TracingObject> objects)
        {
            return objects.Select(x => x.BuildQuery());
        }
    }
}
