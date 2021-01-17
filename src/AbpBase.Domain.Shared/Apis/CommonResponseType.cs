using AbpBase.Domain.Shared.Attributes;

namespace AbpBase.Domain.Shared.Apis
{
    /// <summary>
    /// 常用的 API 响应信息
    /// </summary>
    public enum CommonResponseType
    {
        [SchemeName("")] Default = 0,

        [SchemeName("请求成功")] RequstSuccess = 1,

        [SchemeName("请求失败")] RequstFail = 2,

        [SchemeName("创建资源成功")] CreateSuccess = 4,

        [SchemeName("创建资源失败")] CreateFail = 8,

        [SchemeName("更新资源成功")] UpdateSuccess = 16,

        [SchemeName("更新资源失败")] UpdateFail = 16,

        [SchemeName("删除资源成功")] DeleteSuccess = 32,

        [SchemeName("删除资源失败")] DeleteFail = 64,

        [SchemeName("请求的数据未能通过验证")] BadRequest = 128,

        [SchemeName("服务器出现严重错误")] Status500InternalServerError = 256
    }
}
