using CZGL.Tracing.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CZGL.Tracing.UI.Controllers;
using CZGL.Tracing.UI;

namespace TracingAspNetCoreExapmle
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddGrpc();
            services.AddTransient(typeof(MongoClient), se =>
             {
                 return new MongoClient("mongodb://admin:123456@106.12.123.126:27017/admin");
             });

            services.AddControllers();
            //services.AddSwaggerGen(c =>
            //{
            //    c.SwaggerDoc("v1", new OpenApiInfo { Title = "CZGL.Tracing", Version = "v1" });
            //});

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CZGL.Tracing v1"));
            }

            ////app.AddTracingUI();
            app.UseDefaultFiles();
            app.UseStaticFiles();


            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<TraceCollector>();
                endpoints.MapGrpcService<TracingQuery>();
                endpoints.MapControllers();
            });
        }
    }
}
