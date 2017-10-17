using System;
using Microsoft.Owin.Hosting;

namespace Host.WebService.Client1
{
    public class Program
    {
        static void Main(string[] args)
        {
            string baseAddress = "http://localhost:10008/";

            using (WebApp.Start<Startup>(url: baseAddress))
            {                
                Console.WriteLine("Hit enter to exit");                
                Console.ReadLine();
            }
        }
    }
}