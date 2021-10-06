using Microsoft.Extensions.Hosting;
using Serilog;
using SimpleInjector;

namespace HostApp.WebService.Client3
{
    public class Program
    {
        private static IHost _host;
        public static string ApplicationName = "HostApp.WebService.Client3";

        static void Main(string[] args)
        {
            ConfigureSerilog();
            Container container = new Container();

            using (_host = new ApplicationHostBuilder(args, ApplicationName,  container).Build())
            {
                // Use "Start" and "WaitForShutdown" instead of "Run" as this handles being stopped
                // from a container and allow the CloseAndFlush to be called
                _host.Start();
                _host.WaitForShutdown();

                // Important to CloseAndFlush the logs inside the using to ensure all log 
                // messages from services are in the buffer before CloseAndFlush is called
                Log.Information("Shutting down...");
                Log.CloseAndFlush();
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