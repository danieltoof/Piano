using System.Data;
namespace InEenNotendop.Data;

public interface IDatabaseInterface
{
    void StartSshTunnel();
    void StopSshTunnel();
    string FindDirectory(string scriptFileName);
    void StartProcess(string sshTunnelFile);
    void UploadSongToDataBase(string name, string artiest, int length, int bpm, int diffeculty, string filepath);
    void ChangeHighscore(int id, int score, int currentScore, string Name);
    DataView GetDataForPreviewGrid(int nummerId);
    List<Nummer> ListFunction(string sqlcommand);
    List<Nummer> MakeDefaultList();
    List<Nummer> MakeSortedList(int difficulty, string sort);
    List<Nummer> MakeFilteredList(int difficulty);
    List<Nummer> HighscoreListFunction(string sqlcommand);
}