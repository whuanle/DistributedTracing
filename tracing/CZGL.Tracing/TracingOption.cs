using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CZGL.Tracing
{
    /// <summary>
    /// Tracing 后端存储数据的配置
    /// </summary>
    public class TracingOption
    {
        /// <summary>
        /// MongoDB 数据表名称
        /// </summary>
        public string DataName
        {
            get
            {
                return _DataName;
            }
            set
            {
                _DataName = value;
            }
        }

        /// <summary>
        /// MongoDB 文档名称
        /// </summary>
        public string DocumentName
        {
            get
            {
                return _DocumentName;
            }
            set
            {
                _DocumentName = value;
            }
        }

        protected static string _DataName = "tracing";
        protected static string _DocumentName = "tracing";
        internal static Option Options = new Option();
        internal class Option : TracingOption { }
    }
}
