using CZGL.Tracing.Models;
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
            services.AddTransient<QueryService>();
            services.AddControllers().AddApplicationPart(typeof(TracingUIExtensions).Assembly);
        }


        public static IEnumerable<QueryTracingObject> ToQuery(this IEnumerable<TracingObject> objects)
        {
            List<QueryTracingObject> queries = new List<QueryTracingObject>();
            foreach (var item in objects)
            {
                QueryTracingObject queryTracingObject = new QueryTracingObject()
                {
                    Spans = item.Spans,
                    Processes = new Dictionary<string, TracingProcess>()
                    {
                        { "p1",item.Process}
                    }
                };
                queries.Add(queryTracingObject);
            }

            return queries;
        }


        private static readonly Dictionary<string, string> Route302 = new Dictionary<string, string>
        {
            { "/search","/index.html"},
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
               if (Route302.TryGetValue(path, out var value302))
               {
                   context.Response.Redirect(value302);
               }
           });
        }
    }
}
