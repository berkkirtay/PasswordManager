using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Password_Manager_Client
{
    using System.Security.Cryptography;
    class EncryptionManager
    {
        private RSAParameters public_key { get; set; }
        private RSAParameters private_key { get; set; }

        private RSACryptoServiceProvider rsa;

        public EncryptionManager(int keySize)
        {
            rsa = new RSACryptoServiceProvider(keySize);
        }

        public byte[] Encrypt(byte[] data)
        {
            return rsa.Encrypt(data, true);
        }

        public byte[] Decrypt(byte[] data)
        {
            return rsa.Decrypt(data, true);
        }

        public RSAParameters getPrivateKey()
        {
            return private_key;
        }

        private void KeyGenerator()
        {
            public_key = rsa.ExportParameters(false);
            private_key = rsa.ExportParameters(true);
        }
    }
}
