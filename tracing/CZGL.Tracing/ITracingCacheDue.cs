using CZGL.Tracing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CZGL.Tracing
{
    /// <summary>
    /// 缓存过期时处理
    /// </summary>
    public interface ITracingCacheDue
    {
        Task InvokeAsync(TracingObject tracingObject);
    }
}
