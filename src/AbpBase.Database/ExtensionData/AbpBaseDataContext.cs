using Microsoft.EntityFrameworkCore;

namespace AbpBase.Database
{

    public partial class AbpBaseDataContext
    {
        #region 定义 DbSet<T>

        #endregion

        /// <summary>
        /// 定义映射
        /// </summary>
        /// <param name="modelBuilder"></param>
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {

        }
    }
}
