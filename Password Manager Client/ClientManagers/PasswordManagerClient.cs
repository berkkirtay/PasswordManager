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
        private EncryptionManager signer = new EncryptionManager(2048);

        public PasswordManagerClient()
        {
            SetAuthorization();
            InvokePasswordSigner();
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
                    SendData(
                        Encoding.UTF8.GetBytes("reset"));
                    break;
            }
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
            AttemptCryptography();
            Console.WriteLine(passwordSigner.ToString());
        }

        private void InvokePasswordSigner()
        {
            passwordSigner = new PasswordSigner();
            passwordSigner.SetSigner(signer);
        }

        private void SetAuthorization()
        {
            Console.WriteLine("Enter your key to access passwords: ");
            string pass = Console.ReadLine();
            RequestSender.SetAuthorization(pass);
        }

        private void AttemptCryptography()
        {
            try
            {
                passwordSigner.SecurePasswords(decrypt);
            }
            catch (System.Security.Cryptography.CryptographicException)
            {
                Console.WriteLine(
                    "CryptographicException: Attemp to decrypt with an invalid private key.");
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Cryptography error: " + ex.ToString());
                return;
            }
        }

        private void SendData(byte[] parsedData)
        {
            var req = RequestSender.CreatePOSTRequest(
                "http://localhost:5000" + urlPath, parsedData);
            var respondStr = RequestSender.GetRespond(req);
            Console.WriteLine("Server respond: " + respondStr);        
        }
    }
}
