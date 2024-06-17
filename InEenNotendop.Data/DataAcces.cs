using Microsoft.Data.SqlClient;
using System.Diagnostics;
using Renci.SshNet;
using System.Data;

namespace InEenNotendop.Data
{
    // Code to handle data coming from and going to the database
    public class DataProgram
    {
        public string ConnectionString = "Data Source=127.0.0.1,1433;Initial Catalog=PianoHeroDB;User ID=Newlogin;Password=VeryStr0ngP@ssw0rd;Encrypt=False;";
        private Process _sshTunnelProcess;
        private bool _sshtunnelStarted = false;

        static readonly string Host = "145.44.235.225";
        static readonly string Username = "student";
        static readonly string Password = "PianoHero";

        // Starts the powershell script to connect to the database
        public void StartSshTunnel()
        {
            if (!_sshtunnelStarted)
            {
                string currentDirectory = Directory.GetCurrentDirectory();
                string targetDirectory = "InEenNotendop";
                string sshTunnelFile = "";
                while (!currentDirectory.EndsWith(targetDirectory) && !string.IsNullOrEmpty(currentDirectory))
                {
                    if (Directory.Exists(sshTunnelFile))
                    {
                        break; // Found the target directory, exit the loop
                    }
                    currentDirectory = Path.GetDirectoryName(currentDirectory); // Move up one directory
                    sshTunnelFile = $"{currentDirectory}\\sshtunnel.ps1";
                }

                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = sshTunnelFile,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                _sshTunnelProcess = Process.Start(psi);
                _sshtunnelStarted = true;
                 
            }
        }

        // Stops the powershell script after program has shut down
        public void StopSshTunnel()
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            string targetDirectory = "InEenNotendop";
            string sshTunnelFile = "";
            while (!currentDirectory.EndsWith(targetDirectory) && !string.IsNullOrEmpty(currentDirectory))
            {
                if (Directory.Exists(sshTunnelFile))
                {
                    break; // Found the target directory, exit the loop
                }
                currentDirectory = Path.GetDirectoryName(currentDirectory); // Move up one directory
                sshTunnelFile = $"{currentDirectory}\\StopSshTunnel.ps1";
            }

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = sshTunnelFile,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            _sshTunnelProcess = Process.Start(psi);
            _sshtunnelStarted = true;
        }

        // Downloads selected song from the database
        public void DownloadSong(string artist, string title)
        {
            string remoteFilePath = $"/home/student/Music/{artist} - {title}.mid";


            // Get the absolute path of the current working directory
            string currentDirectory = Directory.GetCurrentDirectory();

            // Navigate up directories until the target directory "Songs" is found
            string targetDirectory = "InEenNotendop";
            string localSavePath = "";
            while (!currentDirectory.EndsWith(targetDirectory) && !string.IsNullOrEmpty(currentDirectory))
            {
                if (Directory.Exists(localSavePath))
                {
                    break; // Found the target directory, exit the loop
                }
                currentDirectory = Path.GetDirectoryName(currentDirectory); // Move up one directory
                localSavePath = $"{currentDirectory}" + $"\\Resources\\Song\\{artist} - {title}.mid";
            }

            if (string.IsNullOrEmpty(localSavePath))
            {
                Console.WriteLine($"Target directory '{targetDirectory}' not found.");
                return;
            }

            try
            {
                using (var sftp = new SftpClient(Host, Username, Password))
                {
                    sftp.Connect();

                    if (!sftp.Exists(remoteFilePath))
                    {
                        Console.WriteLine($"File does not exist on the server: {remoteFilePath}");
                        return;
                    }

                    using (Stream fileStream = File.Create(localSavePath))
                    {
                        sftp.DownloadFile(remoteFilePath, fileStream);
                    }

                    sftp.Disconnect();
                    Console.WriteLine($"Downloaded song '{title}' by '{artist}' to {localSavePath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        // Code to handle uploading the midi file to the database
        public void UploadSongToServer(string name, string artist, string localPath) 
        {
            string remoteFilePath = Path.Combine("/home/student/Music", $"{artist} - {name}.mid");
            string localSavePath = Path.Combine(localPath);

            using (var sftp = new SftpClient(Host, Username, Password))
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
                        Console.WriteLine($"Successfully uploaded '{artist}' by '{name}' to the server.");
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

        // Code to put the song in the SQL database
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

        // Gets the score for the song
        public DataView GetDataForGrid(int nummerId)
        {
            string cmdString = string.Empty;
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                cmdString = $"SELECT Score FROM Scores WHERE NummerID = '{nummerId}'";
                SqlCommand cmd = new SqlCommand(cmdString, con);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);

                return dt.DefaultView;
            }
        }

        // Generic function used to prevent double code with filtering and sorting
        public List<Nummer> LijstFunc(string sqlcommand)
        {
            List<Nummer> nummers = new List<Nummer>();

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = sqlcommand;
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
                            MoeilijkheidConverter converter = new MoeilijkheidConverter();
                            string convertedMoeilijkheid = converter.Convert(moeilijkheid);
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

                            string convertedTime = ToMinutesSeconds(lengte);
                            Nummer nummer = new Nummer(title, artiest, lengte, bpm, moeilijkheid, id, filepath, score, convertedTime, convertedMoeilijkheid);
                            nummers.Add(nummer);
                        }
                        connection.Close();
                    }
                }
            }
            return nummers;
        }
        public int GetNummersAmount()
        {
            if (!string.IsNullOrEmpty(ConnectionString))
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    string sql = "SELECT COUNT(ID) FROM Nummers";
                    using (SqlCommand command = new SqlCommand(sql, connection))
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

        // Default list method
        public List<Nummer> MaakLijst()
        {
            return LijstFunc("SELECT Title, Artiest, Lengte, Bpm, Moeilijkheid, ID, Filepath, Score FROM Nummers INNER JOIN Scores ON Nummers.ID = Scores.NummerID;");
        }

        // Gets sorted list from database
        public List<Nummer> MakeSortedList(int difficulty, string sort)
        {
            if (difficulty != 0)
            {
                return LijstFunc($"SELECT Title, Artiest, Lengte, Bpm, Moeilijkheid, ID, Filepath, Score FROM Nummers INNER JOIN Scores ON Nummers.ID = Scores.NummerID WHERE Moeilijkheid = {difficulty} ORDER BY {sort}");
            }
            else
            {
                return LijstFunc($"SELECT Title, Artiest, Lengte, Bpm, Moeilijkheid, ID, Filepath, Score FROM Nummers INNER JOIN Scores ON Nummers.ID = Scores.NummerID ORDER BY {sort}");
            }
        }
         
        // Gets filtered list from database
        public List<Nummer> MaakFilteredLijst(int difficulty)
        {
            return LijstFunc($"SELECT Title, Artiest, Lengte, Bpm, Moeilijkheid, ID, Filepath, Score FROM Nummers INNER JOIN Scores ON Nummers.ID = Scores.NummerID WHERE Moeilijkheid = {difficulty}");
        }

        // Code to change high-score after song completion
        public void ChangeHighscore(int id, int score, int currentScore)
        {
            if (score > currentScore)
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = $"UPDATE Scores SET Score = {score} WHERE NummerID = {id}";
                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                }
            }
        }

        public string ToMinutesSeconds(int fullTime)
        {
            int minutes = (Convert.ToInt32(fullTime) / 60);
            string minutesString = Convert.ToString(minutes);
            int seconds = (Convert.ToInt32(fullTime) % 60);

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
            return minutesString + ":" + secondsString;
            //return "aaa";
        }
    }
}