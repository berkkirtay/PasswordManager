using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Password_Manager_Client
{
    class BaseCredentialCommand
    {
        private string path =  "/addcredential";

        public void SecureAndSendCredentials(List<PasswordContainer> credentialList, string path = null)
        {
            var encryptedData = PasswordEncryptor.SecurePasswords(credentialList, crypto.encrypt);
            RequestSender.SendData(encryptedData, path);
        }
    }
}
