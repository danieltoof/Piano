using System.Data;
namespace InEenNotendop.Data;

public interface IDatabaseInterface
{
    void StartSshTunnel();
    void StopSshTunnel();
    string FindDirectory(string scriptFileName);
    void StartProcess(string sshTunnelFile);
    void UploadSongToDataBase(string name, string artist, int length, int bpm, int difficulty, string filepath);
    void ChangeHighscore(int id, int score, int currentScore, string Name);
    DataView GetDataForGrid(int nummerId);
    List<Song> ListFunction(string sqlcommand);
    List<Song> MakeDefaultList();
    List<Song> MakeSortedList(int difficulty, string sort);
    List<Song> MakeFilteredList(int difficulty);
}