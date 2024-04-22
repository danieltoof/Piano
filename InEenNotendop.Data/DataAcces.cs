using System;
using Microsoft.Data.SqlClient;
using System.Text;
using System.Reflection.PortableExecutable;

namespace InEenNotendop.Data
{
    public class DataProgram
    {
        public string ConnectionString { get; private set; }

        public void StartDataBase()
        {
            try
            {
                string DBname = "newPianoHeroDB";
                string user = System.Net.Dns.GetHostName() + "\\" + Environment.UserName;

                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                builder.DataSource = "(localdb)\\MSSQLLocalDB";
                builder.IntegratedSecurity = true;
                builder.UserID = user;
                builder.Password = "";
                builder.ApplicationIntent = ApplicationIntent.ReadWrite;

                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    connection.Open();

                    // Check if the database exists
                    string checkDatabaseExistsQuery = $"SELECT 1 FROM sys.databases WHERE name = '{DBname}'";
                    using (SqlCommand checkDatabaseExistsCommand = new SqlCommand(checkDatabaseExistsQuery, connection))
                    {
                        object result = checkDatabaseExistsCommand.ExecuteScalar();
                        if (result == null)
                        {
                            // Database does not exist, create it
                            string createDatabaseQuery = $"CREATE DATABASE {DBname}";
                            using (SqlCommand createDatabaseCommand = new SqlCommand(createDatabaseQuery, connection))
                            {
                                createDatabaseCommand.ExecuteNonQuery();
                            }

                            // Switch to the newly created database
                            builder.InitialCatalog = DBname;
                            connection.ChangeDatabase(DBname);

                            // Create tables
                            string createTablesQuery = @"
                        CREATE TABLE Nummers (
                            Title VARCHAR(255),
                            Artiest VARCHAR(255),
                            Lengte INT,
                            Bpm INT,
                            Moeilijkheid INT,
                            ID INT IDENTITY(1,1) PRIMARY KEY
                        );

                        CREATE TABLE Scores (
                            Score INT,
                            NummerID INT,
                            FOREIGN KEY (NummerID) REFERENCES Nummers(ID)
                        );";
                            using (SqlCommand createTablesCommand = new SqlCommand(createTablesQuery, connection))
                            {
                                createTablesCommand.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            // Database exists, switch to it
                            builder.InitialCatalog = DBname;
                            connection.ChangeDatabase(DBname);
                        }
                    }
                    connection.Close();
                }

                // Set the connection string property
                ConnectionString = builder.ConnectionString;
            }
            catch (Exception ex)
            {
                // Handle exceptions as needed
                Console.WriteLine(ex.Message);
            }
            
        }

        public int getNummersAmount()
        {
            if (!string.IsNullOrEmpty(ConnectionString))
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    string sql = "SELECT COUNT(ID) FROM dbo.Nummers";
                    using (SqlCommand command = new SqlCommand(sql,connection))
                    {
                        return (int)command.ExecuteScalar();
                    }
                }
            }
            else
            {
                Console.WriteLine("Connection string is not set.");
                return -1;
            }
        }

        public List<Nummer> MaakLijst()
        {
            List<Nummer> nummers = new List<Nummer>();

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                using(var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT Title, Artiest, Lengte, Bpm, Moeilijkheid, ID FROM dbo.Nummers";
                    connection.Open();
                    using(var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string title = reader.GetString(reader.GetOrdinal("Title"));
                            string artiest = reader.GetString(reader.GetOrdinal("Artiest"));
                            int lengte = reader.GetInt32(reader.GetOrdinal("Lengte"));
                            int bpm = reader.GetInt32(reader.GetOrdinal("Bpm"));
                            int moeilijkheid = reader.GetInt32(reader.GetOrdinal("Moeilijkheid"));
                            int id = reader.GetInt32(reader.GetOrdinal("Id"));

                            Nummer nummer = new Nummer(title,artiest,lengte,bpm,moeilijkheid,id);

                            nummers.Add(nummer);

                        }
                        connection.Close();
                    }
                }
            }

            return nummers;
        }


    }
}