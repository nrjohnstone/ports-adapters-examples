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
            Application application = new Application();

            application.Configure();
            application.Run();

            Thread.Sleep(5000);

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
}
