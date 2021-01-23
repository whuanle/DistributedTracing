using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CZGL.Tracing.Models
{
    public class SpanLog
    {
        public long Timestamp { get; set; }
        public SpanTag[] Fields { get; set; }
    }
}
