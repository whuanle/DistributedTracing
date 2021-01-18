using AbpBase.Domain.Shared.Helpers;
using System;

namespace AbpBase.Domain.Shared.Apis
{
    /// <summary>
    /// Web 响应格式
    /// <para>避免滥用，此类不能实例化，只能通过预定义的静态方法生成</para>
    /// </summary>
    public abstract class ApiResponseModel : ApiResponseModel<dynamic>
    {
        /// <summary>
        /// 根据枚举创建响应格式
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="code"></param>
        /// <param name="enumType"></param>
        /// <returns></returns>
        public static ApiResponseModel Create<TEnum>(HttpStateCode code, TEnum enumType) where TEnum : Enum
        {
            return new PrivateApiResponseModel
            {
                StatuCode = code,
                Message = SchemeHelper.Get(enumType),
            };
        }

        /// <summary>
        /// 创建标准的响应
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <typeparam name="TData"></typeparam>
        /// <param name="code"></param>
        /// <param name="enumType"></param>
        /// <param name="Data"></param>
        /// <returns></returns>
        public static ApiResponseModel Create<TEnum>(HttpStateCode code, TEnum enumType, dynamic Data)
        {
            return new PrivateApiResponseModel
            {
                StatuCode = code,
                Message = SchemeHelper.Get(enumType),
                Data = Data
            };
        }

        /// <summary>
        /// 创建带自定义消息的响应
        /// <para>避免使用</para>
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <param name="Data"></param>
        /// <returns></returns>
        [Obsolete]
        public static ApiResponseModel Create(HttpStateCode code, string message, dynamic Data)
        {
            return new PrivateApiResponseModel
            {
                StatuCode = code,
                Message = message,
                Data = Data
            };
        }

        /// <summary>
        /// 请求支援成功
        /// </summary>
        /// <param name="code"></param>
        /// <param name="Data"></param>
        /// <returns></returns>
        public static ApiResponseModel CreateSuccess(HttpStateCode code, dynamic Data)
        {
            return new PrivateApiResponseModel
            {
                StatuCode = code,
                Message = "Success",
                Data = Data
            };
        }

        /// <summary>
        /// 私有类
        /// </summary>
        private class PrivateApiResponseModel : ApiResponseModel { }
    }
}
