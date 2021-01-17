namespace CZGL.Tracing.Models
{
    public class SpanTag
    {
        public long VInt64 { get; set; }
        public bool VBool { get; set; }
        public string VStr { get; set; }
        public Jaeger.ApiV2.ValueType VType { get; set; }
        public string Key { get; set; }
        public string VBinary { get; set; }
        public double VFloat64 { get; set; }
    }
}
