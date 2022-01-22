using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using System.Threading;

namespace Password_Manager_Server.Features.Credentials
{
    public class UpdateCredentialRequest : IRequest
    {
        public string userCredentialToken { get; set; }
        public int credentialID { get; set; }
        public PasswordContainer passwordContainer { get; set; }
    }

    internal class UpdateCredentialRequestHandler : IRequestHandler<UpdateCredentialRequest, Unit>
    {
        public async Task<Unit> Handle(UpdateCredentialRequest request, CancellationToken cancellationToken)
        {

            var sqlHelper = new MysqlHelper(request.userCredentialToken);
            var container = request.passwordContainer;
            await sqlHelper.UpdateCredential(request.credentialID, container.userID, container.password);
            return Unit.Value;
        }
    }
}
