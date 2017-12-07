using System;
using Microsoft.Owin.Hosting;

namespace Host.WebService.Client3
{
    public class Program
    {
        static void Main(string[] args)
        {
            string baseAddress = "http://localhost:10010/";

            Startup startup = new Startup();
            using (WebApp.Start(baseAddress, startup.Configuration))
            {
                Console.WriteLine("Hit enter to exit");
                Console.ReadLine();
                startup.Shutdown();
            }
        }
    }
}