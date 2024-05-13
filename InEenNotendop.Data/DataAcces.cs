using Microsoft.Data.SqlClient;

namespace InEenNotendop.Data
{
    public class DataProgram
    {
        public string ConnectionString { get; private set; }

        public void MakeConnectionString()
        {
            try
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                builder.DataSource = "Localhost";
                builder.UserID = "SA";
                builder.Password = "Pi@n0Zer0T0Pi@n0Her0";
                builder.InitialCatalog = "PianoHeroDB";
                builder.TrustServerCertificate = true;

                ConnectionString = builder.ToString();
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
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