using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CZGL.Tracing.Models
{
    public class QueryTracingObject
    {
        public TracingSpan[] Spans { get; set; }

        public Dictionary<string, TracingProcess> Process { get; set; }
    }
}
