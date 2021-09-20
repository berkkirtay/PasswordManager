using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Password_Manager_Client
{
    class PasswordContainer
    {
        public string userID;
        public string password;

        public PasswordContainer(string userID, string password)
        {
            this.userID = userID;
            this.password = password;
        }

        public override string ToString()
        {
            return userID + " - " + password;
        }
    }
}
