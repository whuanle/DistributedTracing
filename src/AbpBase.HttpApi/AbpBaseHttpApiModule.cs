using AbpBase.Application;
using AbpBase.Application.Contracts;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Modularity;

namespace AbpBase.HttpApi
{
    [DependsOn(
        typeof(AbpAspNetCoreMvcModule),
        typeof(AbpBaseApplicationModule),
        typeof(AbpBaseApplicationContractsModule)
        )]
    public class AbpBaseHttpApiModule : AbpModule
    {

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpAspNetCoreMvcOptions>(options =>
            {
                options
                    .ConventionalControllers
                    .Create(typeof(AbpBaseHttpApiModule).Assembly);
            });
        }
    }
}
