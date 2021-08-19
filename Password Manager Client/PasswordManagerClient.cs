using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net;
using System.IO;

namespace Password_Manager
{
    // This class needs to be refactored.
    class PasswordManagerClient
    {
        private List<PasswordContainer> credentials =
                                new List<PasswordContainer>();
        private string urlPath;
        private bool menuLoop = true;
        private PasswordSigner passwordSigner = null;

        public PasswordManagerClient()
        {
            InvokePasswordSigner();
            while (menuLoop)
            {
                Menu();    
            }
        }

        private void Menu()
        {
            Console.WriteLine("1 for importing new credentials,\n" +
                              "2 for inserting a new credential,\n" +
                              "3 for getting all the credentials,\n" + 
                              "4 for exit: ");

            char choice  = Console.ReadLine()[0];
            switch (choice){
                case '1':
                    urlPath = "/importNewContainer";
                    SendContainer();
                    break;
                case '2':
                    urlPath = "/addCredential";
                    SendSignleCredential();
                    break;
                case '3':
                    urlPath = "/getAllCredentials";
                    GetCredentials();
                    break;
                case '4':
                    menuLoop = false;
                    break;
            }
        }

        private void SendContainer()
        {
            var credentialsContainer = 
                RequestSender.ImportData(Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                                "resources\\passwords.json"));

            InvokePasswordSigner();
            SerializeDataAndAssign(
                Encoding.UTF8.GetString(credentialsContainer));
            passwordSigner.SetCredentials(credentials);
            passwordSigner.EncryptPasswords();
            SendData();
        }

        private void SendSignleCredential()
        {
            Console.Write("Enter ID: ");
            string userID = Console.ReadLine();
            Console.Write("Enter password: ");
            string password = Console.ReadLine();

            credentials.Add(new PasswordContainer(userID, password));
            passwordSigner.SetCredentials(credentials);
            passwordSigner.EncryptPasswords();
            SendData();
            Console.WriteLine("A new credential is generated.\n");
        }

        private void GetCredentials()
        {
            var req = RequestSender.CreateGETRequest(
                "http://localhost:5000" + urlPath);
            var data = RequestSender.GetRespond(req);
            SerializeDataAndAssign(data);
            passwordSigner.DecryptPasswords();
            credentials = passwordSigner.GetCredentials();
            Console.WriteLine(ToString());
        }

        private void InvokePasswordSigner()
        {
            passwordSigner = new PasswordSigner();
            passwordSigner.SetCredentials(credentials);
            
        }

        private void SerializeDataAndAssign(string data)
        {
            JsonTextReader jsonReader = new JsonTextReader(
                new StringReader(data))
            {
                SupportMultipleContent = true
            };
            var serializer = new JsonSerializer();

            dynamic credentialContainer =
                serializer.Deserialize<List<PasswordContainer>>(jsonReader);

            foreach (var credential in credentialContainer)
            {
                string userID = credential.userID;
                string password = credential.password;
                credentials.Add(new PasswordContainer(userID, password));
            }
        }

        private void SendData()
        {
            var parsedData = passwordSigner.GetParsedPasswordData();
            var req = RequestSender.CreatePOSTRequest(
                "http://localhost:5000" + urlPath, parsedData);
            RequestSender.GetRespond(req);
        }

        public override string ToString()
        {
            string outStr = "Passwords: \n";
            foreach(var credential in credentials)
            {
                outStr += credential.ToString() + "\n";
            }
            return outStr;
        }
    }

    class PasswordSigner
    {
        List<PasswordContainer> credentials;
        SignatureMaker encryptor = new SignatureMaker(2048);

        public void EncryptPasswords()
        {
            var encryptedCredentials = new List<PasswordContainer>();
            foreach (var credential in credentials)
            {
                var userData = Encoding.UTF8.GetBytes(credential.userID);
                var passwordData = Encoding.UTF8.GetBytes(credential.password);

                userData = encryptor.Encrypt(userData);
                passwordData = encryptor.Encrypt(passwordData);

                encryptedCredentials.Add(new PasswordContainer(
                     Convert.ToBase64String(userData),
                    Convert.ToBase64String(passwordData)));
            }
            credentials = encryptedCredentials;
        }

        public void DecryptPasswords()
        {
            var decryptedCredentials = new List<PasswordContainer>();
            foreach (var credential in credentials)
            {
                var userData = Convert.FromBase64String(credential.userID);
                var passwordData = Convert.FromBase64String(credential.password);

                userData = encryptor.Decrypt(userData);
                passwordData = encryptor.Decrypt(passwordData);

                decryptedCredentials.Add(new PasswordContainer(
                     Encoding.UTF8.GetString(userData),
                     Encoding.UTF8.GetString(passwordData)));
            }
            credentials = decryptedCredentials;
        }

        public byte[] GetParsedPasswordData()
        {
            string container = JsonConvert.SerializeObject(
                credentials, Formatting.Indented);
            return Encoding.ASCII.GetBytes(container);
        }

        public List<PasswordContainer> GetCredentials()
        {
            return credentials;
        }

        public void SetCredentials(List<PasswordContainer>  credentials)
        {
            this.credentials = credentials;
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
