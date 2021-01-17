using AbpBase.Application.Handlers.HandlerEvents;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;

namespace AbpBase.Application.Handlers
{
    /// <summary>
    /// 全局异常记录日志
    /// </summary>
    public class CustomerExceptionHandler : ILocalEventHandler<CustomerExceptionEvent>, ITransientDependency
    {
        private readonly ILogger _ILogger;

        public CustomerExceptionHandler(ILogger logger)
        {
               _ILogger = logger;
        }

        public async Task HandleEventAsync(CustomerExceptionEvent eventData)
        {
            StringBuilder stringBuilder = new StringBuilder(256);
            stringBuilder.AppendLine();
            stringBuilder.Append("Action:    ");
            stringBuilder.AppendLine(eventData.Action);
            if (eventData.MethodInfo != null)
            {
                stringBuilder.Append("Class-Method:    ");
                stringBuilder.Append(eventData.MethodInfo?.DeclaringType.FullName);
                stringBuilder.AppendLine(eventData.MethodInfo?.Name);
            }

            stringBuilder.Append("Source:    ");
            stringBuilder.AppendLine(eventData.Exception.Source);
            stringBuilder.Append("TargetSite:    ");
            stringBuilder.AppendLine(eventData.Exception.TargetSite?.ToString());
            stringBuilder.Append("InnerException:    ");
            stringBuilder.AppendLine(eventData.Exception.InnerException?.ToString());
            stringBuilder.Append("Message:    ");
            stringBuilder.AppendLine(eventData.Exception.Message);
            stringBuilder.Append("HelpLink:    ");
            stringBuilder.AppendLine(eventData.Exception.HelpLink);
            _ILogger.Fatal(stringBuilder.ToString());
            await Task.CompletedTask;
        }
    }
}
