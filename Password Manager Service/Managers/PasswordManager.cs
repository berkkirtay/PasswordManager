using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net;
using System.IO;
using System.Security.Cryptography;

namespace PasswordManagerService
{
    class PasswordManager : IServerManager
    {
        private bool authorizationStatus = false;
        private RequestHandler requestHandler = null;
        private const string authKey = "berk";

        public PasswordManager(HttpListenerContext context)
        {
            requestHandler = new RequestHandler();
            Invoke(context);
        }

        public void Invoke(HttpListenerContext context)
        {
            if (authorizationStatus == true)
            {
                requestHandler.HandleRequests(context);
            }
            else
            {
                HandleAuthorization(context);
            }
        }

        private void HandleAuthorization(HttpListenerContext context)
        {
            var req = context.Request;
            StreamReader reader = new StreamReader(req.InputStream);
            if (reader.ReadToEnd().Equals(authKey))
            {
                context.Response.StatusCode = 201;
                context.Response.StatusDescription = "Authorized session.";
                authorizationStatus = true;
                Server.SendDataToClient(
                    context, Encoding.UTF8.GetBytes("You are authorized."));
            }
            else
            {
                Server.SendDataToClient(
                    context, Encoding.UTF8.GetBytes("AuthErr"));
            }
        }
    }

    public struct PasswordContainer
    {
        public string userID;
        public string password;

        public PasswordContainer(string userID, string password)
        {
            this.userID = userID;
            this.password = password;
        }

        public override string ToString()
        {
            return userID + " " + password;
        } 
    }
}
