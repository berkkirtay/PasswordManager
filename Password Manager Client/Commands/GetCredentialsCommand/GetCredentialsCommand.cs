using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Password_Manager_Client
{
    class GetCredentialsCommand : ICommand
    {
        private string path = "/getallcredentials";
        public void Execute()
        {
            var req = RequestSender.CreateGETRequest(path);
            var data = RequestSender.GetRespond(req);

            var credentials = JsonConvert.DeserializeObject<List<PasswordContainer>>(data);
            var decryptedData = Encoding.UTF8.GetString(PasswordEncryptor.SecurePasswords(credentials, crypto.decrypt));
            var decryptedCredentials = JsonConvert.DeserializeObject<List<PasswordContainer>>(decryptedData);
            foreach (var credential in decryptedCredentials)
            {
                Console.WriteLine(credential.ToString());
            }
        }
    }
}
