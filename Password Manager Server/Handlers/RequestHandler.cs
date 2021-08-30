using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace Password_Manager_Server
{

    class RequestHandler
    {
        private CredentialManager credentialManager = null;
        private string currentRespond;

        public RequestHandler()
        {
            credentialManager = new CredentialManager();
        }

        public void SetUserSession(string token)
        {
            credentialManager.CreateUserCredentialSession(token);
        }

        public void HandleRequests(HttpListenerContext context)
        {
            var req = context.Request;
            if (req.RawUrl == "/importNewContainer")
            {
                if (req.HttpMethod == "POST")
                {
                    ImportNewContainer(context);
                }
            }

            else if (req.RawUrl == "/addCredential")
            {
                AddCredential(context);
            }

            else if (req.RawUrl == "/getAllCredentials")
            {
                SendAllCredentials(context);
            }

            else if (req.RawUrl == "/reset")
            {
                credentialManager.ResetCredentials();
                currentRespond = "Credential database is removed.";
            }
            else
            {
                context.Response.StatusCode = 501;
                currentRespond = "Not implemented!";
            }

            Server.SendDataToClient(
                    context, Encoding.UTF8.GetBytes(currentRespond));

            credentialManager.SetUserData();
        }

        private void ImportNewContainer(HttpListenerContext context)
        {
            credentialManager.SetPasswordsData(context);
            currentRespond = "A new password container imported by " +
                            context.Request.RemoteEndPoint.ToString();
            Console.WriteLine(currentRespond);
        }

        private void AddCredential(HttpListenerContext context)
        {
            credentialManager.SetPasswordsData(context);
            currentRespond = "A new password is inserted by " +
                            context.Request.RemoteEndPoint.ToString();
            Console.WriteLine(currentRespond);
        }

        private void SendAllCredentials(HttpListenerContext context)
        {
            currentRespond = JsonConvert.SerializeObject(
                    credentialManager.GetPasswordContainer(),
                                        Formatting.Indented);
            Console.WriteLine("Password container is sent to " +
                                context.Request.RemoteEndPoint.ToString());
        }
    }
}
