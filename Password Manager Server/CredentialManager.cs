using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace Password_Manager_Server
{
    class CredentialManager
    {
        private List<PasswordContainer> credentials =
                    new List<PasswordContainer>();

        public void SetPasswordsData(HttpListenerContext context)
        {
            var req = context.Request;
            StreamReader reader = new StreamReader(req.InputStream);
            SerializeDataAndAssign(reader.ReadToEnd());
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
        public void SaveContainerToLocal()
        {
            string container = JsonConvert.SerializeObject(
                credentials, Formatting.Indented);
            var writer = new StreamWriter("credentials.json");
            writer.Write(container);
            writer.Close();
        }

        public List<PasswordContainer> GetPasswordContainer()
        {
            return credentials;
        }

        public void ResetCredentials()
        {
            credentials.Clear();
        }

        public override string ToString()
        {
            string outStr = "Credentials: \n";
            foreach (var credential in credentials)
            {
                outStr += credential.ToString() + "\n";
            }
            return outStr;
        }


    }
}
