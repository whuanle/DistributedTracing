using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CZGL.Tracing
{
    public class TracingBuilder
    {
        public TracingBuilder WithDataName(string dataName)
        {
            Option.DataName = dataName;
            return this;
        }
        public TracingBuilder WithDocument(string documentName)
        {
            Option.DocumentName = documentName;
            return this;
        }
        public static TracingOption Option
        { get; private set; } = new TracingOption();

        public class TracingOption
        {
            public string DataName { get; set; } = "tracing";
            public string DocumentName { get; set; } = "tracing";
        }
    }
}
