using AbpBase.Domain.Shared.Apis;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace AbpBase.Web.Filters
{
    public static class InvalidModelStateFilter
    {
        /// <summary>
        /// 统一模型验证
        /// <para>控制器必须添加 [ApiController] 才能被此过滤器拦截</para>
        /// </summary>
        /// <param name="services"></param>
        public static void GlabalInvalidModelStateFilter(this IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = actionContext =>
                {
                    if (actionContext.ModelState.IsValid)
                        return new BadRequestObjectResult(actionContext.ModelState);

                    int count = actionContext.ModelState.Count;
                    ValidationErrors[] errors = new ValidationErrors[count];
                    int i = 0;
                    foreach (var item in actionContext.ModelState)
                    {
                        errors[i] = new ValidationErrors
                        {
                            Member = item.Key,
                            Messages = item.Value.Errors?.Select(x => x.ErrorMessage).ToArray()
                        };
                        i++;
                    }

                    // 响应消息
                    var result = ApiResponseModel.Create(HttpStateCode.Status400BadRequest, CommonResponseType.BadRequest, errors);
                    var objectResult = new BadRequestObjectResult(result);
                    objectResult.StatusCode = StatusCodes.Status400BadRequest;
                    return objectResult;
                };
            });
        }


        /// <summary>
        /// 用于格式化实体验证信息的模型
        /// </summary>
        private class ValidationErrors
        {
            /// <summary>
            /// 验证失败的字段
            /// </summary>
            public string Member { get; set; }

            /// <summary>
            /// 此字段有何种错误
            /// </summary>
            public string[] Messages { get; set; }
        }
    }
}