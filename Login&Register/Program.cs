using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using NLog.Extensions.Logging;
using NLog;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NLog.Web;
using DotNetEnv;

namespace LoginRegisterAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            
            DotNetEnv.Env.Load();
                        
            CreateHostBuilder(args).Build().Run();
            var logger = NLog.Web.NLogBuilder.ConfigureNLog("NLog.config").GetCurrentClassLogger();
            try
            {
                logger.Debug("Application Starting Up");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception exception)
            {                
                logger.Error(exception, "Stopped program because of exception");
                throw;
            }
            finally
            {
                NLog.LogManager.Shutdown();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)            
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();                    
                    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Debug);
                })
            .UseNLog();

    }
}
