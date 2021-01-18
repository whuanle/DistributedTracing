using AbpBase.Application.Contracts;
using AbpBase.Database;
using AbpBase.Domain;
using Microsoft.Extensions.DependencyInjection;
using System;
using Volo.Abp;
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
        }
    }
}
