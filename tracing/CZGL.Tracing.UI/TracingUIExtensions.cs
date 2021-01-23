using CZGL.Tracing.Models;
using CZGL.Tracing.Services;
using CZGL.Tracing.UI.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CZGL.Tracing.UI
{
    public static class TracingUIExtensions
    {

        /// <summary>
        /// 注入 CZGL.Tracing 服务
        /// </summary>
        /// <param name="services"></param>
        public static void AddTracingUI(this IServiceCollection services)
        {
            services.AddGrpc();
            services.AddTransient<TracingQueryService>();
            services.AddControllers().AddApplicationPart(typeof(TracingUIExtensions).Assembly);
        }
    }
}
