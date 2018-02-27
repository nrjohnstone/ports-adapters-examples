using System;
using Microsoft.Owin.Hosting;
using Serilog;

namespace Host.WebService.Client1
{
    public class Program
    {
        static void Main(string[] args)
        {
            ConfigureSerilog();
            string baseAddress = "http://localhost:10008/";

            using (WebApp.Start<Startup>(url: baseAddress))
            {
                Console.WriteLine($"Running on {baseAddress}");
                Console.WriteLine("Hit enter to exit");
                Console.ReadLine();
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