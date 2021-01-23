using CZGL.Tracing.Models;
using CZGL.Tracing.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace CZGL.Tracing.UI.Controllers
{
    /*
     * https://github.com/jaegertracing/jaeger-idl/blob/master/swagger/zipkin2-api.yaml
     */

    /// <summary>
    /// 实现 Zipkin API
    /// </summary>
    [ApiController]
    [Route("/api")]
    public class TracingController : ControllerBase
    {
        private readonly TracingQueryService _queryService;
        private readonly ILogger<TracingController> logger;
        public TracingController(TracingQueryService queryService, ILoggerFactory loggerFactory)
        {
            _queryService = queryService;
            logger = loggerFactory.CreateLogger<TracingController>();
        }

        // 空过滤
        private static readonly FilterDefinition<TracingObject> EmptyFilter = Builders<TracingObject>.Filter.Empty;
        // 只查询 Process.ServiceName
        private static readonly ProjectionDefinition<TracingObject> ServiceNameProjection = Builders<TracingObject>.Projection.Include("Process.ServiceName");



        /// <summary>
        /// 查询所有的服务
        /// </summary>
        /// <returns></returns>
        [HttpGet("services")]
        public async Task<QueryResponseServices<string>> Services()
        {
            return await _queryService.GetServices();
        }

        [HttpGet("dependencies")]
        public async Task<QueryResponseServices<SpanReference>> Dependencies(long endTs, long lookback)
        {
            return await _queryService.Dependencies(endTs - lookback, lookback);
        }

        /// <summary>
        /// 查询服务
        /// </summary>
        /// <returns></returns>
        [HttpGet("/traces/{serviceId}")]
        public async Task<QueryResponseServices<QueryTracingObject>> Services(string serviceId)
        {
            return await _queryService.GetService(serviceId);
        }

        /// <summary>
        /// 查询一个 service 中的 Operation 
        /// </summary>
        /// <returns></returns>
        [HttpGet("services/{service}/operations")]
        public async Task<QueryResponseServices<string>> ServiceOperation(string service)
        {
            if (string.IsNullOrWhiteSpace(service))
                return new QueryResponseServices<string>();

            return await _queryService.ServiceOperation(service);
        }

        /// <summary>
        /// 聚合搜索
        /// </summary>
        /// <returns></returns>
        [HttpGet("traces")]
        public async Task<QueryResponseServices<QueryTracingObject>> Traces(
            [Required] string service,
            string operation,
            string tags,
            int? minDuration,
            int? maxDuration,
            [Required] long start,
            [Required] long end,
            string lookback,
            int limit = 10
            )
        {
            if (string.IsNullOrWhiteSpace(service))
                return new QueryResponseServices<QueryTracingObject>();

            SearchTrace model = new SearchTrace();
            model.ServiceName = service;
            model.Operation = string.IsNullOrWhiteSpace(operation) ? null : operation;

            // 格式
            // http.status_code=200 error=true
            if (!string.IsNullOrWhiteSpace(tags))
            {
                try
                {
                    model.Tags = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(tags);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Tags 格式有误！", tags);
                    return new QueryResponseServices<QueryTracingObject>()
                    {
                        Errors = "Tags 格式有误！"
                    };
                }
            }


            model.MaxDuration = maxDuration;
            model.MinDuration = minDuration;

            model.Start = start;
            model.End = end;

            model.Limit = limit;

            return await _queryService.SearchTraces(model);
        }
    }
}
