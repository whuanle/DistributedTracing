using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace AbpBase.Database
{
    /// <summary>
    /// 上下文
    /// <para>这部分用于定义和配置基础表的映射</para>
    /// </summary>
    [ConnectionStringName("Default")]
    public partial class AbpBaseDataContext : AbpDbContext<AbpBaseDataContext>
    {

        #region 定义 DbSet<T>

        #endregion


        public AbpBaseDataContext(DbContextOptions<AbpBaseDataContext> options)
    : base(options)
        {
        }

        /// <summary>
        /// 定义映射
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region 定义 映射

            #endregion

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
