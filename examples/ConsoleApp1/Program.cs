using Jaeger;
using Jaeger.Samplers;
using Jaeger.Senders;
using Jaeger.Senders.Grpc;
using Microsoft.Extensions.Logging;
using OpenTracing;
using OpenTracing.Propagation;
using OpenTracing.Tag;
using System;
using System.Buffers.Binary;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using static Jaeger.Configuration;

namespace ConsoleApp1
{
    public class Hello
    {
        private readonly ITracer _tracer;
        private readonly ILogger<Hello> _logger;
        public Hello(ITracer tracer, ILoggerFactory loggerFactory)
        {
            _tracer = tracer;
            _logger = loggerFactory.CreateLogger<Hello>();
        }

        public void SayHello(string helloTo)
        {
            using var scope = _tracer.BuildSpan("say-hello").StartActive(true);
            scope.Span.SetTag("hello-to", helloTo);
            var helloString = FormatString(helloTo);
            PrintHello(helloString);
        }

        private string FormatString(string helloTo)
        {
            using (var scope = _tracer.BuildSpan("format-string").StartActive(true))
            {
                var url = $"http://127.0.0.1:18081/api/format/{helloTo}";

                // 为 Header 头生成信息
                var span = scope.Span
                    .SetTag(Tags.SpanKind, Tags.SpanKindClient)
                    .SetTag(Tags.HttpMethod, "GET")
                    .SetTag(Tags.HttpUrl, url);
                var dictionary = new Dictionary<string, string>();
                _tracer.Inject(span.Context, BuiltinFormats.HttpHeaders, new TextMapInjectAdapter(dictionary));

                // 注入 Header 头
                using WebClient webClient = new WebClient();
                foreach (var entry in dictionary)
                    webClient.Headers.Add(entry.Key, entry.Value);

                // 请求远程 Web 服务
                var helloString = webClient.DownloadString(url);

                // 获得 Web 响应内容
                span.Log(new Dictionary<string, object>
                {
                    [LogFields.Event] = "string.Format",
                    ["value"] = helloString
                });

                return helloString;
            }
        }

        private void PrintHello(string helloString)
        {
            using var scope = _tracer.BuildSpan("print-hello").StartActive(true);
            _logger.LogInformation(helloString);
            scope.Span.Log(new Dictionary<string, object>
            {
                [LogFields.Event] = "WriteLine"
            });
        }
    }

    class Program
    {
        private static Tracer InitTracer(string serviceName, ILoggerFactory loggerFactory)
        {

            //var reporter = new RemoteReporter.Builder()
            //    .WithLoggerFactory(loggerFactory)
            //    .WithSender(new GrpcSender("127.0.0.1:14250", null, 0))
            //    //.WithSender(new GrpcSender("180.102.130.181:14250", null, 0))
            //    .Build();

            //var tracer = new Tracer.Builder(serviceName)
            //    .WithLoggerFactory(loggerFactory)
            //    .WithSampler(new ConstSampler(true))
            //    .WithReporter(reporter);
            //return tracer.Build();

            Configuration.SenderConfiguration.DefaultSenderResolver = new SenderResolver(loggerFactory)
    .RegisterSenderFactory<GrpcSenderFactory>();

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


        static void Main(string[] args)
        {
            long value = Convert.ToInt64("6aa1d92bbfcc6977",16);
            byte[] bytes = BitConverter.GetBytes(1611318628516206);
            Array.Reverse(bytes);
            if (BitConverter.IsLittleEndian)
            {
                value =  BinaryPrimitives.ReadInt64LittleEndian(bytes);
            }
            else
            {
                value = BinaryPrimitives.ReadInt64BigEndian(bytes);
            }

            Console.WriteLine();
            using ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            while (true)
            {
                var tracer = InitTracer("hello-world", loggerFactory);
                Hello hello = new Hello(tracer, loggerFactory);
                hello.SayHello("This trace");
                Console.Read();
            }
        }
    }
}
