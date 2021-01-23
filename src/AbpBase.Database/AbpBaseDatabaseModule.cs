using AbpBase.Domain;
using AbpBase.Domain.Shared;
using Microsoft.Extensions.DependencyInjection;
using System;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.MySQL;
using Volo.Abp.EntityFrameworkCore.Sqlite;
using Volo.Abp.EntityFrameworkCore.SqlServer;
using Volo.Abp.Modularity;

namespace AbpBase.Database
{
    [DependsOn(
        typeof(AbpBaseDomainModule),
        typeof(AbpEntityFrameworkCoreModule),
        typeof(AbpEntityFrameworkCoreSqliteModule),
        typeof(AbpEntityFrameworkCoreSqlServerModule),
        typeof(AbpEntityFrameworkCoreMySQLModule)
        )]
    public class AbpBaseDatabaseModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddAbpDbContext<AbpBaseDataContext>();

            string connectString = WholeShared.SqlConnectString;

            Configure<AbpDbConnectionOptions>(options =>
            {
                options.ConnectionStrings.Default = connectString;
            });


            FreeSql.DataType dataType = default;
            switch (WholeShared.DataType)
            {
                case AbpBaseDataType.Sqlite:
                    dataType = FreeSql.DataType.Sqlite; break;
                case AbpBaseDataType.Mysql:
                    dataType = FreeSql.DataType.MySql; break;
                case AbpBaseDataType.Sqlserver:
                    dataType = FreeSql.DataType.SqlServer; break;
            }

            Configure<AbpDbContextOptions>(options =>
            {
                switch (WholeShared.DataType)
                {
                    case AbpBaseDataType.Sqlite:
                        options.UseSqlite<AbpBaseDataContext>(); dataType = FreeSql.DataType.Sqlite; break;
                    case AbpBaseDataType.Mysql:
                        options.UseMySQL<AbpBaseDataContext>(); dataType = FreeSql.DataType.MySql; break;
                    case AbpBaseDataType.Sqlserver:
                        options.UseSqlServer<AbpBaseDataContext>(); dataType = FreeSql.DataType.SqlServer; break;
                }
            });


            FreesqlContext.Init(connectString, dataType);
            context.Services.AddSingleton(typeof(IFreeSql), FreesqlContext.FreeselInstance);
            context.Services.AddTransient(typeof(FreesqlContext), typeof(FreesqlContext));

        }
    }
}
