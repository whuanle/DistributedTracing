using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CZGL.Tracing.Models
{
    public class TracingSpan
    {
        [JsonPropertyName("traceID")]
        public string TraceId { get; set; }

        [JsonPropertyName("spanID")]
        public string SpanId { get; set; }
        public string OperationName { get; set; }
        public SpanReference[] References { get; set; }
        public uint Flags { get; set; }

        public long StartTime { get; set; }
        public int Duration { get; set; }

        public SpanTag[] Tags { get; set; }

        public SpanLog[] Logs { get; set; }
        [JsonPropertyName("processID")]
        public string ProcessId { get; set; }
        public string[] Warnings { get; set; }
    }
}
