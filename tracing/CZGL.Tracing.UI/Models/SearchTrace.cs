using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CZGL.Tracing.UI.Models
{
    public class SearchTrace
    {
        /// <summary>
        /// Service
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// Operation，留空则查询全部
        /// </summary>
        public string Operation { get; set; }

        /// <summary>
        /// 标签，如 http.status_code=200 error=true，可留空
        /// </summary>
        public Dictionary<string, object> Tags { get; set; }

        /// <summary>
        /// 时间范围，起始时间
        /// </summary>
        public long Start { get; set; }

        /// <summary>
        /// 时间范围，结束时间
        /// </summary>
        public long End { get; set; }

        /// <summary>
        /// 最小延迟时间
        /// </summary>
        public int? MinDuration { get; set; }

        /// <summary>
        /// 最大延迟时间
        /// </summary>
        public int? MaxDuration { get; set; }

        /// <summary>
        /// 查询记录条数，默认 10 条
        /// </summary>
        public int Limit { get; set; } = 10;
    }
}
