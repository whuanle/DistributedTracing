using CZGL.Tracing.Services;
using Microsoft.Extensions.DependencyInjection;

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
