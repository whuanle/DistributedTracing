using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AbpBase.Application.Handlers.HandlerEvents
{
    /// <summary>
    /// 全局异常推送事件
    /// </summary>
    public class CustomerExceptionEvent
    {
        /// <summary>
        /// 只记录异常
        /// </summary>
        /// <param name="ex"></param>
        public CustomerExceptionEvent(Exception ex)
        {
            Exception = ex;
        }

        /// <summary>
        /// 此异常发生时，用户请求的路由地址
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="actionRoute"></param>
        public CustomerExceptionEvent(Exception ex, string actionRoute)
        {
            Exception = ex;
            Action = actionRoute;
        }

        /// <summary>
        /// 此异常发生在哪个类型的方法中
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="method"></param>
        public CustomerExceptionEvent(Exception ex, MethodBase method)
        {
            Exception = ex;
            MethodInfo = (MethodInfo)method;
        }

        /// <summary>
        /// 记录异常信息
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="actionRoute"></param>
        /// <param name="method"></param>
        public CustomerExceptionEvent(Exception ex, string actionRoute, MethodBase method)
        {
            Exception = ex;
            Action = actionRoute;
            MethodInfo = (MethodInfo)method;
        }

        /// <summary>
        /// 当前出现位置
        /// <example>
        /// <code>
        /// MethodInfo = (MethodInfo)MethodBase.GetCurrentMethod();
        /// </code>
        /// </example>
        /// </summary>
        public MethodInfo MethodInfo { get; private set; }

        /// <summary>
        /// 发生异常的 Action
        /// </summary>
        public string Action { get; private set; }

        /// <summary>
        /// 具体异常
        /// </summary>
        public Exception Exception { get; private set; }
    }
}
