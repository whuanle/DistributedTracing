using System;
using System.Collections.Generic;
using System.Text;

namespace AbpBase.Domain.Shared
{
    /// <summary>
    /// 全局共享内容
    /// </summary>
    public static class WholeShared
    {
        // 数据库连接属性可以自行在配置文件中定义，这里写固定的，只是为了演示

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public static readonly string SqlConnectString = "";

        /// <summary>
        /// 要使用的数据库类型
        /// </summary>
        public static readonly AbpBaseDataType DataType = AbpBaseDataType.Sqlite;
    }
}
