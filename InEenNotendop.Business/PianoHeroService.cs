using System.Data;

namespace InEenNotendop.Business;

public class PianoHeroService
{
    private IDatabaseInterface _database;

    public PianoHeroService(IDatabaseInterface database)
    {
        _database = database;
    }

    public void SaveSongInfo(string name, string artist, int length, int bpm, int difficulty, string filepath)
    {
        _database.SaveSongInfo(name, artist, length, bpm, difficulty, filepath);
    }

    public void SaveHighscore(int id, int score, int currentScore, string Name)
    {
        _database.SaveHighscore(id, score, currentScore, Name);
    }

    public DataView GetAllScores(int nummerId)
    {
        return _database.GetAllScores(nummerId);
    }

    public DataView GetTopScores(int nummerId, int amount)
    {
        return _database.GetTopScores(nummerId, amount);
    }

    public List<Song> DefaultSongsList()
    {
        return _database.MakeDefaultList();
    }

    public List<Song> SortSongsList(int difficulty, string sort)
    {
        return _database.MakeSortedList(difficulty, sort);
    }

    public List<Song> FilterSongsList(int difficulty)
    {
        return _database.MakeFilteredList(difficulty);
    }
}