using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Password_Manager_Client
{
    class PasswordContainer
    {
        public int credentialID;
        public string userID;
        public string password;

        public PasswordContainer(int credentialID, string userID, string password)
        {
            this.credentialID = credentialID;
            this.userID = userID;
            this.password = password;
        }

        public override string ToString()
        {
            return credentialID + ". Credential: " + userID + " - " + password;
        }
    }
}
