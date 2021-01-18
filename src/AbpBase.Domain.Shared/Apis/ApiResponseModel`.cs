namespace AbpBase.Domain.Shared.Apis
{
    /// <summary>
    /// API 响应格式
    /// <para>避免滥用，此类不能实例化，只能通过预定义的静态方法生成</para>
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public abstract class ApiResponseModel<TData>
    {
        public HttpStateCode StatuCode { get; set; }
        public string Message { get; set; }
        public TData Data { get; set; }


        /// <summary>
        /// 私有类
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        private class PrivateApiResponseModel<TResult> : ApiResponseModel<TResult> { }
    }
}
