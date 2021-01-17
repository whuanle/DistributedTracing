using System;
using System.Collections.Generic;
using System.Text;

namespace AbpBase.Domain.Shared.Attributes
{
    /// <summary>
    /// 标记枚举代表的信息
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class SchemeNameAttribute : Attribute
    {
        public string Message { get; set; }
        public SchemeNameAttribute(string message)
        {
            Message = message;
        }
    }
}
