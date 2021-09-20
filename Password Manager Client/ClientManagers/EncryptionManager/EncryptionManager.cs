using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

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
            try
            {
                ReadKeys();
            }
            catch(IOException ex)
            {          
                WriteKeys();
            }
        }

        private void ReadKeys()
        {
            string public_key_str = File.ReadAllText("RSA_public_key.xml");
            string private_key_str = File.ReadAllText("RSA_private_key.xml");

            var xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));

            var reader = new System.IO.StringReader(public_key_str);
            public_key = (RSAParameters)xmlSerializer.Deserialize(reader);
            reader = new System.IO.StringReader(private_key_str);
            private_key = (RSAParameters)xmlSerializer.Deserialize(reader);
        }

        private void WriteKeys()
        {
            KeyGenerator();
            var stringWriter = new System.IO.StringWriter();
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));

            serializer.Serialize(stringWriter, public_key);
            string public_key_str = stringWriter.ToString();

            stringWriter = new System.IO.StringWriter();
            serializer.Serialize(stringWriter, private_key);
            string private_key_str = stringWriter.ToString();

            File.WriteAllText("RSA_public_key.xml", public_key_str);
            File.WriteAllText("RSA_private_key.xml", private_key_str); 
        }

        private void KeyGenerator()
        {
            public_key = rsa.ExportParameters(false);
            private_key = rsa.ExportParameters(true);
        }

        public byte[] Encrypt(byte[] data)
        {
            rsa.ImportParameters(public_key);
            return rsa.Encrypt(data, false);
        }

        public byte[] Decrypt(byte[] data)
        {
            rsa.ImportParameters(private_key);
            return rsa.Decrypt(data, false);
        }

        static public byte[] HashData(string key)
        {
            var sha256 = SHA256.Create();
            var hashedData = sha256.ComputeHash(Encoding.UTF8.GetBytes(key));
            return hashedData;
        }
    }
}
