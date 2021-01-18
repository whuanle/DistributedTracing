using Jaeger.ApiV2;

namespace CZGL.Tracing.Models
{
    public class SpanReference
    {
        public string TraceId { get; set; }
        public string SpanId { get; set; }

        /// <summary>
        /// <para><see cref="Jaeger.ApiV2.SpanRefType"/></para>
        /// </summary>
        public SpanRefType RefType { get; set; }
    }
}
