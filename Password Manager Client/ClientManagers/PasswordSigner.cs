using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace Password_Manager_Client
{

    class PasswordSigner : BaseSigner
    {
        List<PasswordContainer> credentials =
                           new List<PasswordContainer>();
        List<PasswordContainer> unsignedCredentials =
                            new List<PasswordContainer>();
        EncryptionManager signer;

        public void SerializeDataAndAssign(string data)
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
                unsignedCredentials.Add(new PasswordContainer(userID, password));
            }
        }

        public override void SecurePasswords(int option)
        {
            var tempCredentials = new List<PasswordContainer>();
            foreach (var credential in unsignedCredentials)
            {
                if (option == encrypt)
                {
                    Encrypt(tempCredentials, credential);
                }
                else if (option == decrypt)
                {
                    Decrypt(tempCredentials, credential);
                }
            }
            credentials = credentials.Concat(tempCredentials).ToList();
        }

        private void Encrypt(
            List<PasswordContainer> tempCredentials, PasswordContainer credential)
        {
            var userData = Encoding.UTF8.GetBytes(credential.userID);
            var passwordData = Encoding.UTF8.GetBytes(credential.password);
            userData = signer.Encrypt(userData);
            passwordData = signer.Encrypt(passwordData);

            tempCredentials.Add(new PasswordContainer(
                 Convert.ToBase64String(userData),
                 Convert.ToBase64String(passwordData)));
        }

        private void Decrypt(
            List<PasswordContainer> tempCredentials, PasswordContainer credential)
        {
            var userData = Convert.FromBase64String(credential.userID);
            var passwordData = Convert.FromBase64String(credential.password);

            userData = signer.Decrypt(userData);
            passwordData = signer.Decrypt(passwordData);

            tempCredentials.Add(new PasswordContainer(
                 Encoding.UTF8.GetString(userData),
                 Encoding.UTF8.GetString(passwordData)));
        }

        public void InsertCredential(PasswordContainer credential)
        {
            unsignedCredentials.Clear();
            unsignedCredentials.Add(credential);
        }
        public byte[] GetParsedPasswordData()
        {
            string container = JsonConvert.SerializeObject(
                credentials, Formatting.Indented);
            return Encoding.UTF8.GetBytes(container);
        }

        public void SetSigner(EncryptionManager signer)
        {
            this.signer = signer;
        }

        public override string ToString()
        {
            string outStr = "Passwords: \n";
            foreach (var credential in credentials)
            {
                outStr += credential.ToString() + "\n";
            }
            return outStr;
        }
    }

}
