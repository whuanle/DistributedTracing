using CZGL.Tracing;
using Google.Protobuf;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TracingAspNetCoreExapmle
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //Jaeger.TraceId traceId = Jaeger.TraceId.FromString("658b079af1589b5b");
            //var tmp = traceId.ToByteArray();
            //ByteString tmptmp = Google.Protobuf.ByteString.CopyFrom(tmp);

            //var str = TracingUntil.ByteStringToString(tmptmp);

            TracingBuilder tracingBuilder = new TracingBuilder();
            tracingBuilder.WithDataName("test")
                .WithDocument("test");
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseUrls("http://*:14250");

                    webBuilder.UseStartup<Startup>();
                });
    }
}
