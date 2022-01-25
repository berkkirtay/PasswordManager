using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using System.Threading;

namespace Password_Manager_Server
{
    public class UpdateCredentialRequest : IRequest
    {
        public PasswordContainerModel passwordContainerModel { get; set; }
    }

    internal class UpdateCredentialRequestHandler : IRequestHandler<UpdateCredentialRequest, Unit>
    {
        public async Task<Unit> Handle(UpdateCredentialRequest request, CancellationToken cancellationToken)
        {
            var model = request.passwordContainerModel;
            var sqlHelper = new MysqlHelper(model.userKeyToken);
            foreach (var credential in model.passwordContainer)
            {
                await sqlHelper.UpdateCredential(credential.credentialID, credential.userID, credential.password);
            }      
            return Unit.Value;
        }
    }
}
