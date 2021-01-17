using System;
using Volo.Abp;
using Volo.Abp.Modularity;

namespace AbpBase.Domain.Shared
{
    [DependsOn()]
    public class AbpBaseDomainSharedModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
        }
    }
}
