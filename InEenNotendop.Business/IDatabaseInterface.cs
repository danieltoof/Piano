using System.Data;

namespace InEenNotendop.Business;

public interface IDatabaseInterface
{
    void SaveSongInfo(string name, string artist, int length, int bpm, int difficulty, string filepath);
    void SaveHighscore(int id, int score, int currentScore, string Name);
    DataView GetAllScores(int nummerId);
    DataView GetTopScores(int nummerId, int amount);
    List<Song> CreateSongsList();
    List<Song> SortSongsList(int difficulty, string sort);
    List<Song> FilterSongsList(int difficulty);
}