using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Password_Manager_Client
{
    class UpdateCredentialCommand : BaseCredentialCommand, ICommand
    {
        private string path = "/updatecredential";
        private ICommand command = new GetCredentialsCommand();

        public void Execute()
        {
            // Get a list of credentials:
            command.Execute();
            Console.WriteLine("Please enter the index of the credential that you want to change:");
            int credentialID = Convert.ToInt32(Console.ReadLine());

            Console.Write("Enter new fields userID and password consecutively: ");
            string input = Console.ReadLine();
            var tokens = input.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            var list = new List<PasswordContainer>();
            list.Add(new PasswordContainer(credentialID, tokens[0], tokens[1]));
            SecureAndSendCredentials(list, path);
        }
    }
}
