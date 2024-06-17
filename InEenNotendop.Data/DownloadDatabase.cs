using Microsoft.Data.SqlClient;
using System.Data;

namespace InEenNotendop.Data;

public class DownloadDatabase
{
    private TimeConverter _timeConverter = new TimeConverter();

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