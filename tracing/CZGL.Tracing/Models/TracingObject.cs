using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CZGL.Tracing.Models
{
    public class TracingObject
    {
        [JsonIgnore]
        public ObjectId _id { get; set; }
        public TracingSpan[] Spans { get; set; }

        public TracingProcess Process { get; set; }
    }
}
