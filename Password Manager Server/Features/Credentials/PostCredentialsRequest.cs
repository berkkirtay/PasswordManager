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
        public PasswordContainerModel passwordContainerModel { get; set; }
    }

    internal class PostCredentialsRequestHandler : IRequestHandler<PostCredentialsRequest, Unit>
    {
        public async Task<Unit> Handle(PostCredentialsRequest request, CancellationToken cancellationToken)
        {
            var model = request.passwordContainerModel;
            var sqlHelper = new MysqlHelper(model.userKeyToken);
            foreach (var credential in model.passwordContainer)
            {
                await sqlHelper.InsertPassword(credential.userID, credential.password);
            }
            return Unit.Value;
        }
    }
}
