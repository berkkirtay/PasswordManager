using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Password_Manager_Client
{
    class AuthorizationCommand : ICommand
    {
        public void Execute()
        {
            Console.WriteLine("Enter your key to access passwords: ");
            string pass = Console.ReadLine();
            RequestSender.SetAuthorization(pass);

            var token = EncryptionManager.HashData(pass);
            var authKey = "Bearer " + Convert.ToBase64String(token);
            RequestSender.SetAuthorization(authKey);
            Console.WriteLine("Your authorization token: " + authKey);
        }
    }
}
