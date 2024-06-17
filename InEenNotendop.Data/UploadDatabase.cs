using Microsoft.Data.SqlClient;

namespace InEenNotendop.Data;

public class UploadDatabase
{
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
}