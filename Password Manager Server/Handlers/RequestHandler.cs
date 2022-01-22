using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using MediatR;

namespace Password_Manager_Server
{
    class RequestHandler
    {
        private string currentRespond;
        private string userKeyToken;
        private IMediator mediator;

        public RequestHandler(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public void SetUserSession(string token)
        {
            userKeyToken = token;
        }

        public async Task<byte[]> HandleRequest(HttpListenerRequest req)
        {
            if (req.RawUrl == "/importNewContainer" && req.HttpMethod == "POST")
            {
                var credentials = DeserializeData(req.InputStream);
                foreach(var credential in credentials)
                {
                    await mediator.Send(new PostCredentialsRequest
                    { userCredentialToken=userKeyToken, passwordContainer = new PasswordContainer(credential.userID, credential.password) });
                }

                currentRespond = req.RemoteEndPoint.ToString() + 
                    $": A new password container imported by {userKeyToken}.";                            
            }

            else if (req.RawUrl == "/addCredential" && req.HttpMethod == "POST")
            {
                var credential = DeserializeData(req.InputStream)[0];
                await mediator.Send(new PostCredentialsRequest
                { userCredentialToken = userKeyToken, passwordContainer = new PasswordContainer(credential.userID, credential.password) });

                currentRespond = req.RemoteEndPoint.ToString() + 
                    $": A new password is inserted by {userKeyToken}.";                                          
            }

            else if (req.RawUrl == "/getAllCredentials" && req.HttpMethod == "GET")
            {
                var res = await mediator.Send(new GetCredentialsRequest 
                                            { userCredentialToken = userKeyToken });
                currentRespond = JsonConvert.SerializeObject(res, Formatting.Indented);
            }

            else if (req.RawUrl == "/reset" && req.HttpMethod == "POST")
            {
                await mediator.Send(new DeleteUserRequest { userCredentialToken = userKeyToken });
                currentRespond = req.RemoteEndPoint.ToString() + 
                                                $": User {userKeyToken} is deleted.";
            }
            else
            {
                throw new NotImplementedException();
            }

            return Encoding.UTF8.GetBytes(currentRespond);
        }

        private List<PasswordContainer> DeserializeData(Stream inputStream)
        {
            StreamReader reader = new StreamReader(inputStream);
            return JsonConvert.DeserializeObject<List<PasswordContainer>>(reader.ReadToEnd());
        }

        private void SaveContainerToLocal(List<PasswordContainer> credentials)
        {
            string container = JsonConvert.SerializeObject(
                credentials, Formatting.Indented);
            var writer = new StreamWriter("PasswordContainerSave.json");
            writer.Write(container);
            writer.Close();
        }
    }
}
