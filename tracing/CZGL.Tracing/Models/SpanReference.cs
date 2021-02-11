using Jaeger.ApiV2;

namespace CZGL.Tracing.Models
{
    public class SpanReference
    {
        public string TraceID { get; set; }
        public string SpanID { get; set; }

        /// <summary>
        /// <para><see cref="Jaeger.ApiV2.SpanRefType"/></para>
        /// <para>CHILD_OF/Follows_From</para>
        /// </summary>
        public string RefType { get; set; }
    }
}
