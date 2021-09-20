using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace Password_Manager_Server
{
    class CredentialsDB
    {
        private MySqlConnection mySqlConnection = null;
        private string userKeyToken;

        public CredentialsDB(string userKeyToken)
        {
            this.userKeyToken = userKeyToken;
            mySqlConnection = new MySqlConnection();
            mySqlConnection.ConnectionString = "server=localhost;user id=root;password=mypassword;database=passwordmanagerdb";
            InitializeDB();
        }

        private void InitializeDB()
        {
            string result = CheckIfTableIsAvailable();

            if (result == "False")
            {
                CreateNewTables();
                Console.WriteLine("Database tables are created.");
            }
            InsertNewUserSession();
        }

        private string CheckIfTableIsAvailable()
        {
            OpenConnection();
            string query = $"SHOW TABLES FROM passwordmanagerdb LIKE 'usersTable';";
            MySqlCommand cmd = new MySqlCommand(query, mySqlConnection);
            var reader = cmd.ExecuteReader();

            string result = reader.Read() + "";

            CloseConnection();
            return result;
        }

        private void CreateNewTables()
        {
            OpenConnection();
            string query = "CREATE TABLE usersTable(userKeyToken VARCHAR(50) UNIQUE);";


            MySqlCommand cmd = new MySqlCommand(query, mySqlConnection);
            cmd.ExecuteNonQueryAsync();

            query = "CREATE TABLE credentialsTable(userID VARCHAR(400), " +
                "password VARCHAR(400), " +
                "userKeyToken VARCHAR(50), " +
                "FOREIGN KEY(userKeyToken) REFERENCES usersTable(userKeyToken) ON DELETE SET NULL);";
            cmd = new MySqlCommand(query, mySqlConnection);
            cmd.ExecuteNonQueryAsync();

            CloseConnection();
        }

        private void InsertNewUserSession()
        {
            OpenConnection();

            string query = $"INSERT INTO usersTable VALUES(\"{userKeyToken}\");";
            MySqlCommand cmd = new MySqlCommand(query, mySqlConnection);
            cmd.ExecuteNonQueryAsync();

            CloseConnection();
        }


        private void OpenConnection()
        {
            try
            {
                mySqlConnection.Open();
                if (mySqlConnection.State == System.Data.ConnectionState.Open)
                {
                    Console.WriteLine("Connected to db!");
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

        public List<PasswordContainer> GetUserCredentials()
        {
            OpenConnection();

            string query = $"SELECT * FROM credentialsTable WHERE credentialsTable.userKeyToken = \"{userKeyToken}\";";

            MySqlCommand cmd = new MySqlCommand(query, mySqlConnection);
            var reader = cmd.ExecuteReader();


            var userCredentialsList = new List<PasswordContainer>();
            while (reader.Read())
            {
                userCredentialsList.Add(new PasswordContainer(reader["userId"] + "", reader["password"] + ""));
            }
            CloseConnection();

            return userCredentialsList;
        }

        public void Insert(string userID, string password)
        {
            OpenConnection();

            string query = $"INSERT INTO credentialsTable VALUES(\"{userID}\", \"{password}\", \"{userKeyToken}\");";
            MySqlCommand cmd = new MySqlCommand(query, mySqlConnection);
            cmd.ExecuteNonQuery();

            CloseConnection();
        }

        public void Update()
        {
            OpenConnection();

            string query = "..";
            MySqlCommand cmd = new MySqlCommand(query, mySqlConnection);
            cmd.ExecuteNonQuery();

            CloseConnection();
        }

        public void Delete(string userID)
        {
            OpenConnection();

            string query = $"DELETE FROM credentialsTable WHERE userKeyToken=\"{userKeyToken}\" AND userID =\"{userID}\";";
            MySqlCommand cmd = new MySqlCommand(query, mySqlConnection);
            cmd.ExecuteNonQuery();

            CloseConnection();
        }

        public void DeleteAllSessionCredentials(string userKeyToken)
        {
            OpenConnection();

            string query = $"DELETE FROM credentialsTable WHERE userKeyToken=\"{userKeyToken}\"";
            MySqlCommand cmd = new MySqlCommand(query, mySqlConnection);
            cmd.ExecuteNonQuery();
            CloseConnection();
        }
    }
}
