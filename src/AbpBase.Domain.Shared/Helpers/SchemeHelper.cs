using AbpBase.Domain.Shared.Attributes;
using System;
using System.Linq;
using System.Reflection;

namespace AbpBase.Domain.Shared.Helpers
{
    /// <summary>
    /// 获取各种枚举代表的信息
    /// </summary>
    public static class SchemeHelper
    {
        private static readonly PropertyInfo SchemeNameAttributeMessage = typeof(SchemeNameAttribute).GetProperty(nameof(SchemeNameAttribute.Message));

        /// <summary>
        /// 获取一个使用了 SchemeNameAttribute 特性的 Message 属性值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string Get<T>(T type)
        {
            return GetValue(type);
        }

        private static string GetValue<T>(T type)
        {
            var attr = typeof(T).GetField(Enum.GetName(type.GetType(), type))
                .GetCustomAttributes()
                .FirstOrDefault(x => x.GetType() == typeof(SchemeNameAttribute));

            if (attr == null)
                return string.Empty;

            var value = (string)SchemeNameAttributeMessage.GetValue(attr);
            return value;
        }
    }
}