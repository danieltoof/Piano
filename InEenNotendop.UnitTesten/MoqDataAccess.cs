using System.Data;
using InEenNotendop.Data;

namespace InEenNotendop.UI.Tests;

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

    public void UploadSongToDataBase(string name, string artiest, int length, int bpm, int diffeculty, string filepath)
    {
        throw new NotImplementedException();
    }

    public void ChangeHighscore(int id, int score, int currentScore)
    {
        throw new NotImplementedException();
    }

    public DataView GetDataForGrid(int nummerId)
    {
        throw new NotImplementedException();
    }

    public List<Nummer> ListFunction(string sqlcommand)
    {
        throw new NotImplementedException();
    }

    public List<Nummer> MakeDefaultList()
    {
        throw new NotImplementedException();
    }

    public List<Nummer> MakeSortedList(int difficulty, string sort)
    {
        throw new NotImplementedException();
    }

    public List<Nummer> MakeFilteredList(int difficulty)
    {
        throw new NotImplementedException();
    }
}