namespace CZGL.Tracing.Models
{
    public class SpanTag
    {
        public string Key { get; set; }

        /// <summary>
        /// <see cref="Jaeger.ApiV2.ValueType"/>
        /// </summary>
        public string Type { get; set; }
        public object Value { get; set; }
    }
}
