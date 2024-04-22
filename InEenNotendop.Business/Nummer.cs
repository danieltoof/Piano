using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InEenNotendop.Business
{
    public class Nummer
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Artiest { get; set; }
        public int Lengte { get; set; }
        public int Bpm { get; set; }
        public int Moeilijkheid { get; set; }

        public string GetMoeilijkheidText()
        {
            switch (Moeilijkheid)
            {
                case 1:
                    return "easy";
                case 2:
                    return "medium";
                case 3:
                    return "difficult";
                default:
                    return "Unknown"; 
            }
        }
        // Methode om nummers uit de database te halen
        public static List<Nummer> GetNummersFromDatabase(string connectionString)
        {
            var nummers = new List<Nummer>();

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Query om nummers uit de database te halen
                    string query = "SELECT ID, Title, Artiest, Lengte, Bpm, Moeilijkheid FROM Nummers";
                    using (var command = new SqlCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var nummer = new Nummer
                                {
                                    ID = reader.GetInt32(0),
                                    Title = reader.GetString(1),
                                    Artiest = reader.GetString(2),
                                    Lengte = reader.GetInt32(3),
                                    Bpm = reader.GetInt32(4),
                                    Moeilijkheid = reader.GetInt32(5)
                                };
                                nummers.Add(nummer);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return nummers;
        }
    }
}
