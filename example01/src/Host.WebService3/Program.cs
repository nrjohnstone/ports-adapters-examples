using System;
using Microsoft.Owin.Hosting;
using Serilog;

namespace Host.WebService.Client3
{
    public class Program
    {
        static void Main(string[] args)
        {
            ConfigureSerilog();
            string baseAddress = "http://localhost:10010/";

            Startup startup = new Startup();
            using (WebApp.Start(baseAddress, startup.Configuration))
            {
                Console.WriteLine("Hit enter to exit");
                Console.ReadLine();
                startup.Shutdown();
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