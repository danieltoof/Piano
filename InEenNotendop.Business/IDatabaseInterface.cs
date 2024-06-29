using System.Data;

namespace InEenNotendop.Business;

public interface IDatabaseInterface
{
    void SaveSongInfo(string name, string artist, int length, int bpm, int difficulty, string filepath);
    void SaveHighscore(int id, int score, int currentScore, string Name);
    DataView GetAllScores(int nummerId);
    DataView GetTopScores(int nummerId, int amount);
    List<Song> MakeDefaultList();
    List<Song> MakeSortedList(int difficulty, string sort);
    List<Song> MakeFilteredList(int difficulty);
}