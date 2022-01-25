using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using System.Threading;

namespace Password_Manager_Server
{
    public class DeleteCredentialRequest : IRequest
    {
        public PasswordContainerModel passwordContainerModel { get; set; }
    }

    internal class DeleteCredentialRequestHandler : IRequestHandler<DeleteCredentialRequest, Unit>
    {
        public async Task<Unit> Handle(DeleteCredentialRequest request, CancellationToken cancellationToken)
        {
            var model = request.passwordContainerModel;
            var sqlHelper = new MysqlHelper(model.userKeyToken);
            foreach (var credential in model.passwordContainer)
            {
                await sqlHelper.DeleteCredential(credential.credentialID);
            }
            return Unit.Value;
        }
    }
}
