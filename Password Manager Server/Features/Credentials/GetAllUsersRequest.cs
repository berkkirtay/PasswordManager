using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using System.Threading;

namespace Password_Manager_Server
{
    public class GetAllUsersRequest : IRequest<List<string>>
    {
        public string authToken { get; set; }
    }

    internal class GetAllUsersRequestHandler : IRequestHandler<GetAllUsersRequest, List<string>>
    {
        public async Task<List<string>> Handle(GetAllUsersRequest request, CancellationToken cancellationToken)
        {
            if(request.authToken != "admin")
            {
                throw new Exception("AuthErr");
            }
            var sqlHelper = new MysqlHelper(request.authToken);
            return await sqlHelper.GetAllUsers();
        }
    }
}
