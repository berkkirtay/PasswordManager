using System.Text;
using System.Threading.Tasks;
using System.Net;
using MediatR;
using System.Net.Http.Headers;

namespace Password_Manager_Server
{
    class PasswordManager : IPasswordManager
    {
        private bool authorizationStatus = false;
        private readonly RequestHandler requestHandler;

        public PasswordManager(IMediator mediator)
        {
            requestHandler = new RequestHandler(mediator);
        }

        public async Task Invoke(HttpListenerContext context)
        {
            if (authorizationStatus == true)
            {
                var res = await requestHandler.HandleRequest(context.Request);
                authorizationStatus = false;
                await Server .SendDataToClient(context, res);
            }
            else
            {
                await HandleAuthorization (context);
            }
        }

        // We check if the client has the authorization token.
        // If the authorization key is invalid then password manager
        // will block every request.
        
        private async Task HandleAuthorization(HttpListenerContext context)
        {
            var req = context.Request;
            string authorizationHeader = req.Headers.Get("Authorization");

            if (authorizationHeader.Length != 0)
            {
                var authorizationToken = AuthenticationHeaderValue.Parse(authorizationHeader);
                requestHandler.SetUserSession(authorizationToken.Parameter);
                authorizationStatus = true;
                context.Response.StatusCode = 201;
                context.Response.StatusDescription = "Authorized session.";
                await Invoke(context);
            }
            else
            {
                context.Response.StatusCode = 401;
                context.Response.StatusDescription = "AuthErr";
                await Server.SendDataToClient( context, Encoding.UTF8.GetBytes("AuthErr"));
            }
        }
    }
}
