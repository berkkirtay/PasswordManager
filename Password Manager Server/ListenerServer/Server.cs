using System;
using System.Collections.Generic;
using System.Linq;
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
        private const int PORT = 8000;
        private readonly HttpListener listener;
        private IHost host;
        static private PasswordManager passwordManagerSession = null;

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
                var context = await listener.GetContextAsync();
                lock (listener)
                {
                    HandleRequest(context);
                }
            }
        }

        private void HandleRequest(HttpListenerContext context)
        {
            // This statement looks like a method version of proxy pattern.
            // When we get a new request we do not need to instantiate a new object again.
            try
            {     
                if (context.Request.HttpMethod == "OPTIONS")
                {
                    SendDataToClient(context, Encoding.UTF8.GetBytes(""));
                    return;
                }


                if (passwordManagerSession == null)
                {
                    var mediator = host.Services.GetService<IMediator>();
                    passwordManagerSession = new PasswordManager(context, mediator);
                }
                else
                {
                    passwordManagerSession.Invoke(context);
                }
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 500;
                context.Response.StatusDescription = "Internal server error.";
                SendDataToClient(context, Encoding.UTF8.GetBytes(ex.Message));
            }
        }

    

        static public void SendDataToClient(HttpListenerContext httpListenerContext, byte[] buffer)
        {
            using (var response = httpListenerContext.Response)
            {
                response.ContentType = "application/json";
                response.ContentLength64 = buffer.Length;
                response.OutputStream.Write(buffer, 0, buffer.Length);
                response.Close();
            }
        }
    }
}