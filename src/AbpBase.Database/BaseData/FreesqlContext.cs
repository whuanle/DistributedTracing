using FreeSql.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace AbpBase.Database
{
    /// <summary>
    /// Freesql 上下文
    /// </summary>
    public partial class FreesqlContext
    {
        public static IFreeSql FreeselInstance => Freesql_Instance;
        private static IFreeSql Freesql_Instance;

        public static void Init(string connectStr, FreeSql.DataType dataType = FreeSql.DataType.Sqlite)
        {
            Freesql_Instance = new FreeSql.FreeSqlBuilder()
                .UseNameConvert(NameConvertType.PascalCaseToUnderscore)
                .UseConnectionString(dataType, connectStr)

                //.UseAutoSyncStructure(true) // 自动同步实体结构到数据库，生产环境禁止使用！

                .Build();
            OnModelCreating(Freesql_Instance);
        }

        private static void OnModelCreating(IFreeSql freeSql)
        {


            OnModelCreatingPartial(freeSql);
        }
    }
}
