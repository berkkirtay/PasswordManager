using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net;
using System.IO;

namespace Password_Manager_Server
{
    class PasswordManager : IServerManager
    {
        private bool authorizationStatus = false;
        private RequestHandler requestHandler = null;
        private string authKey = "berk";

        public PasswordManager(HttpListenerContext context)
        {
            requestHandler = new RequestHandler();
            SetAuthorizationToken();
            Invoke(context);
        }

        public void Invoke(HttpListenerContext context)
        {
           
            if (authorizationStatus == true)
            {
                requestHandler.HandleRequests(context);
                authorizationStatus = false;
            }
            else
            {
                HandleAuthorization(context);
            }
        }

        // We check if the client has the authorization token.
        // If the authorization key is invalid then password manager
        // will block every request.
        
        private void HandleAuthorization(HttpListenerContext context)
        {
            var req = context.Request;
            var authorizationKey = req.Headers.Get("Authorization");

            if (authorizationKey.Contains(authKey))
            {
                authorizationStatus = true;
                context.Response.StatusCode = 201;
                context.Response.StatusDescription = "Authorized session.";
                Invoke(context);
            }
            else
            {
                context.Response.StatusCode = 401;
                context.Response.StatusDescription = "AuthErr";
                Server.SendDataToClient(
                        context, Encoding.UTF8.GetBytes("AuthErr"));
            }

        }

        private void SetAuthorizationToken()
        {
            var token = EncryptionManager.HashData(authKey);
            authKey = Convert.ToBase64String(token);
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
