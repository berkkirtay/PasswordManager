using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Password_Manager_Client
{
    enum crypto{
        encrypt,
        decrypt
    }

    class PasswordEncryptor
    {
        private static EncryptionManager encryptor = new EncryptionManager(2048);

        public static byte[] SecurePasswords
            (List<PasswordContainer> credentials, crypto option)
        {
            try
            {
                var signedCredentials = new List<PasswordContainer>();
                foreach (var credential in credentials)
                {
                    if (option == crypto.encrypt)
                    {
                        Encrypt(signedCredentials, credential);
                    }
                    else if (option == crypto.decrypt)
                    {
                        Decrypt(signedCredentials, credential);
                    }
                }
                return GetParsedPasswordData(signedCredentials);
            }
            catch (System.Security.Cryptography.CryptographicException)
            {
                Console.WriteLine(
                    "CryptographicException: Attemp to decrypt with an invalid private key.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Cryptography error: " + ex.ToString());
            }
            return null;
        }

        private static void Encrypt(
            List<PasswordContainer> tempCredentials, PasswordContainer credential)
        {
            var userData = Encoding.UTF8.GetBytes(credential.userID);
            var passwordData = Encoding.UTF8.GetBytes(credential.password);
            userData = encryptor.Encrypt(userData);
            passwordData = encryptor.Encrypt(passwordData);

            tempCredentials.Add(new PasswordContainer(
                 credential.credentialID,
                 Convert.ToBase64String(userData),
                 Convert.ToBase64String(passwordData)));
        }

        private static void Decrypt(
            List<PasswordContainer> tempCredentials, PasswordContainer credential)
        {
            var userData = Convert.FromBase64String(credential.userID);
            var passwordData = Convert.FromBase64String(credential.password);

            userData = encryptor.Decrypt(userData);
            passwordData = encryptor.Decrypt(passwordData);

            tempCredentials.Add(new PasswordContainer(
                 credential.credentialID,
                 Encoding.UTF8.GetString(userData),
                 Encoding.UTF8.GetString(passwordData)));
        }

        private static byte[] GetParsedPasswordData(List<PasswordContainer> credentials)
        {
            string container = JsonConvert.SerializeObject(
                credentials, Formatting.Indented);
            return Encoding.UTF8.GetBytes(container);
        }
    }

}
