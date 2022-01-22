using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using System.Threading;

namespace Password_Manager_Server.Features.Credentials
{
    public class DeleteCredentialRequest : IRequest
    {
        public string userCredentialToken { get; set; }
        public int credentialID { get; set; }
    }

    internal class DeleteCredentialRequestHandler : IRequestHandler<DeleteCredentialRequest, Unit>
    {
        public async Task<Unit> Handle(DeleteCredentialRequest request, CancellationToken cancellationToken)
        {
            var sqlHelper = new MysqlHelper(request.userCredentialToken);
            await sqlHelper.DeleteCredential(request.credentialID);
            return Unit.Value;
        }
    }
}
