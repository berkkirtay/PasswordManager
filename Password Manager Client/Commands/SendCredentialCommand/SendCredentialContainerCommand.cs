using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace Password_Manager_Client
{
    class SendCredentialContainerCommand : BaseCredentialCommand, ICommand
    {
        public void Execute()
        {
            var data = ImportData(
               Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "resources\\passwords.json"));
            // Serialize the local json data.
            var credentials = JsonConvert.DeserializeObject<List<PasswordContainer>>(data);
            // Encrypt the serialized credential data and send it.
            SecureAndSendCredentials(credentials);
        }

        private string ImportData(string dataAddr)
        {
            StreamReader reader = new StreamReader(dataAddr);
            return reader.ReadToEnd();
        }
    }
}
