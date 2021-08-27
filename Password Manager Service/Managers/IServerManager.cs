using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Password_Manager_Server
{
    interface IServerManager
    {
        void Invoke(HttpListenerContext context);
    }
}
