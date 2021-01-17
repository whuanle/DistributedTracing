namespace CZGL.Tracing.Models
{
    public class TracingProcess
    {
        public string ServiceName { get; set; }
        public SpanTag[] Tags { get; set; }
    }
}