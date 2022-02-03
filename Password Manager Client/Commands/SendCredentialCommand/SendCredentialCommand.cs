using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Password_Manager_Client
{
    class SendCredentialCommand : BaseCredentialCommand, ICommand
    {
        public void Execute()
        {
            Console.Write("Enter fields userID and password consecutively: ");
            string input = Console.ReadLine();
            var tokens = input.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            var list = new List<PasswordContainer>();
            list.Add(new PasswordContainer(0, tokens[0], tokens[1]));
            SecureAndSendCredentials(list);
        }
    }
}
