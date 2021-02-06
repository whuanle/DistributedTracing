using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using OpenTracing;
using OpenTracing.Propagation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1
{
    public class Jaegeriddleware
    {
        private readonly RequestDelegate _next;
        public Jaegeriddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ITracer tracer)
        {
            var callingHeaders = new TextMapExtractAdapter(context.Request.Headers.ToDictionary(m => m.Key, m => m.Value.ToString()));
            var spanContex = tracer.Extract(BuiltinFormats.HttpHeaders, callingHeaders);
            ISpanBuilder builder = null;
            if (spanContex == null)
            {
                builder = tracer.BuildSpan("webService");
            }
            else
            {
                builder = tracer.BuildSpan("webService").AsChildOf(spanContex);
            }
            await _next(context);
        }
    }
    public static class JaegerExtensions
    {
        public static IApplicationBuilder UseJaegerProcess(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<Jaegeriddleware>();
        }
    }
}
