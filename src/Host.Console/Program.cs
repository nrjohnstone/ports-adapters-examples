using System.Threading;
using AmbientContext.LogService.Serilog;
using Serilog;
using Serilog.Core;

namespace Host.Console
{
    class Program
    {
        public static AmbientLogService Logger = new AmbientLogService();

        static void Main(string[] args)
        {
            ConfigureSerilog();
            Logger.Debug("Application starting");

            Settings settings = new Settings()
            {
                TriggerAdapter = "Test",
                NotificationAdapter = "RabbitMq",
                PersistenceAdapter = "Test"
            };

            Application application = new Application(settings);

            application.Configure();
            application.Run();
            
            System.Console.ReadLine();
            
            Logger.Debug("Application shutting down");
            application.Shutdown();
        }

        private static void ConfigureSerilog()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.ColoredConsole()
                .MinimumLevel.Debug()
                .CreateLogger();
        }
    }

    public class Settings
    {
        public string TriggerAdapter { get; set; }
        public string NotificationAdapter { get; set; }
        public string PersistenceAdapter { get; set; }
    }
}
