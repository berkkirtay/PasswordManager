using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net;
using System.IO;

namespace Password_Manager_Client
{

    class PasswordManagerClient
    {

        private ICommand command = new AuthorizationCommand();

        public PasswordManagerClient()
        {
            while (true)
            {
                Menu();    
            }
        }

        private void Menu()
        {
            // Invoking command pattern:
            command.Execute();

            Console.WriteLine("1 to import a credential file,\n" +
                              "2 to insert a new credential,\n" +
                              "3 to get all the credentials,\n" +
                              "4 to update a credential,\n" +
                              "5 to delete a credential,\n" +
                              "6 to reset credentials of current user: ");

            char choice  = Console.ReadLine()[0];
            Console.Clear();
            switch (choice){
                case '1':
                    command = new SendCredentialContainerCommand();
                    break;
                case '2':
                    command = new SendCredentialCommand();
                    break;
                case '3':
                    command = new GetCredentialsCommand();
                    break;
                case '4':
                    command = new UpdateCredentialCommand();
                    break;
                case '5':
                    command = new DeleteCredentialCommand();
                    break;
                case '6':
                    command = new DeleteUserCommand();
                    break;
            }
        }
    }
}
