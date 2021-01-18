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

        }

        /// <summary>
        /// API 路由表
        /// </summary>
        private static readonly Dictionary<string, string> Route = new Dictionary<string, string>()
        {
            {"/api/services","/api/Tracing/services"}
        };

        private static readonly Dictionary<string, string> Route302 = new Dictionary<string, string>
        {
            { "/search","/index.html?search"},
        };



        /// <summary>
        /// 添加 CZGL.Tracing 中间件
        /// </summary>
        /// <param name="app"></param>
        public static void AddTracingUI(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
           {
               var path = context.Request.Path.ToUriComponent().ToLowerInvariant();
               //if (Route.TryGetValue(path, out var value))
               //{
               //    context.Request.Path = value;
               //    await next();
               //}
               //else
               if (Route302.TryGetValue(path,out var value302))
               {
                   context.Response.Redirect(value302);
               }
           });
        }
    }
}
