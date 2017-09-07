using System;
using System.Net.Http;
using Microsoft.Owin.Hosting;

namespace Host.WebService1
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