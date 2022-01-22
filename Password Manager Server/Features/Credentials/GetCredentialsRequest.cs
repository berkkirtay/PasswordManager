using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using System.Threading;

namespace Password_Manager_Server
{
    public class GetCredentialsRequest : IRequest<List<PasswordContainer>>
    {
        public string userCredentialToken { get; set; }
    }

    internal class GetCredentialsRequestHandler : IRequestHandler<GetCredentialsRequest, List<PasswordContainer>>
    {
        public async Task<List<PasswordContainer>> Handle(GetCredentialsRequest request, CancellationToken cancellationToken)
        {
            var sqlHelper = new MysqlHelper(request.userCredentialToken);
            return await sqlHelper.GetUserCredentials();
        }
    }
}
