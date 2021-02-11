using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CZGL.Tracing.Caches
{
    /// <summary>
    /// 缓存键
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EntryCache<T>
    {
        public EntryCache(DateTime dateTime)
        {
            DateTime = dateTime;
        }
        /// <summary>
        /// 数据被缓存时的时间
        /// </summary>
        public DateTime DateTime { get; private set; }

        /// <summary>
        /// 数据
        /// </summary>
        public T Data { get; set; }
    }
}
