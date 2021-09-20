﻿using System;
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

        private List<string> authorizedUsers = new List<string>();


        public PasswordManager(HttpListenerContext context)
        {
            requestHandler = new RequestHandler();
            //  SetAuthorizationToken();
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
         /*   bool userFlag = false;
            authorizedUsers.ForEach(user => 
            {
                if (authorizationKey.Contains(user))
                {
                    userFlag = true;
                }
            }); */

            if (authorizationKey.Length != 0)
            {
                authorizationStatus = true;
                context.Response.StatusCode = 201;
                context.Response.StatusDescription = "Authorized session.";
                requestHandler.SetUserSession(authorizationKey);
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
            authorizedUsers.Add("berk");
            List<string> generatedTokens = new List<string>();
            authorizedUsers.ForEach(user =>
            {
                var token = EncryptionManager.HashData(user);
                generatedTokens.Add(Convert.ToBase64String(token));
            });
            authorizedUsers = generatedTokens;
            authorizedUsers.Add("test");
        }
    }

}
