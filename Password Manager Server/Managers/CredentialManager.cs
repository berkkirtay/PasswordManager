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
        private string userCredentialToken;
        private List<PasswordContainer> credentials =
                    new List<PasswordContainer>();
        private List<PasswordContainerModel> userCredentials =
                    new List<PasswordContainerModel>();

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

        public List<PasswordContainer> GetPasswordContainer()
        {
            List<PasswordContainer> fetchedContainer = null;
            userCredentials.ForEach(user =>
            {
                if(user.userKeyToken == userCredentialToken)
                {
                    fetchedContainer = user.passwordContainer;
                }
            });
            return fetchedContainer;
        }

        public void ResetCredentials()
        {
            credentials.Clear();
        }

        public void CreateUserCredentialSession(string token)
        {
            userCredentialToken = token;
            foreach(PasswordContainerModel user in userCredentials)
            {
                if(user.userKeyToken == userCredentialToken)
                {
                    credentials = user.passwordContainer;
                    return;
                }
            }
            userCredentials.Add(
                new PasswordContainerModel(token, new List<PasswordContainer>()));
        }

        public void SetUserData()
        {
            foreach (PasswordContainerModel user in userCredentials)
            {
                if (user.userKeyToken == userCredentialToken)
                {
                    List<PasswordContainer> userCredentials = new List<PasswordContainer>();
                    credentials.ForEach(credential =>
                    {
                        userCredentials.Add(credential);
                    });
                    user.passwordContainer = userCredentials;
                    credentials.Clear();
                    SaveContainerToLocal();
                    return;
                }
            }
        }

        public void SaveContainerToLocal()
        {
            string container = JsonConvert.SerializeObject(
                userCredentials, Formatting.Indented);
            var writer = new StreamWriter("PasswordContainerSave.json");
            writer.Write(container);
            writer.Close();
        }


    }
}
