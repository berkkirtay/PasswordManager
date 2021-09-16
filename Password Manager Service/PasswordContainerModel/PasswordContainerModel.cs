using System.Collections.Generic;

namespace PasswordManagerService
{
    class PasswordContainerModel
    {
        public string userKeyToken;
        public List<PasswordContainer> passwordContainer;

        public PasswordContainerModel(string userKeyToken, List<PasswordContainer> passwordContainer)
        {
            this.userKeyToken = userKeyToken;
            this.passwordContainer = passwordContainer;
        }

        public override string ToString()
        {
            string outStr = "Credentials of the user: \n";
            foreach (var credential in passwordContainer)
            {
                outStr += credential.ToString() + "\n";
            }
            outStr = "\n User Token: " + userKeyToken;
            return outStr;
        }
    }

    public struct PasswordContainer
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
            return userID + " " + password;
        }
    }
}
