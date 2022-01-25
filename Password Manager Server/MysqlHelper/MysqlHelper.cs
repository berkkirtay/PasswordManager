using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace Password_Manager_Server
{
    class MysqlHelper
    {
        private MySqlConnection mySqlConnection = null;
        private readonly string userKeyToken;

        /*
         * When we instantiate an object, user
         * can only do transactions on the data
         * he owns. Therefore we encapsulate
         * other users data from a particular user.
         */

        public MysqlHelper(string userKeyToken)
        {
            this.userKeyToken = userKeyToken;
            mySqlConnection = new MySqlConnection
            {
                ConnectionString = "server=localhost;user id=root;password=mypassword;database=passwordmanagerdb"
            };
            InitializeDB().Wait();
        }

        private async Task InitializeDB()
        {
            string result = await CheckIfTableIsAvailable();

            if (result == "False")
            {
                await CreateNewTables();
                Console.WriteLine("Database tables are created.");
            }
            await InsertNewUserSession();
        }

        private async Task<string> CheckIfTableIsAvailable()
        {
            OpenConnection();
            string query = $"SHOW TABLES FROM passwordmanagerdb LIKE 'usersTable';";
            MySqlCommand cmd = new MySqlCommand(query, mySqlConnection);
            var reader = await cmd.ExecuteReaderAsync();

            string result = reader.Read() + "";

            CloseConnection();
            return result;
        }

        private async Task CreateNewTables()
        {
            OpenConnection();
            string query = "CREATE TABLE usersTable(" +
                           "userKeyToken VARCHAR(50)," +
                           "PRIMARY KEY(userKeyToken));";
                           
            MySqlCommand cmd = new MySqlCommand(query, mySqlConnection);
            await cmd.ExecuteNonQueryAsync();

            query = "CREATE TABLE credentialsTable(" +
                    "credentialID int NOT NULL AUTO_INCREMENT," +
                    "userName VARCHAR(500)," +
                    "password VARCHAR(500)," +
                    "userKeyToken VARCHAR(50)," +
                    "PRIMARY KEY(credentialID)," +
                    "FOREIGN KEY(userKeyToken) REFERENCES usersTable(userKeyToken) " +
                    "ON DELETE CASCADE ON UPDATE CASCADE);";

            cmd = new MySqlCommand(query, mySqlConnection);
            await cmd.ExecuteNonQueryAsync();

            CloseConnection();
        }

        private async Task InsertNewUserSession()
        {
            OpenConnection();

            string query = "INSERT IGNORE INTO usersTable " +
                            $"VALUES(\"{userKeyToken}\");";
            MySqlCommand cmd = new MySqlCommand(query, mySqlConnection);
            await cmd.ExecuteNonQueryAsync();

            CloseConnection();
        }


        private void OpenConnection()
        {
            try
            {
                mySqlConnection.Open();
                if (mySqlConnection.State != System.Data.ConnectionState.Open)
                {
                    throw new Exception("Mysql connection attempt is failed!");
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private void CloseConnection()
        {
            try
            {
                mySqlConnection.Close();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public async Task<List<string>> GetAllUsers()
        {
            OpenConnection();

            string query = $"SELECT * FROM usersTable";
            MySqlCommand cmd = new MySqlCommand(query, mySqlConnection);
            var reader = await cmd.ExecuteReaderAsync();

            var usersList = new List<string>();
            while (reader.Read())
            {
                usersList.Add(reader["userKeyToken"] + "");
            }
            CloseConnection();

            return usersList;
        }

        public async Task<List<PasswordContainer>> GetUserCredentials()
        {
            OpenConnection();

            string query = $"SELECT * FROM credentialsTable " +
                           $"WHERE userKeyToken = \"{userKeyToken}\";";
            MySqlCommand cmd = new MySqlCommand(query, mySqlConnection);
            var reader = await cmd.ExecuteReaderAsync();

            var userCredentialsList = new List<PasswordContainer>();
            while (reader.Read())
            {
                userCredentialsList.Add(new PasswordContainer(
                    reader["userName"] + "", reader["password"] + "", reader.GetInt32(reader.GetOrdinal("credentialID"))));
            }
            CloseConnection();

            return userCredentialsList;
        }

        public async Task InsertPassword(string userName, string password)
        {
            OpenConnection();

            string query = $"INSERT INTO credentialsTable " +
                           $"VALUES(NULL, \"{userName}\", \"{password}\", \"{userKeyToken}\");";
            MySqlCommand cmd = new MySqlCommand(query, mySqlConnection);
            await cmd.ExecuteNonQueryAsync();

            CloseConnection();
        }

        public async Task UpdateCredential(int credentialID, string userName, string password)
        {
            OpenConnection();

            string query = "UPDATE credentialsTable " +
                           $"Set userName=\"{userName}\", password={password}\" " +
                           $"WHERE credentialID=\"{credentialID}\" " +
                           $"and userKeyToken=\"{userKeyToken}\";";
            MySqlCommand cmd = new MySqlCommand(query, mySqlConnection);
            await cmd.ExecuteNonQueryAsync();

            CloseConnection();
        }

        public async Task DeleteCredential(int credentialID)
        {
            OpenConnection();

            string query = $"DELETE FROM credentialsTable " +
                           $"WHERE credentialID=\"{credentialID}\" " +
                           $"and userKeyToken=\"{userKeyToken}\";";
            MySqlCommand cmd = new MySqlCommand(query, mySqlConnection);
            await cmd.ExecuteNonQueryAsync();

            CloseConnection();
        }

        public async Task DeleteUser()
        {
            OpenConnection();

            string query = $"DELETE FROM usersTable " +
                           $"WHERE userKeyToken=\"{userKeyToken}\"";
            MySqlCommand cmd = new MySqlCommand(query, mySqlConnection);
            await cmd.ExecuteNonQueryAsync();

            CloseConnection();
        }
    }
}
