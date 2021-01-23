using AbpBase.Application.Contracts;
using AbpBase.Database;
using AbpBase.Domain;
using CZGL.Tracing;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace AbpBase.Application
{
    [DependsOn(
        typeof(AbpBaseDomainModule),
        typeof(AbpBaseApplicationContractsModule),
        typeof(AbpBaseDatabaseModule)
    )]
    public class AbpBaseApplicationModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddSingleton<Serilog.ILogger>(Serilog.Log.Logger);

            Configure<AbpAutoMapperOptions>(options =>
            {
                // 以模块为单位注册映射
                options.AddMaps<AbpBaseApplicationModule>(validate: true);
                //// 以单个 Profiel 为单位注册映射
                //options.AddProfile<AbpBaseApplicationAutoMapperProfile>(validate: true);
            });

            COnfigTracing(context.Services);
        }

        private void COnfigTracing(IServiceCollection services)
        {
            services.AddGrpc();
            services.AddTracing(option =>
            {
                option.DataName = "test";
                option.DocumentName = "test";
            });
            services.AddTransient(typeof(MongoClient), se =>
            {
                return new MongoClient("mongodb://admin:123456@106.12.123.126:27017/admin");
            });
        }
    }
}
