using System;
using AmbientContext.LogService.Serilog;
using Serilog;

namespace Host.Console
{
    class Program
    {
        public static AmbientLogService Logger = new AmbientLogService();

        static void Main(string[] args)
        {
            ConfigureSerilog();
            Logger.Debug("Application starting");

            try
            {
                Settings settings = new Settings()
                {
                    TriggerAdapter = "Test",
                    NotificationAdapter = "Email",
                    PersistenceAdapter = "Test",
                    PersistenceConnectionString = "server=127.0.0.1;" +
                                                  "uid=bookorder_service;" +
                                                  "pwd=123;" +
                                                  "database=bookorders"
                };

                Application application = new Application(settings);

                application.Configure();
                application.Run();
                
                System.Console.ReadLine();

                Logger.Debug("Application shutting down");
                application.Shutdown();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ex.StackTrace);
            }                     
        }

        private static void ConfigureSerilog()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.ColoredConsole()
                .MinimumLevel.Debug()
                .CreateLogger();
        }
    }
}
