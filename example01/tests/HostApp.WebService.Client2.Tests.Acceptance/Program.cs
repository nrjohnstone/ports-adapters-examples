using System;
using HostApp.WebService.Client2.Tests.Acceptance.Scenarios;
using RabbitMQ.Client;

namespace HostApp.WebService.Client2.Tests.Acceptance
{
    class Program
    {
        static void Main(string[] args)
        {
            string UserName = "guest";
            string Password = "guest";
            string HostName = "localhost";

            var connectionFactory = new ConnectionFactory()
            {
                UserName = UserName,
                Password = Password,
                HostName = HostName
            };
            
            Scenario1 scenario1 = new Scenario1(connectionFactory);
            scenario1.Run();
        }
    }
}
