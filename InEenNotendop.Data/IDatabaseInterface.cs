using System.Data;
namespace InEenNotendop.Data;

public interface IDatabaseInterface
{
    void StartSshTunnel();
    void StopSshTunnel();
    string FindDirectory(string scriptFileName);
    void StartProcess(string sshTunnelFile);
    void UploadsongToDataBase(string name, string artiest, int length, int bpm, int diffeculty, string filepath);
    void ChangeHighscore(int id, int score, int currentScore);
    DataView GetDataForGrid(int nummerId);
    List<Nummer> LijstFunc(string sqlcommand);
    List<Nummer> MaakLijst();
    List<Nummer> MakeSortedList(int difficulty, string sort);
    List<Nummer> MaakFilteredLijst(int difficulty);
}