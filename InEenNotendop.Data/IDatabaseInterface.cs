using System.Data;
namespace InEenNotendop.Data;

public interface IDatabaseInterface
{
    void StartSshTunnel();
    void StopSshTunnel();
    string FindPowershellScript(string scriptFileName);
    void ExecutePowershellScript(string sshTunnelFile);
    void SaveSongInfo(string name, string artist, int length, int bpm, int difficulty, string filepath);
    void SaveHighscore(int id, int score, int currentScore, string Name);
    DataView GetAllDataForGrid(int nummerId);
    DataView GetDataForGrid(int nummerId, int amount);
    List<Song> ListFunction(string sqlcommand);
    List<Song> HighscoreListFunction(string sqlcommand);
    List<Song> MakeDefaultList();
    List<Song> MakeSortedList(int difficulty, string sort);
    List<Song> MakeFilteredList(int difficulty);
}