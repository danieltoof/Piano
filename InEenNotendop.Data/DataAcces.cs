using Microsoft.Data.SqlClient;

namespace InEenNotendop.Data
{
    public class DataProgram
    {
        public string ConnectionString { get; private set; }

        public void StartDataBase()
        {
            try
            {
                string DBname = "PianoHeroDB";
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
                            string createTablesQuery = @"USE PianoHeroDB
                                DROP TABLE IF EXISTS Scores;
                                DROP TABLE IF EXISTS Nummers;

                                CREATE TABLE Nummers (
                                    Title VARCHAR(255),
                                    Artiest VARCHAR(255),
                                    Lengte INT,
                                    Bpm INT,
                                    Moeilijkheid INT NOT NULL,
                                    ID INT IDENTITY(1,1) PRIMARY KEY,
                                    Filepath varchar(255),
                                    CONSTRAINT CHK_Moeilijkheidsgraad CHECK (Moeilijkheid >= 1 AND Moeilijkheid <= 3)
                                    );

                                CREATE TABLE Scores (
                                    Score INT,
                                    NummerID INT,
                                    FOREIGN KEY (NummerID) REFERENCES Nummers(ID)
                                    );


                                DECLARE @CurrentTitle VARCHAR(255) = 'Title1';
                                DECLARE @CurrentTitleNumber INT = 1;

                                DECLARE @CurrentArtist VARCHAR(255) = 'Artiest1';;
                                DECLARE @CurrentArtistNumber INT = 1;



                                WHILE @CurrentTitleNumber <= 25
                                BEGIN
                                    SET @CurrentTitle = CONCAT('Title', @CurrentTitleNumber);
                                    SET @CurrentArtist = CONCAT('Artist', @CurrentArtistNumber)

                                    DECLARE @RandomLengte INT = ABS(CHECKSUM(NEWID())) % 151 + 150;
                                    DECLARE @RandomBpm INT = ABS(CHECKSUM(NEWID())) % 101 + 80;
                                    DECLARE @RandomMoeilijkheid INT = ABS(CHECKSUM(NEWID())) % 3 + 1;

                                    INSERT INTO Nummers (Title, Artiest, Lengte, Bpm, Moeilijkheid)
                                    VALUES (@CurrentTitle, @CurrentArtist, @RandomLengte, @RandomBpm, @RandomMoeilijkheid);

                                    DECLARE @LastInsertedId INT;
                                    SET @LastInsertedId = SCOPE_IDENTITY();

                                    INSERT INTO Scores (Score, NummerID)
                                    VALUES (0, @LastInsertedId);

                                    SET @CurrentTitleNumber = @CurrentTitleNumber + 1;
                                    SET @CurrentArtistNumber = @CurrentArtistNumber + 1;
                                END";

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
                    command.CommandText = "SELECT n.Title, n.Artiest, n.Lengte, n.Bpm, n.Moeilijkheid, n.ID, n.Filepath, s.Score FROM dbo.Nummers AS n INNER JOIN dbo.Scores AS s ON n.ID = s.NummerID";
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

                            Nummer nummer = new Nummer(title,artiest,fullTime,bpm,moeilijkheid,id,filepath,score);

                            nummers.Add(nummer);

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
                        command.CommandText = $"SELECT n.Title, n.Artiest, n.Lengte, n.Bpm, n.Moeilijkheid, n.ID, n.Filepath, s.Score FROM dbo.Nummers AS n INNER JOIN dbo.Scores AS s ON n.ID = s.NummerID WHERE Moeilijkheid = {Difficulty} ORDER BY {Sort}";
                    }
                    else
                    {
                        command.CommandText = $"SELECT n.Title, n.Artiest, n.Lengte, n.Bpm, n.Moeilijkheid, n.ID, n.Filepath, s.Score FROM dbo.Nummers AS n INNER JOIN dbo.Scores AS s ON n.ID = s.NummerID ORDER BY {Sort}";
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
                    command.CommandText = $"SELECT n.Title, n.Artiest, n.Lengte, n.Bpm, n.Moeilijkheid, n.ID, n.Filepath, s.Score FROM dbo.Nummers AS n INNER JOIN dbo.Scores AS s ON n.ID = s.NummerID WHERE Moeilijkheid = {Difficulty}";
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