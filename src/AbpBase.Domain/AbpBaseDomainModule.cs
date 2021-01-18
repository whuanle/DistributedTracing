using AbpBase.Domain.Shared;
using System;
using Volo.Abp;
using Volo.Abp.Modularity;

namespace AbpBase.Domain
{
    [DependsOn(
        typeof(AbpBaseDomainSharedModule)
        )]
    public class AbpBaseDomainModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
        }
    }
}
