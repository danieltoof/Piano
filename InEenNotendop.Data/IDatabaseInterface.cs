using System.Data;
namespace InEenNotendop.Data;

public interface IDatabaseInterface
{
    void StartSshTunnel();
    void StopSshTunnel();
    string FindPowershellScript(string scriptFileName);
    void ExecutePowershellScript(string sshTunnelFile);
    void UploadSongInfo(string name, string artist, int length, int bpm, int difficulty, string filepath);
    void SaveHighscore(int id, int score, int currentScore, string Name);
    DataView GetAmountSongsForID(int nummerId, int amount);
    DataView GetAllSongsForID(int nummerId);
    List<Song> MakeSongList(string sqlcommand);
    List<Song> MakeListOfAllSongs();
    List<Song> MakeListForDifficultyAndSort(int difficulty, string sort);
    List<Song> MakeListOfSongsWithDifficulty(int difficulty);
}