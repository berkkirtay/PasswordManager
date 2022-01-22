using System;
using Microsoft.Extensions.Hosting;
using MediatR;

namespace Password_Manager_Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = CreateHostBuilder().Build();

            Server server = new Server(host);
            server.StartServer();
            System.Threading.Thread.Sleep(int.MaxValue);
        }

        private static IHostBuilder CreateHostBuilder()
        {

            return Host.CreateDefaultBuilder()
                .ConfigureServices((hostContext, services) =>
                    services
                    .AddMediatR(typeof(Program).Assembly)
                    );
        }

    }
}
