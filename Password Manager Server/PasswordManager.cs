﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net;
using System.IO;
using System.Security.Cryptography;

namespace Password_Manager_Server
{
    class PasswordManager : IServerManager
    {
        private bool authorizationStatus = true;
        private RequestHandler requestHandler = null;

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
                context.Response.StatusCode = 503;
                HandleAuthorization(context);
            }
        }

        private void HandleAuthorization(HttpListenerContext context)
        {
            var req = context.Request;
            StreamReader reader = new StreamReader(req.InputStream);
            if (reader.ReadToEnd().Equals("12345"))
            {
                context.Response.StatusCode = 201;
                context.Response.StatusDescription = "Authorized session.";
                authorizationStatus = true;
            }
            else
            {
              //  currentRespond =
                //    "Please send your key first for authorization.";
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
