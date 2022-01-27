using System;
using System.Threading;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MediatR;

namespace Password_Manager_Server
{
    using System.Net;
    using System.Threading.Tasks;

    class Server
    {
        private IHost host;
        private const int PORT = 8000;
        private readonly HttpListener listener;
        private static SemaphoreSlim semaphore = new SemaphoreSlim(1, 10);
        private static PasswordManager passwordManagerSession = null;

        public Server(IHost host)
        {
            this.host = host;
            listener = new HttpListener();
            listener.Prefixes.Add($"http://localhost:{PORT}/");
        }

        public async void StartServer()
        {
            await ListenerLoop(listener);
        }
        private async Task ListenerLoop(HttpListener listener)
        {

            listener.Start();
            while (true)
            {
                try
                {
                    var context = await listener.GetContextAsync();
                    await semaphore.WaitAsync();
                    await HandleRequest(context);
                }
                finally
                {
                    semaphore.Release();
                }
            }
        }

        // Cors support is needed for a client app.
        static private void IncludeCorsSupport(HttpListenerContext context)
        {
            var res = context.Response;
            res.AddHeader("Access-Control-Allow-Origin", "http://localhost:3000");
            res.AddHeader("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
            res.AddHeader("Access-Control-Allow-Headers", "*");
            res.StatusCode = 200;
        }

        private async Task HandleRequest(HttpListenerContext context)
        {
            // This statement looks like a method version of proxy pattern.
            // When we get a new request we do not need to instantiate a new object again.

            try
            {
                IncludeCorsSupport(context);
                if (context.Request.HttpMethod == "OPTIONS")
                {
                    await SendDataToClient(context, Encoding.UTF8.GetBytes(""));
                    return;
                }

                if (passwordManagerSession == null)
                {
                    var mediator = host.Services.GetService<IMediator>();
                    passwordManagerSession = new PasswordManager(mediator);
                }

                await passwordManagerSession.Invoke(context);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 500;
                context.Response.StatusDescription = "Internal server error.";
                await SendDataToClient (context, Encoding.UTF8.GetBytes(ex.Message));
            }
        }

        static public async Task SendDataToClient(HttpListenerContext httpListenerContext, byte[] buffer)
        {
            var res = httpListenerContext.Response;
            res.ContentType = "application/json";
            res.ContentLength64 = buffer.Length;
            await res.OutputStream.WriteAsync(buffer.AsMemory(0, buffer.Length));
            res.Close();
        }
    }
}