using Microsoft.Data.SqlClient;
using System.Diagnostics;
using Renci.SshNet;

namespace InEenNotendop.Data
{
    public class DataProgram
    {
        public string ConnectionString = "Data Source=127.0.0.1,1433;Initial Catalog=PianoHeroDB;User ID=Newlogin;Password=VeryStr0ngP@ssw0rd;Encrypt=False;";

        public void StartSshTunnel()
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = ".\\sshtunnel.ps1",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };
        }

        /*public void DownloadSong() 
        {
            string host = "145.44.235.225";
            string username = "student";
            string password = "PianoHero";
            string remoteFilePath = $"/home/student/Music/{*//*filename*//*}";
            string localSavePath = $".\\Resources\\Songs\\{*//*file.mid*//*}";
            using (var sftp = new SftpClient(host, username, password))
            {
                sftp.Connect();

                using (Stream fileStream = File.Create(localSavePath))
                {
                    sftp.DownloadFile(remoteFilePath, fileStream);
                }

                sftp.Disconnect();
            }
        }*/
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
                    command.CommandText = "SELECT Title, Artiest, Lengte, Bpm, Moeilijkheid, ID, Filepath, Score FROM Nummers INNER JOIN Scores ON Nummers.ID = Scores.NummerID;";
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
                            string filepath;
                            int filepathOrdinal = reader.GetOrdinal("Filepath");
                            if (!reader.IsDBNull(filepathOrdinal))
                            {
                                filepath = reader.GetString(filepathOrdinal);
                            }
                            else
                            { 
                                filepath = null;
                            }
                            int score = reader.GetInt32(reader.GetOrdinal("Score"));
                            int minutes = (Convert.ToInt32(lengte) / 60);
                            string minutesString = Convert.ToString(minutes);
                            int seconds = (Convert.ToInt32(lengte) % 60);

                            string secondsString = null;

                            // 0 voor de secondes plakken als ze onder 10 zijn
                            if (seconds < 10)
                            {
                                secondsString = "0" + seconds;
                            }
                            else
                            {
                                secondsString = seconds.ToString();
                            }

                            // minuten en secondes aan elkaar plakken
                            string fullTime = minutesString + ":" + secondsString;

                            Nummer nummer = new Nummer(title, artiest, fullTime, bpm, moeilijkheid, id, filepath, score);
                        }
                        connection.Close();
                    }
                }
            }

            return nummers;
        }

        public List<Nummer> MakeSortedList(int Difficulty, string Sort)
        {
            List<Nummer> nummers = new List<Nummer>();

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                using (var command = connection.CreateCommand())
                {
                    if (Difficulty != 0)
                    {
                        command.CommandText = $"SELECT Title, Artiest, Lengte, Bpm, Moeilijkheid, ID, Filepath, Score FROM Nummers INNER JOIN Scores ON Nummers.ID = Scores.NummerID WHERE Moeilijkheid = {Difficulty} ORDER BY {Sort}";
                    }
                    else
                    {
                        command.CommandText = $"SELECT Title, Artiest, Lengte, Bpm, Moeilijkheid, ID, Filepath, Score FROM Nummers INNER JOIN Scores ON Nummers.ID = Scores.NummerID ORDER BY {Sort}";
                    }
                    
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string title = reader.GetString(reader.GetOrdinal("Title"));
                            string artiest = reader.GetString(reader.GetOrdinal("Artiest"));
                            int lengte = reader.GetInt32(reader.GetOrdinal("Lengte"));
                            int bpm = reader.GetInt32(reader.GetOrdinal("Bpm"));
                            int moeilijkheid = reader.GetInt32(reader.GetOrdinal("Moeilijkheid"));
                            int id = reader.GetInt32(reader.GetOrdinal("Id"));
                            string filepath;
                            int filepathOrdinal = reader.GetOrdinal("Filepath");
                            if (!reader.IsDBNull(filepathOrdinal))
                            {
                                filepath = reader.GetString(filepathOrdinal);
                            }
                            else
                            {
                                filepath = null;
                            }
                            int score = reader.GetInt32(reader.GetOrdinal("Score"));

                            int minutes = (Convert.ToInt32(lengte) / 60);
                            string minutesString = Convert.ToString(minutes);
                            int seconds = (Convert.ToInt32(lengte) % 60);

                            string secondsString = null;

                            // 0 voor de secondes plakken als ze onder 10 zijn
                            if (seconds < 10)
                            {
                                secondsString = "0" + seconds;
                            }
                            else
                            {
                                secondsString = seconds.ToString();
                            }

                            // minuten en secondes aan elkaar plakken
                            string fullTime = minutesString + ":" + secondsString;

                            Nummer nummer = new Nummer(title, artiest, fullTime, bpm, moeilijkheid, id, filepath, score);

                            nummers.Add(nummer);
                        }
                        connection.Close();
                    }
                }
            }

            return nummers;
        }
        public List<Nummer> MaakFilteredLijst(int Difficulty)
        {
            List<Nummer> nummers = new List<Nummer>();

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"SELECT Title, Artiest, Lengte, Bpm, Moeilijkheid, ID, Filepath, Score FROM Nummers INNER JOIN Scores ON Nummers.ID = Scores.NummerID WHERE Moeilijkheid = {Difficulty}";
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string title = reader.GetString(reader.GetOrdinal("Title"));
                            string artiest = reader.GetString(reader.GetOrdinal("Artiest"));
                            int lengte = reader.GetInt32(reader.GetOrdinal("Lengte"));
                            int bpm = reader.GetInt32(reader.GetOrdinal("Bpm"));
                            int moeilijkheid = reader.GetInt32(reader.GetOrdinal("Moeilijkheid"));
                            int id = reader.GetInt32(reader.GetOrdinal("Id"));
                            string filepath;
                            int filepathOrdinal = reader.GetOrdinal("Filepath");
                            if (!reader.IsDBNull(filepathOrdinal))
                            {
                                filepath = reader.GetString(filepathOrdinal);
                            }
                            else
                            {
                                filepath = null;
                            }
                            int score = reader.GetInt32(reader.GetOrdinal("Score"));

                            int minutes = (Convert.ToInt32(lengte) / 60);
                            string minutesString = Convert.ToString(minutes);
                            int seconds = (Convert.ToInt32(lengte) % 60);

                            string secondsString = null;

                            // 0 voor de secondes plakken als ze onder 10 zijn
                            if (seconds < 10)
                            {
                                secondsString = "0" + seconds;
                            }
                            else
                            {
                                secondsString = seconds.ToString();
                            }

                            // minuten en secondes aan elkaar plakken
                            string fullTime = minutesString + ":" + secondsString;

                            Nummer nummer = new Nummer(title, artiest, fullTime, bpm, moeilijkheid, id, filepath, score);

                            nummers.Add(nummer);
                        }
                        connection.Close();
                    }
                }
            }
            return nummers;
        }
        public void ChangeHighscore(int ID, int Score)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"UPDATE Scores SET Score = {Score} WHERE NummerID = {ID}";
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }
    }
}