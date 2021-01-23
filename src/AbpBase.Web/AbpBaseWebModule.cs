using AbpBase.Application;
using AbpBase.HttpApi;
using AbpBase.Web.Filters;
using CZGL.Tracing;
using CZGL.Tracing.UI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Reflection;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Modularity;
using Volo.Abp.Swashbuckle;

namespace AbpBase.Web
{
    [DependsOn(
        typeof(AbpBaseApplicationModule),
        typeof(AbpBaseHttpApiModule),
        typeof(AbpAspNetCoreMvcModule),
        typeof(AbpSwashbuckleModule)
        )]
    public class AbpBaseWebModule : AbpModule
    {

        private const string ABPCosr = "AllowSpecificOrigins";


        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddTracingUI();
            Configure<AbpAspNetCoreMvcOptions>(options =>
            {
                options
                    .ConventionalControllers
                    .Create(typeof(CZGL.Tracing.UI.Controllers.TracingController).Assembly);
            });
            Configure<MvcOptions>(options =>
            {
                // 全局异常拦截器
                options.Filters.Add(typeof(WebGlobalExceptionFilter));
            });

            ConfigureSwaggerServices(context.Services);

            // 跨域请求
            ConfigureCors(context);

            // 全局 API 请求实体验证失败信息格式化
            context.Services.GlabalInvalidModelStateFilter();

            // 配置依赖注入服务
            ConfigureAutoIoc(context);
        }

        private void ConfigureSwaggerServices(IServiceCollection services)
        {
            services.AddSwaggerGen(
                options =>
                {
                    options.SwaggerDoc("v1", new OpenApiInfo { Title = "CZGL.Tracing API", Version = "v1" });
                    options.DocInclusionPredicate((docName, description) => true);
                    options.CustomSchemaIds(type => type.FullName);
                    // Use method name as operationId
                    options.CustomOperationIds(apiDesc =>
                    {
                        return apiDesc.TryGetMethodInfo(out MethodInfo methodInfo) ? methodInfo.Name : null;
                    });
                }
            );
        }

        /// <summary>
        /// 配置跨域
        /// </summary>
        /// <param name="context"></param>
        private void ConfigureCors(ServiceConfigurationContext context)
        {
            context.Services.AddCors(options =>
            {
                options.AddPolicy(ABPCosr,
                    builder => builder.AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowAnyOrigin());
            });
        }

        /// <summary>
        /// 自动扫描所有的服务并进行依赖注入
        /// </summary>
        /// <param name="context"></param>
        private void ConfigureAutoIoc(ServiceConfigurationContext context)
        {
            context.Services.AddAssemblyOf<AbpBaseApplicationModule>();
            context.Services.AddAssemblyOf<AbpBaseWebModule>();
            context.Services.AddAssemblyOf<CZGL.Tracing.UI.Controllers.TracingController>();
        }


        public override void OnApplicationInitialization(
            ApplicationInitializationContext context)
        {
            var app = context.GetApplicationBuilder();
            var env = context.GetEnvironment();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
            app.UseRouting();

            app.UseCors(ABPCosr);

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "CZGL.Tracing API");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapTracing();
            });

            app.UseConfiguredEndpoints(endpoints => { endpoints.MapControllers().RequireCors(ABPCosr); });
        }
    }
}
