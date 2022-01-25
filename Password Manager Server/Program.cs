using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using MediatR;

namespace Password_Manager_Server
{
    class Program
    {
        static async Task Main()
        {
            var host = CreateHostBuilder().Build();

            Server server = new Server(host);
            server.StartServer();
            await host.WaitForShutdownAsync();
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
