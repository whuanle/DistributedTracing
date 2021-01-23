using CZGL.Tracing.UI;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Volo.Abp.Modularity;

namespace CZGL.Tracing.ABP
{
    [DependsOn()]
    public class TracingModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            ConfigTracing(context.Services);
        }
        private void ConfigTracing(IServiceCollection services)
        {
            services.AddGrpc();
            services.AddTracing();
        }
    }
}
