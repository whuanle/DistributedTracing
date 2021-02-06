using Jaeger;
using Jaeger.Samplers;
using Jaeger.Senders;
using Jaeger.Senders.Grpc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTracing.Util;
using System;
using System.Threading.Tasks;
using static Jaeger.Configuration;

namespace WebApplication1
{
    public class Startup
    {
        private static readonly ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        private static readonly Lazy<Tracer> Tracer = new Lazy<Tracer>(() =>
        {
            return InitTracer("WebService", loggerFactory);
        });
        private static Tracer InitTracer(string serviceName, ILoggerFactory loggerFactory)
        {
            SenderConfiguration.DefaultSenderResolver = new SenderResolver(loggerFactory)
                .RegisterSenderFactory<Jaeger.Senders.Grpc.GrpcSenderFactory>();

            var samplerConfiguration = new Configuration.SamplerConfiguration(loggerFactory)
                .WithType(ConstSampler.Type)
                .WithParam(1);

            var reporterConfiguration = new Configuration.ReporterConfiguration(loggerFactory)
                .WithSender(new SenderConfiguration(loggerFactory).WithSender(new GrpcSender("127.0.0.1:14250", null, 0)))
                .WithLogSpans(true);

            return (Tracer)new Configuration(serviceName, loggerFactory)
                .WithSampler(samplerConfiguration)
                .WithReporter(reporterConfiguration)
                .GetTracer();
        }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            GlobalTracer.Register(Tracer.Value);
            services.AddOpenTracing(build =>
            {
                build.AddHttpHandler();
                build.AddGenericDiagnostics();
                build.AddAspNetCore();
                build.AddLoggerProvider();
            });
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            //app.UseJaegerProcess();

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
