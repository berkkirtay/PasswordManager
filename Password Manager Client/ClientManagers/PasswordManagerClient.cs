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
    // This class needs to be refactored.
    class PasswordManagerClient : BaseSigner
    {
        private string urlPath;
        private bool menuLoop = true;
        private PasswordSigner passwordSigner = null;
        private EncryptionManager signer = new EncryptionManager(1024);

        public PasswordManagerClient()
        {
            InvokePasswordSigner();
            TestServer();
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
                              "4 to reset credentials database: ");

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
                    SendData(Encoding.UTF8.GetBytes("reset"));
                   // menuLoop = false;
                    break;
            }
           // credentials.Clear();
        }

        private void SendContainer()
        {
            urlPath = "/importNewContainer";
            var credentialsContainer = 
                RequestSender.ImportData(Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                                "resources\\passwords.json"));
            // Call encryptor class
            InvokePasswordSigner();
            // Serialize the local json data to send to the server.
            passwordSigner.SerializeDataAndAssign(
                Encoding.UTF8.GetString(credentialsContainer));
            // Encrypt the serialized credential data and send it.
            passwordSigner.SecurePasswords(encrypt);
            SendData(passwordSigner.GetParsedPasswordData());
        }

        private void SendSignleCredential()
        {
            urlPath = "/addCredential";
            Console.Write("Enter ID: ");
            string userID = Console.ReadLine();
            Console.Write("Enter password: ");
            string password = Console.ReadLine();
            InvokePasswordSigner();
            passwordSigner.InsertCredential(
                new PasswordContainer(userID, password));

            passwordSigner.SecurePasswords(encrypt);
            var data = passwordSigner.GetParsedPasswordData();
            SendData(data);
        }

        private void GetCredentials()
        {
            urlPath = "/getAllCredentials";
            var req = RequestSender.CreateGETRequest(
                "http://localhost:5000" + urlPath);
            var data = RequestSender.GetRespond(req);
            InvokePasswordSigner();
            passwordSigner.SerializeDataAndAssign(data);
            passwordSigner.SecurePasswords(decrypt);
            Console.WriteLine(passwordSigner.ToString());
        }

        private void InvokePasswordSigner()
        {
            passwordSigner = new PasswordSigner();
            passwordSigner.SetSigner(signer);
        }

        private void TestServer()
        {
            var req = RequestSender.CreateGETRequest(
           "http://localhost:5000" + urlPath);
            var data = RequestSender.GetRespond(req);
            if (data.Equals("AuthErr"))
            {
                SetAuthorization();
            }

        }
        private void SetAuthorization()
        {
            Console.WriteLine("Enter your key to access passwords: ");
            string pass = Console.ReadLine();
            SendData(Encoding.UTF8.GetBytes(pass));
        }

        private void SendData(byte[] parsedData)
        {
            var req = RequestSender.CreatePOSTRequest(
                "http://localhost:5000" + urlPath, parsedData);
            var respondStr = RequestSender.GetRespond(req);
            Console.WriteLine("Server respond: " + respondStr);
            
        }
    }

    public struct PasswordContainer
    {
        public string userID;
        public string password;

        public PasswordContainer(string userID, string password)
        {
            this.userID = userID;
            this.password = password;
        }

        public override string ToString()
        {
            return userID + " " + password;
        }
    }

}
