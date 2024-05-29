﻿using Microsoft.Data.SqlClient;
using System.Diagnostics;
using Renci.SshNet;
using System.Xml.Linq;
using System.Security.Cryptography.X509Certificates;

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
                Arguments = "\\Piano\\sshtunnel.ps1",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };
        }

       /* public void DownloadSong()
        {
            string host = "145.44.235.225";
            string username = "student";
            string password = "PianoHero";
            string remoteFilePath = $"/home/student/Music/{filename}";
            string localSavePath = $".\\Resources\\Songs\\{file.mid}";
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

        public void UploadSong(string name, string artist, string localPath)
        {
            string host = "145.44.235.225";
            string username = "student";
            string password = "PianoHero";
            string remoteFilePath = Path.Combine("/home/student/Music", $"{name} - {artist}.mid");
            string localSavePath = Path.Combine(localPath);

            using (var sftp = new SftpClient(host, username, password))
            {
                sftp.Connect();

                if (!File.Exists(localSavePath))
                {
                    throw new FileNotFoundException($"The file does not exist at the specified path: {localSavePath}");
                }

                try
                {
                    using (Stream fileStream = File.OpenRead(localSavePath))
                    {
                        remoteFilePath = remoteFilePath.Replace("\\", "/");
                        sftp.UploadFile(fileStream, remoteFilePath);
                        Console.WriteLine($"Successfully uploaded '{name}' by '{artist}' to the server.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while uploading the file: {ex.Message}");
                }
                finally
                {
                    sftp.Disconnect();
                }
            }
        }


        public void UploadsongToDataBase(string name, string artiest, int length, int bpm, int diffeculty, string filepath)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                using (SqlCommand insertSongCommand = new SqlCommand("INSERT INTO Nummers (Title, Artiest, Lengte, Bpm, Moeilijkheid, Filepath) OUTPUT INSERTED.ID VALUES (@Title, @Artist, @Length, @Bpm, @Difficulty, @Filepath)", connection))
                {
                    insertSongCommand.Parameters.AddWithValue("@Title", name);
                    insertSongCommand.Parameters.AddWithValue("@Artist", artiest);
                    insertSongCommand.Parameters.AddWithValue("@Length", length);
                    insertSongCommand.Parameters.AddWithValue("@Bpm", bpm);
                    insertSongCommand.Parameters.AddWithValue("@Difficulty", diffeculty);
                    insertSongCommand.Parameters.AddWithValue("@Filepath", filepath);

                    // haalt de ID van het liedje
                    var lastInsertedId = (int)insertSongCommand.ExecuteScalar();

                    // Insert een score van 0 in het nieuwe liedje
                    using (SqlCommand insertScoreCommand = new SqlCommand("USE PianoHeroDB \n INSERT INTO Scores (Score, NummerID) VALUES (0, @LastInsertedId)", connection))
                    {
                        insertScoreCommand.Parameters.AddWithValue("@LastInsertedId", lastInsertedId);
                        insertScoreCommand.ExecuteNonQuery();
                    }
                }
                connection.Close();
            }
        }
        public int getNummersAmount()
        {
            if (!string.IsNullOrEmpty(ConnectionString))
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    string sql = "SELECT COUNT(ID) FROM Nummers";
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

        public List<Nummer> LijstFunc(string sqlCommand)
        {
            List<Nummer> nummers = new List<Nummer>();

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = sqlCommand;
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

                            Nummer nummer = new Nummer(title, artiest, lengte, bpm, moeilijkheid, id, filepath, score);

                            nummers.Add(nummer);

                        }
                        connection.Close();
                    }
                }
            }

            return nummers;
        }

        public List<Nummer> MaakLijst()
        {
            return LijstFunc("SELECT Title, Artiest, Lengte, Bpm, Moeilijkheid, ID, Filepath, Score FROM Nummers INNER JOIN Scores ON Nummers.ID = Scores.NummerID;");
        }

        public List<Nummer> MakeSortedList(int Difficulty, string Sort)
        {
            if (Difficulty != 0)
            {
                return LijstFunc($"SELECT Title, Artiest, Lengte, Bpm, Moeilijkheid, ID, Filepath, Score FROM Nummers INNER JOIN Scores ON Nummers.ID = Scores.NummerID WHERE Moeilijkheid = {Difficulty} ORDER BY {Sort}");
            }
            else
            {
                return LijstFunc($"SELECT Title, Artiest, Lengte, Bpm, Moeilijkheid, ID, Filepath, Score FROM Nummers INNER JOIN Scores ON Nummers.ID = Scores.NummerID ORDER BY {Sort}");
            }       
        }
        public List<Nummer> MaakFilteredLijst(int Difficulty)
        {
            return LijstFunc($"SELECT Title, Artiest, Lengte, Bpm, Moeilijkheid, ID, Filepath, Score FROM Nummers INNER JOIN Scores ON Nummers.ID = Scores.NummerID WHERE Moeilijkheid = {Difficulty}");
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