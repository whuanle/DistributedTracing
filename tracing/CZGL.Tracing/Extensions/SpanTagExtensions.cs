using CZGL.Tracing.Models;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Jaeger.ApiV2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CZGL.Tracing.Extensions
{
    /// <summary>
    /// Tag 转换
    /// </summary>
    public static class SpanTagExtensions
    {
        public static IEnumerable<SpanTag> BuildTags(this IEnumerable<KeyValue> spans)
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
                    case Jaeger.ApiV2.ValueType.Binary: tag.Value = (item.VBinary.ToString(Encoding.Unicode)); break;
                }
                tags.Add(tag);
            }
            return tags;
        }

        public static RepeatedField<Log> BuildLogs(this IEnumerable<SpanLog> logs)
        {
            RepeatedField<Log> list = new RepeatedField<Log>();

            foreach (var item in logs)
            {
                Log log = new Log()
                {
                    Timestamp = TracingUtil.GetTraceTimestamp(item.Timestamp)
                };
                var fields = log.Fields;
                foreach (var field in item.Fields)
                {
                    KeyValue value = new KeyValue()
                    {
                        Key = field.Key
                    };
                    Jaeger.ApiV2.ValueType type = (Jaeger.ApiV2.ValueType)System.Enum.Parse(typeof(Jaeger.ApiV2.ValueType), field.Type);
                    switch (type)
                    {
                        case Jaeger.ApiV2.ValueType.String: value.VStr = field.Value.ToString(); break;
                        case Jaeger.ApiV2.ValueType.Bool: value.VBool = (bool)field.Value; break;
                        case Jaeger.ApiV2.ValueType.Int64: value.VInt64 = (long)field.Value; break;
                        case Jaeger.ApiV2.ValueType.Float64: value.VFloat64 = (double)field.Value; break;
                        case Jaeger.ApiV2.ValueType.Binary: value.VBinary = ByteString.CopyFrom(Encoding.Unicode.GetBytes(field.Value.ToString())); break;
                    }
                    fields.Add(value);
                }
                list.Add(log);
            }
            return list;
        }

        public static RepeatedField<KeyValue> BuildTags(this IEnumerable<SpanTag> spans)
        {
            RepeatedField<KeyValue> list = new RepeatedField<KeyValue>();

            foreach (var field in spans)
            {
                KeyValue value = new KeyValue()
                {
                    Key = field.Key
                };
                Jaeger.ApiV2.ValueType type = (Jaeger.ApiV2.ValueType)System.Enum.Parse(typeof(Jaeger.ApiV2.ValueType), field.Type);
                switch (type)
                {
                    case Jaeger.ApiV2.ValueType.String: value.VStr = field.Value.ToString(); break;
                    case Jaeger.ApiV2.ValueType.Bool: value.VBool = (bool)field.Value; break;
                    case Jaeger.ApiV2.ValueType.Int64: value.VInt64 = (long)field.Value; break;
                    case Jaeger.ApiV2.ValueType.Float64: value.VFloat64 = (double)field.Value; break;
                    case Jaeger.ApiV2.ValueType.Binary: value.VBinary = ByteString.CopyFrom(Encoding.Unicode.GetBytes(field.Value.ToString())); break;
                }
                list.Add(value);
            }
            return list;
        }

        public static List<SpanLog> BuildLogs(this RepeatedField<Log> logs)
        {
            List<SpanLog> spans = new List<SpanLog>();
            foreach (var item in logs)
            {
                SpanLog span = new SpanLog()
                {
                    Timestamp = TracingUtil.GetLongTime(item.Timestamp),
                    Fields = item.Fields.BuildTags().ToArray()
                };
                spans.Add(span);
            }
            return spans;
        }

    }

}
