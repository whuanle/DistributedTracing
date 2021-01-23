using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CZGL.Tracing.Models
{
    public class QueryTracingObject
    {
        public string TraceID
        {
            get
            {
                if (Spans == null)
                    return null;

                return Spans.FirstOrDefault()?.TraceId;
            }
        }

        public TracingSpan[] Spans { get; set; }

        public Dictionary<string, TracingProcess> Processes { get; set; }
    }
}
