
using CZGL.Tracing.Caches;
using CZGL.Tracing.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace CZGL.Tracing
{
    public static class TracingExtensions
    {

        /// <summary>
        /// 注入 CZGL.Tracing 服务
        /// </summary>
        /// <param name="services"></param>
        public static void AddTracing(this IServiceCollection services, Action<TracingOption> option = null)
        {
            services.AddGrpc();
            if (option != null)
            {
                TracingOption tracingOption = new TracingOption();
                option.Invoke(tracingOption);
            }

            services.AddSingleton<ConcurrentCache>();
        }

        /// <summary>
        /// 添加 Tracing GRPC 中间件
        /// </summary>
        /// <param name="builder"></param>
        public static void MapTracing(this IEndpointRouteBuilder builder)
        {
            builder.MapGrpcService<TraceCollector>();
            builder.MapGrpcService<TracingQuery>();
        }
    }
}
