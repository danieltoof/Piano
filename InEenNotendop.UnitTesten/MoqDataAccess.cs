using System.Data;
using InEenNotendop.Data;

namespace InEenNotendop.UnitTesten;

public class MoqDataAccess : IDatabaseInterface
{
    // Not relevant to Moq testing
    public void StartSshTunnel()
    {
        throw new NotImplementedException();
    }

    // Not relevant to Moq testing
    public void StopSshTunnel()
    {
        throw new NotImplementedException();
    }

    public string FindDirectory(string scriptFileName)
    {
        string currentDirectory = Directory.GetCurrentDirectory();
        string targetDirectory = "InEenNotendop.UnitTesten";
        string moqTextFile = "";
        while (!currentDirectory.EndsWith(targetDirectory) && !string.IsNullOrEmpty(currentDirectory))
        {
            if (Directory.Exists(moqTextFile))
            {
                break; // Found the target directory, exit the loop
            }
            currentDirectory = Path.GetDirectoryName(currentDirectory); // Move up one directory
            moqTextFile = $"{currentDirectory}\\{scriptFileName}";
        }
        return moqTextFile;
    }

    // Not relevant to Moq testing
    
    public void StartProcess(string sshTunnelFile)
    {
        throw new NotImplementedException();
    }

    public void UploadSongToDataBase(string name, string artist, int length, int bpm, int difficulty, string filepath)
    {
        throw new NotImplementedException();
    }

    public void ChangeHighscore(int id, int score, int currentScore, string Name)
    {
        throw new NotImplementedException();
    }

    public DataView GetDataForGrid(int nummerId)
    {
        throw new NotImplementedException();
    }

    public List<Song> ListFunction(string sqlcommand)
    {
        throw new NotImplementedException();
    }

    public List<Song> MakeDefaultList()
    {
        throw new NotImplementedException();
    }

    public List<Song> MakeSortedList(int difficulty, string sort)
    {
        throw new NotImplementedException();
    }

    public List<Song> MakeFilteredList(int difficulty)
    {
        throw new NotImplementedException();
    }
}
