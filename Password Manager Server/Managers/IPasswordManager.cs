using System.Threading.Tasks;
using System.Net;

namespace Password_Manager_Server
{
    interface IPasswordManager
    {
        Task Invoke(HttpListenerContext context);
    }
}
