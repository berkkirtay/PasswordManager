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
        private string urlPath;
        private bool menuLoop = true;

        public PasswordManagerClient()
        {
            SetAuthorization();

            while (menuLoop)
            {
                Menu();    
            }
        }

        private void Menu()
        {
            Console.WriteLine("1 to import a credential file,\n" +
                              "2 to insert a new credential,\n" +
                              "3 to get all the credentials,\n" + 
                              "4 to reset credentials of current session: ");

            char choice  = Console.ReadLine()[0];
            Console.Clear();
            switch (choice){
                case '1':
                    SendContainer();
                    break;
                case '2':
                    SendSignleCredential();
                    break;
                case '3':
                    GetCredentials();
                    break;
                case '4':
                    urlPath = "/reset";
                    SendData(
                        Encoding.UTF8.GetBytes("reset"));
                    break;
            }
        }

        private void SendContainer()
        {
            var data = ImportData(
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"resources\\passwords.json"));
            // Serialize the local json data.
            var credentials = JsonConvert.DeserializeObject<List<PasswordContainer>>(data);
            // Encrypt the serialized credential data and send it.
            SecureAndSendCredentials(credentials);
        }

        private void SendSignleCredential()
        {
            Console.Write("Enter ID: ");
            string userID = Console.ReadLine();
            Console.Write("Enter password: ");
            string password = Console.ReadLine();

            var list = new List<PasswordContainer>();
            list.Add(new PasswordContainer(userID, password));

            SecureAndSendCredentials(list);
        }

        private void SecureAndSendCredentials(List<PasswordContainer> credentialList)
        {
            urlPath = "/addcredential";
            var encryptedData = PasswordEncryptor.SecurePasswords(credentialList, crypto.encrypt);
            SendData(encryptedData);
        }

        private void GetCredentials()
        {
            urlPath = "/getallcredentials";
            var req = RequestSender.CreateGETRequest(
                "http://localhost:8000" + urlPath);      
            var data = RequestSender.GetRespond(req);

            var credentials = JsonConvert.DeserializeObject<List<PasswordContainer>>(data);
            var decryptedData = Encoding.UTF8.GetString(PasswordEncryptor.SecurePasswords(credentials, crypto.decrypt));
            var decryptedCredentials = JsonConvert.DeserializeObject<List<PasswordContainer>>(decryptedData);
            foreach (var credential in decryptedCredentials)
            {
                Console.WriteLine(credential.ToString());
            }
        }

        private void SetAuthorization()
        {
            Console.WriteLine("Enter your key to access passwords: ");
            string pass = Console.ReadLine();
            RequestSender.SetAuthorization(pass);
        }

        static public string ImportData(string dataAddr)
        {
            StreamReader reader = new StreamReader(dataAddr);
            return reader.ReadToEnd();
        }

        private void SendData(byte[] parsedData)
        {
            var req = RequestSender.CreatePOSTRequest(
                "http://localhost:8000" + urlPath, parsedData);
            var respondStr = RequestSender.GetRespond(req);
            Console.WriteLine("Server respond: " + respondStr);        
        }
    }
}
