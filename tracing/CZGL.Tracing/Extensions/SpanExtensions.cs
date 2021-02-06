using CZGL.Tracing.Models;
using Google.Protobuf.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CZGL.Tracing.Extensions
{

    /// <summary>
    /// Span 拓展
    /// </summary>
    public static class SpanExtensions
    {
        /// <summary>
        /// 批量将 <see cref="Jaeger.ApiV2.Span"/> 转换为 <see cref="TracingSpan"/> 对象
        /// </summary>
        /// <param name="spans"></param>
        /// <returns></returns>
        public static List<TracingSpan> BuildTracingSpan(this RepeatedField<Jaeger.ApiV2.Span> spans,int index)
        {
            List<TracingSpan> tracingSpans = new List<TracingSpan>();
            foreach (var item in spans)
            {
                TracingSpan tracingSpan = new TracingSpan()
                {
                    TraceId = TracingUtil.GetTraceId(item.TraceId),
                    SpanId = TracingUtil.GetSpanId(item.SpanId),
                    OperationName = item.OperationName,
                    References = item.References.Select(x => x.ToSpanReference()).ToArray(),
                    Flags = item.Flags,
                    StartTime = TracingUtil.GetLongTime(item.StartTime),
#warning Duration 需要修正
                    Duration = item.Duration.Nanos
                };

                tracingSpan.Tags = item.Tags.BuildTags().ToArray();
                tracingSpan.Logs = item.Logs.BuildLogs().ToArray();
                tracingSpan.ProcessId = "p" + index;
                tracingSpan.Warnings = item.Warnings.ToArray();

                tracingSpans.Add(tracingSpan);
            };

            return tracingSpans;
        }
    }
}
