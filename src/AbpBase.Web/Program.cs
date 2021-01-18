using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;


namespace AbpBase.Web
{
    public class Program
    {
        public static int Main(string[] args)
        {
            try
            {
                ConfigLog();
                Serilog.Log.Information("Starting web host.");
                CreateHostBuilder(args).Build().Run();
                return 0;
            }
            catch (Exception ex)
            {
                Serilog.Log.Fatal("Host terminated unexpectedly!");
                Serilog.Log.Fatal(ex.Message);
                return 1;
            }
            finally
            {
                Serilog.Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
            .UseAutofac()
            .UseSerilog();



        private static void ConfigLog()
        {
            Serilog.Log.Logger = new LoggerConfiguration()
#if DEBUG
                .MinimumLevel.Debug()
#else
                .MinimumLevel.Information()
#endif
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Async(c => c.File($"Logs/{DateTime.Now.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.CurrentCulture)}-logs.txt"))
                .WriteTo.Logger(log =>
                        log.Filter.ByIncludingOnly(evt => evt.Level == LogEventLevel.Fatal)
                            .WriteTo.File(
                                $"Logs/{(DateTime.Now.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.CurrentCulture) + "-Fatal.txt")}",
                                fileSizeLimitBytes: 83886080),
                    LogEventLevel.Fatal)
                .WriteTo.Logger(log =>
                        log.Filter.ByIncludingOnly(evt => evt.Level == LogEventLevel.Error)
                            .WriteTo.File(
                                $"Logs/{(DateTime.Now.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.CurrentCulture) + "-Error.txt")}",
                                fileSizeLimitBytes: 83886080),
                    LogEventLevel.Fatal)
                .WriteTo.Console()
                .CreateLogger();
        }
    }
}
