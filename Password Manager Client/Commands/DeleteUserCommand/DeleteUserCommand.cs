using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Password_Manager_Client
{
    class DeleteUserCommand: ICommand
    {
        string path =  "/reset";

        public void Execute()
        {
            RequestSender.SendData(Encoding.UTF8.GetBytes("reset"), path);
        }
    }
}
