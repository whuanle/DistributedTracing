using FreeSql;
using System;
using System.Collections.Generic;
using System.Text;

namespace AbpBase.Database
{
    public partial class FreesqlContext
    {
        private static void OnModelCreatingPartial(IFreeSql freeSql)
        {
            var modelBuilder = freeSql.CodeFirst;

            SyncStruct(modelBuilder);

        }

        /// <summary>
        /// 同步结构到数据中
        /// </summary>
        /// <param name="codeFirst"></param>
        private static void SyncStruct(ICodeFirst codeFirst)
        {
            //  codeFirst.SyncStructure(typeof(user));
        }
    }
}
