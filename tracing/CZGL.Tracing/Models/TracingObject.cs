using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CZGL.Tracing.Models
{
    public class TracingObject
    {
        public TracingSpan[] Spans { get; set; }

        public TracingProcess Process { get; set; }
    }
}
