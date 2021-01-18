using CZGL.Tracing.Models;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Jaeger.ApiV2;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CZGL.Tracing
{
    public static class TracingUntil
    {
        public const string Spans = "Spans";

        public static string ByteStringToString(this ByteString byteString)
        {
            if (byteString.Length == 16)
            {
                var traceId = GetTracingId(byteString);
                return traceId.Low.ToString("x016");
            }
            var spanId = GetSpanId(byteString);
            return spanId.ToString("x016");
        }

        public static long GetSpanId(ByteString byteString)
        {
            if (byteString.Length != 8)
                return 0;

            byte[] bytes = byteString.ToArray();
            long low = BytesToLong(bytes);
            return low;
        }

        public static (long High, long Low) GetTracingId(ByteString byteString)
        {
            if (byteString.Length != 16)
                return (0, 0);

            byte[] bytes = byteString.ToArray();
            long high = BytesToLong(bytes[0..8]);
            long low = BytesToLong(bytes[8..16]);
            return (high, low);
        }


        public static long BytesToLong(byte[] bytes)
        {
            if (BitConverter.IsLittleEndian)
            {
                return BinaryPrimitives.ReadInt64BigEndian(bytes);
            }
            return BinaryPrimitives.ReadInt64LittleEndian(bytes);
        }

        /// <summary>
        /// 将 gRPC 传递的 Tracing 消息转成对象
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static TracingObject BuildTracingObject(this PostSpansRequest request)
        {
            TracingObject tracingObject = new TracingObject();

            tracingObject.Spans = request.Batch.Spans.BuildTracingSpan().ToArray();

            tracingObject.Process = request.Batch.Process.BuildProcess();

            return tracingObject;
        }

        public static TracingProcess BuildProcess(this Process process)
        {
            if (process is null)
                return null;

            TracingProcess tracingProcess = new TracingProcess();
            tracingProcess.ServiceName = process.ServiceName;
            tracingProcess.Tags = process.Tags.BuildTags().ToArray();
            return tracingProcess;
        }

        public static List<TracingSpan> BuildTracingSpan(this RepeatedField<Span> spans)
        {
            List<TracingSpan> tracingSpans = new List<TracingSpan>();
            foreach (var item in spans)
            {
                TracingSpan tracingSpan = new TracingSpan()
                {
                    TraceId = ByteStringToString(item.TraceId),
                    SpanId = ByteStringToString(item.SpanId),
                    OperationName = item.OperationName,
                    References = item.References.Select(x => x.ToSpanReference()).ToArray(),
                    Flags = item.Flags,
                    StartTime = item.StartTime.ToDateTime().GetTimestamp(),
                    Duration = (int)item.Duration.ToTimeSpan().TotalMilliseconds/10
                };

                tracingSpan.Tags = item.Tags.BuildTags().ToArray();
                tracingSpan.Logs = item.Logs.BuildLogs().ToArray();
                tracingSpan.Process = item.Process.BuildProcess();
                tracingSpan.ProcessId = item.ProcessId;
                tracingSpan.Warnings = item.Warnings.ToArray();

                tracingSpans.Add(tracingSpan);
            };

            return tracingSpans;
        }

        public static long GetTimestamp(this DateTime dateTime)
        {
            DateTime dt1970 = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return (dateTime.Ticks - dt1970.Ticks) / 10;
        }

        public static List<SpanTag> BuildTags(this RepeatedField<KeyValue> spans)
        {
            List<SpanTag> tags = new List<SpanTag>();

            foreach (var item in spans)
            {
                SpanTag tag = new SpanTag()
                {
                    Key = item.Key,
                    Type = item.VType.ToString().ToLower()
                };
                switch (item.VType)
                {
                    case Jaeger.ApiV2.ValueType.String: tag.Value = item.VStr; break;
                    case Jaeger.ApiV2.ValueType.Bool: tag.Value = item.VBool; break;
                    case Jaeger.ApiV2.ValueType.Int64: tag.Value = item.VInt64; break;
                    case Jaeger.ApiV2.ValueType.Float64: tag.Value = item.VFloat64; break;
                    case Jaeger.ApiV2.ValueType.Binary: tag.Value = ByteStringToString(item.VBinary); break;
                }
                tags.Add(tag);
            }

            return tags;
        }

        public static List<SpanLog> BuildLogs(this RepeatedField<Log> logs)
        {
            List<SpanLog> spans = new List<SpanLog>();
            foreach (var item in logs)
            {
                SpanLog span = new SpanLog()
                {
                    DateTime = item.Timestamp.ToDateTime(),
                    Fields = item.Fields.BuildTags().ToArray()
                };
                spans.Add(span);
            }
            return spans;
        }


        #region

        public static SpanReference ToSpanReference(this SpanRef spanRef)
        {
            return new SpanReference
            {
                TraceId = ByteStringToString(spanRef.TraceId),
                SpanId = ByteStringToString(spanRef.SpanId),
                RefType = spanRef.RefType
            };
        }
        #endregion
    }
}
