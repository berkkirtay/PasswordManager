using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using System.Threading;

namespace Password_Manager_Server
{
    public class DeleteUserRequest : IRequest
    {
        public string userCredentialToken { get; set; }
    }

    internal class DeleteUserRequestHandler : IRequestHandler<DeleteUserRequest, Unit>
    {
        public async Task<Unit> Handle(DeleteUserRequest request, CancellationToken cancellationToken)
        {
            var sqlHelper = new MysqlHelper(request.userCredentialToken);
            await sqlHelper.DeleteUser();
            return Unit.Value;
        }
    }
}
