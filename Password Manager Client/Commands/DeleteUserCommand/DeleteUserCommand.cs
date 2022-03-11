using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Password_Manager_Client
{
    class DeleteUserCommand: ICommand
    {
        string path =  "/reset";

        public void Execute()
        {
            var list = new List<PasswordContainer>();
           // list.Add(new PasswordContainer(credentialID, "", ""));
            string container = JsonConvert.SerializeObject(
                    list, Formatting.Indented);
            RequestSender.SendData(Encoding.UTF8.GetBytes(container), path);
        }
    }
}
