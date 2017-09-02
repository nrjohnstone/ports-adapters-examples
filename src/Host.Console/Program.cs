using System.Threading;

namespace Host.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            Application application = new Application();

            application.Configure();
            application.Run();

            Thread.Sleep(5000);

            application.Shutdown();

        }
    }
}
