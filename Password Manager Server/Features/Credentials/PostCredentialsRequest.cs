using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using System.Threading;

namespace Password_Manager_Server
{
    public class PostCredentialsRequest : IRequest
    {
        public string userCredentialToken { get; set; }

        public PasswordContainer passwordContainer { get; set; }
    }

    internal class PostCredentialsRequestHandler : IRequestHandler<PostCredentialsRequest, Unit>
    {
        public async Task<Unit> Handle(PostCredentialsRequest request, CancellationToken cancellationToken)
        {
            var sqlHelper = new MysqlHelper(request.userCredentialToken);
            await sqlHelper.InsertPassword(request.passwordContainer.userID, request.passwordContainer.password);
            return Unit.Value;
        }
    }
}
