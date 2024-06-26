﻿using Microsoft.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using InEenNotendop.Business;

namespace InEenNotendop.Data;

public class SqlDataAccess : IDatabaseInterface
{
    private Process _sshTunnelProcess;
    private bool _sshTunnelStarted = false;
    private TimeConverter _timeConverter = new TimeConverter();



    // Method to upload information about the song to the database
    public void SaveSongInfo(string name, string artist, int length, int bpm, int difficulty, string filepath)
    {
        using (SqlConnection connection = new SqlConnection(ConfigClass.s_ConnectionString))
        {
            connection.Open();

            using (SqlCommand insertSongCommand = new SqlCommand("INSERT INTO Nummers (Title, Artiest, Lengte, Bpm, Moeilijkheid, Filepath) OUTPUT INSERTED.ID VALUES (@Title, @Artist, @Length, @Bpm, @Difficulty, @Filepath)", connection))
            {
                insertSongCommand.Parameters.AddWithValue("@Title", name);
                insertSongCommand.Parameters.AddWithValue("@Artist", artist);
                insertSongCommand.Parameters.AddWithValue("@Length", length);
                insertSongCommand.Parameters.AddWithValue("@Bpm", bpm);
                insertSongCommand.Parameters.AddWithValue("@Difficulty", difficulty);
                insertSongCommand.Parameters.AddWithValue("@Filepath", filepath);

                // haalt de ID van het liedje
                var lastInsertedId = (int)insertSongCommand.ExecuteScalar();

                // Insert een score van 0 in het nieuwe liedje
                using (SqlCommand insertScoreCommand = new SqlCommand("USE PianoHeroDB \n INSERT INTO Scores (Score, NummerID, Naam) VALUES (0, @LastInsertedId, 'Legend47')", connection))
                {
                    insertScoreCommand.Parameters.AddWithValue("@LastInsertedId", lastInsertedId);
                    insertScoreCommand.ExecuteNonQuery();
                }
            }
            connection.Close();
        }
    }

    // Code to change high-score after song completion
    public void SaveHighscore(int id, int score, int currentScore, string Name)
    {
        using (SqlConnection connection = new SqlConnection(ConfigClass.s_ConnectionString))
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = $"INSERT INTO Scores (Score, NummerID, Naam) VALUES ({score}, {id}, '{Name}')";
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
    }

    // Gets the score of the selected song in SelectingWindow
    public DataView GetAllScores(int nummerId)
    {
        string cmdString = string.Empty;
        using (SqlConnection con = new SqlConnection(ConfigClass.s_ConnectionString))
        {
            cmdString = $"SELECT Score, Naam FROM Scores WHERE NummerID = '{nummerId}' AND Naam <> '' order by Score desc";
            SqlCommand cmd = new SqlCommand(cmdString, con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);

            return dt.DefaultView;
        }
    }
    public DataView GetTopScores(int nummerId, int amount)
    {
        string cmdString = string.Empty;
        using (SqlConnection con = new SqlConnection(ConfigClass.s_ConnectionString))
        {
            cmdString = $"SELECT TOP {amount} Score, Naam FROM Scores WHERE NummerID = '{nummerId}' AND Naam <> '' order by Score desc";
            SqlCommand cmd = new SqlCommand(cmdString, con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);

            return dt.DefaultView;
        }
    }

    // Generic function used to prevent double code with filtering and sorting
    public List<Song> ListFunction(string sqlcommand)
    {
        List<Song> songs = new List<Song>();

        using (SqlConnection connection = new SqlConnection(ConfigClass.s_ConnectionString))
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
                        string artist = reader.GetString(reader.GetOrdinal("Artiest"));
                        int length = reader.GetInt32(reader.GetOrdinal("Lengte"));
                        int bpm = reader.GetInt32(reader.GetOrdinal("Bpm"));
                        int difficulty = reader.GetInt32(reader.GetOrdinal("Moeilijkheid"));
                        DifficultyConverter converter = new DifficultyConverter();
                        string convertedDifficulty = converter.Convert(difficulty);
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

                        string convertedTime = _timeConverter.ToMinutesSeconds(length);

                        Song song = new Song(title, artist, length, bpm, difficulty, id, filepath, score, convertedTime, convertedDifficulty,"");
                        songs.Add(song);
                    }
                    connection.Close();
                }
            }
        }
        return songs;
    }

    // Gets all highscores in a List
    public List<Song> HighscoreListFunction(string sqlcommand)
    {
        List<Song> nummers = new List<Song>();

        using (SqlConnection connection = new SqlConnection(ConfigClass.s_ConnectionString))
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
                        int difficulty = reader.GetInt32(reader.GetOrdinal("Moeilijkheid"));
                        DifficultyConverter converter = new DifficultyConverter();
                        string converteddifficulty = converter.Convert(difficulty);
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
                        string name = reader.GetString(reader.GetOrdinal("Naam"));

                        string convertedTime = _timeConverter.ToMinutesSeconds(lengte);
                        Song song = new Song(title, artiest, lengte, bpm, difficulty, id, filepath, score, convertedTime, converteddifficulty, name);
                        nummers.Add(song);
                    }
                    connection.Close();
                }
            }
        }
        return nummers;
    }
    // Default list method used on startup
    public List<Song> CreateSongsList()
    {
        return ListFunction("SELECT N.Title, N.Artiest, N.Lengte, N.Bpm, N.Moeilijkheid, N.ID, N.Filepath, Score FROM Nummers N INNER JOIN (SELECT NummerID, MAX(Score) AS Score FROM Scores GROUP BY NummerID) S ON N.ID = S.NummerID");
    }

    // Gets sorted list from database
    public List<Song> SortSongsList(int difficulty, string sort)
    {
        if (difficulty != 0)
        {
            return ListFunction($"SELECT N.Title, N.Artiest, N.Lengte, N.Bpm, N.Moeilijkheid, N.ID, N.Filepath, Score FROM Nummers N INNER JOIN (SELECT NummerID, MAX(Score) as Score from Scores group by NummerID) as S on N.ID = S.NummerID WHERE Moeilijkheid = {difficulty} order by {sort};");
        }
        else
        {
            return ListFunction($"SELECT N.Title, N.Artiest, N.Lengte, N.Bpm, N.Moeilijkheid, N.ID, N.Filepath, Score FROM Nummers N INNER JOIN (SELECT NummerID, MAX(Score) as Score from Scores group by NummerID) as S on N.ID = S.NummerID order by {sort};");
        }
    }

    // Gets filtered list from database
    public List<Song> FilterSongsList(int difficulty)
    {
        return ListFunction($"SELECT N.Title, N.Artiest, N.Lengte, N.Bpm, N.Moeilijkheid, N.ID, N.Filepath, Score FROM Nummers N INNER JOIN (SELECT NummerID, MAX(Score) as Score from Scores group by NummerID) as S on N.ID = S.NummerID WHERE Moeilijkheid = {difficulty}");
    }
    public List<Song> MakeHighscoreList()
    {
        return HighscoreListFunction("WITH RankedScores AS (SELECT N.Title, N.Artiest, N.Lengte, N.Bpm, N.Moeilijkheid, N.ID, N.Filepath, S.Score, S.Naam, " +
            "ROW_NUMBER() OVER(PARTITION BY N.ID " +
            "ORDER BY S.Score DESC, S.Naam ASC) AS RowNum FROM Nummers N " +
            "INNER JOIN Scores S ON N.ID = S.NummerID) " +
            "SELECT Title, Artiest, Lengte, Bpm, Moeilijkheid, ID, Filepath, Score, Naam " +
            "FROM RankedScores WHERE RowNum = 1;");
    }
}