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
    /// <summary>
    /// MongoDB 数据库存储结构
    /// </summary>
    public class TracingObject
    {
        /// <summary>
        /// 请忽略此 Id
        /// </summary>
        [JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public ObjectId _id { get; set; }

        /// <summary>
        /// 并发🔒
        /// </summary>
        [BsonIgnore]
        [JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public object ObjLock { get; } = new object();

        /// <summary>
        /// 当前已经有多少个进程加入
        /// </summary>
        [BsonIgnore]
        [JsonIgnore]
        public int Index { get; set; } = 1;

        /// <summary>
        /// 用于快速对照的缓存 index
        /// </summary>
        [BsonIgnore]
        [JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public long TraceId { get; set; }

        public List<TracingSpan> Spans { get; set; }

        public List<TracingProcess> Process { get; set; }
    }
}
