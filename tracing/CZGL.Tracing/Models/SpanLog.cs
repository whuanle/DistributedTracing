using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CZGL.Tracing.Models
{
    public class SpanLog
    {
        public DateTime DateTime { get; set; }
        public SpanTag[] Fields { get; set; }
    }
}
