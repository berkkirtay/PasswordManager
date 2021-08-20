using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Password_Manager_Client
{
    abstract class BaseSigner
    {
        public const int encrypt = 1;
        public const int decrypt = 2;

        virtual public void SecurePasswords(int option)
        {

        }
    }
}
