using System.Text.Json.Serialization;

namespace CZGL.Tracing.Models
{
    public class TracingProcess
    {
        [JsonIgnore]
        public string ProcessId { get; set; }
        public string ServiceName { get; set; }
        public SpanTag[] Tags { get; set; }
    }
}