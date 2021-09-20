using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Password_Manager_Server
{
    using System.Net;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using System.IO;

    class Server
    {
        private const int PORT = 5000;
        private readonly HttpListener listener;

        static private PasswordManager passwordManagerSession = null;

        public Server()
        {
            listener = new HttpListener();
            listener.Prefixes.Add($"http://localhost:{PORT}/");
        }

        public async void StartServer()
        {
            await ListenerLoop(listener);
        }
        static private async Task ListenerLoop(HttpListener listener)
        {
            listener.Start();
            while (true)
            {
                try
                {
                    var context = await listener.GetContextAsync();
                    lock (listener)
                    {
                        HandleRequest(context);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    return;
                }
            }
        }

        static private void HandleRequest(HttpListenerContext context)
        {
            // This statement looks like a method version of proxy pattern.
            // When we get a new request we do not need to instantiate a new object again.

            try
            {
                if (passwordManagerSession == null)
                {
                    passwordManagerSession = new PasswordManager(context);
                }
                else
                {
                    passwordManagerSession.Invoke(context);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
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