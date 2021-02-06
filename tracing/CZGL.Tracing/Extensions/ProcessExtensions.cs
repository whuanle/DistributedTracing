using CZGL.Tracing.Models;
using Jaeger.ApiV2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CZGL.Tracing.Extensions
{

    /// <summary>
    /// 解析 <see cref="Jaeger.ApiV2.Process"/> 进程信息
    /// </summary>
    public static class ProcessExtensions
    {
        public static TracingProcess BuildProcess(this Jaeger.ApiV2.Process process,int index)
        {
            if (process is null)
                return null;

            var tracingProcess = new TracingProcess();
            tracingProcess.ProcessId = "p" + index;
            tracingProcess.ServiceName = process.ServiceName;
            tracingProcess.Tags = process.Tags.BuildTags().ToArray();
            return tracingProcess;
        }

    }
}
