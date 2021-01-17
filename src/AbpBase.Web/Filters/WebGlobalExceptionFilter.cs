using AbpBase.Application.Handlers.HandlerEvents;
using AbpBase.Domain.Shared.Apis;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System;
using System.Reflection;
using System.Threading.Tasks;
using Volo.Abp.EventBus.Local;

namespace AbpBase.Web.Filters
{

    /// <summary>
    /// Web 全局异常过滤器，处理 Web 中出现的、运行时未处理的异常
    /// </summary>
    public class WebGlobalExceptionFilter : IAsyncExceptionFilter
    {
        private readonly ILocalEventBus _localEventBus;

        public WebGlobalExceptionFilter(ILocalEventBus localEventBus)
        {
            _localEventBus = localEventBus;
        }

        public async Task OnExceptionAsync(ExceptionContext context)
        {

            if (!context.ExceptionHandled)
            {
                await _localEventBus.PublishAsync(new CustomerExceptionEvent(context.Exception,
                    context.ActionDescriptor?.DisplayName));

                ApiResponseModel model = ApiResponseModel.Create(
                    HttpStateCode.Status500InternalServerError,
                    CommonResponseType.Status500InternalServerError);

                context.Result = new ContentResult
                {
                    Content = JsonConvert.SerializeObject(model),
                    StatusCode = StatusCodes.Status200OK,
                    ContentType = "application/json; charset=utf-8"
                };
            }

            context.ExceptionHandled = true;

            await Task.CompletedTask;
        }
    }
}
