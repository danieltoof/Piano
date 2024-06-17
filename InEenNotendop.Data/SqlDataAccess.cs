﻿using Microsoft.Data.SqlClient;
using System.Data;
using System.Diagnostics;

namespace InEenNotendop.Data;

public class SqlDataAccess
{
    private Process _sshTunnelProcess;
    private bool _sshTunnelStarted = false;
    private TimeConverter _timeConverter = new TimeConverter();

    // Starts the powershell script to connect to the database if it is not already running
    public void StartSshTunnel()
    {
        if (!_sshTunnelStarted)
        {
            StartProcess(FindDirectory("sshtunnel.ps1"));
            _sshTunnelStarted = true;
        }
    }

    // Stops the powershell script after program has shut down
    public void StopSshTunnel()
    {
        StartProcess(FindDirectory("StopSshTunnel.ps1"));
        _sshTunnelStarted = false;
    }

    // Method to find the powershell script
    public string FindDirectory(string scriptFileName)
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
            sshTunnelFile = $"{currentDirectory}\\{scriptFileName}";
        }
        return sshTunnelFile;
    }

    // Starts the powershell script to start/stop the ssh tunnel
    public void StartProcess(string sshTunnelFile)
    {
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
    }

    // Method to upload information about the song to the database
    public void UploadsongToDataBase(string name, string artiest, int length, int bpm, int diffeculty, string filepath)
    {
        using (SqlConnection connection = new SqlConnection(ConfigClass.ConnectionString))
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

    // Code to change high-score after song completion
    public void ChangeHighscore(int id, int score, int currentScore)
    {
        if (score > currentScore)
        {
            using (SqlConnection connection = new SqlConnection(ConfigClass.ConnectionString))
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

    // Generic function used to prevent double code with filtering and sorting
    public List<Nummer> LijstFunc(string sqlcommand)
    {
        List<Nummer> nummers = new List<Nummer>();

        using (SqlConnection connection = new SqlConnection(ConfigClass.ConnectionString))
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

                        string convertedTime = _timeConverter.ToMinutesSeconds(lengte);
                        Nummer nummer = new Nummer(title, artiest, lengte, bpm, moeilijkheid, id, filepath, score, convertedTime, convertedMoeilijkheid);
                        nummers.Add(nummer);
                    }
                    connection.Close();
                }
            }
        }
        return nummers;
    }

    // Gets the score of the selected song in SelectingWindow
    public DataView GetDataForGrid(int nummerId)
    {
        string cmdString = string.Empty;
        using (SqlConnection con = new SqlConnection(ConfigClass.ConnectionString))
        {
            cmdString = $"SELECT Score FROM Scores WHERE NummerID = '{nummerId}'";
            SqlCommand cmd = new SqlCommand(cmdString, con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);

            return dt.DefaultView;
        }
    }

    // Default list method used on startup
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
}