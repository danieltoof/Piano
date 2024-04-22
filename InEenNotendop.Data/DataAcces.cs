﻿using Microsoft.Data.SqlClient;

namespace InEenNotendop.Data
{
    public class DataProgram
    {
        public void StartDataBase()
        {
            try
            {
                string user = System.Net.Dns.GetHostName() + "\\" + Environment.UserName;
                string DBname = "PianoHeroDB";

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
                                Moelijkheid INT NOT NULL,
                                ID INT AUTO_INCREMENT PRIMARY KEY,
                                CONSTRAINT CHK_Moeilijkheidsgraad CHECK (Moeilijkheid >= 1 AND Moeilijkheid <= 3)
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
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }
            Console.ReadLine();
        }
    }
}