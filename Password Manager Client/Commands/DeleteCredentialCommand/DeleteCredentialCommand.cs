using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Password_Manager_Client {
    class DeleteCredentialCommand : ICommand
    {

        private string path = "/deletecredential";
        private ICommand command = new GetCredentialsCommand();

        public void Execute()
        {
            // Get a list of credentials:
            command.Execute();

            // Delete from the list:
            Console.WriteLine("Please enter the index of the credential that you want to delete:");
            int credentialID = Convert.ToInt32(Console.ReadLine());

            var list = new List<PasswordContainer>();
            list.Add(new PasswordContainer(credentialID, "", ""));
            string container = JsonConvert.SerializeObject(
                    list, Formatting.Indented);
            RequestSender.SendData(Encoding.UTF8.GetBytes(container), path);
        }
    }

}
