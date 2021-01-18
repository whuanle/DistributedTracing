using AbpBase.Domain;
using System;
using Volo.Abp;
using Volo.Abp.Modularity;

namespace AbpBase.Application.Contracts
{
    [DependsOn(
       typeof(AbpBaseDomainModule)
   )]
    public class AbpBaseApplicationContractsModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {

        }
    }
}
