using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CZGL.Tracing.Models
{
    /// <summary>
    /// Query 查询统一返回格式
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class QueryResponseServices<T>
    {
        public IEnumerable<T> Data { get; set; } = Array.Empty<T>();
        public int Total => Data.Count();
        public int Limit { get; set; }
        public int Offset { get; set; }
        public string Errors { get; set; } = null;
    }
}
