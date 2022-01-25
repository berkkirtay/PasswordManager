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
        private PasswordContainerModel passwordContainerModel;
        private readonly IMediator mediator;

        public RequestHandler(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public void SetUserSession(string token)
        {
            passwordContainerModel = new PasswordContainerModel {userKeyToken = token};
        }

        public async Task<byte[]> HandleRequest(HttpListenerRequest req)
        {
            passwordContainerModel.passwordContainer = DeserializeInputStream(req.InputStream);

            if (req.RawUrl == "/addcredential" && req.HttpMethod == "POST")
            {
                await mediator.Send(new PostCredentialsRequest
                { passwordContainerModel = passwordContainerModel });
                currentRespond = req.RemoteEndPoint.ToString() +
                    $": Credential is successfully added by {passwordContainerModel.userKeyToken}.";
            }

            else if (req.RawUrl == "/getallusers" && req.HttpMethod == "GET")
            {
                var res = await mediator.Send(new GetAllUsersRequest
                { authToken = passwordContainerModel.userKeyToken });
                currentRespond = JsonConvert.SerializeObject(res, Formatting.Indented);
            }

            else if (req.RawUrl == "/getallcredentials" && req.HttpMethod == "GET")
            {
                var res = await mediator.Send(new GetCredentialsRequest
                { userCredentialToken = passwordContainerModel.userKeyToken });
                currentRespond = JsonConvert.SerializeObject(res, Formatting.Indented);
            }

            else if (req.RawUrl == "/reset" && req.HttpMethod == "POST")
            {
                await mediator.Send(new DeleteUserRequest { userCredentialToken = passwordContainerModel.userKeyToken });
                currentRespond = req.RemoteEndPoint.ToString() +
                                                $": User {passwordContainerModel.userKeyToken} is deleted.";
            }

            else if (req.RawUrl == "/deletecredential" && req.HttpMethod == "POST")
            {
                await mediator.Send(new DeleteCredentialRequest { passwordContainerModel = passwordContainerModel });
                currentRespond = req.RemoteEndPoint.ToString() +
                    $": Credential is successfully deleted by {passwordContainerModel.userKeyToken}.";
            }

            else if (req.RawUrl == "/updatecredential" && req.HttpMethod == "POST")
            {
                await mediator.Send(new UpdateCredentialRequest { passwordContainerModel = passwordContainerModel });
                currentRespond = req.RemoteEndPoint.ToString() +
                    $": Credential is successfully updated by {passwordContainerModel.userKeyToken}.";
            }

            else
            {
                throw new NotImplementedException();
            }

            return Encoding.UTF8.GetBytes(currentRespond);
        }

        private static List<PasswordContainer> DeserializeInputStream(Stream inputStream)
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
