using System.Diagnostics;

namespace InEenNotendop.Data;

public class SshScript
{
    private Process _sshTunnelProcess;
    private bool _sshTunnelStarted = false;

    // Starts the powershell script to connect to the database if it is not already running
    public void StartSshTunnel()
    {
        if (!_sshTunnelStarted)
        {
            StartProcess(FindDirectory("sshtunnel.ps1"));
            _sshTunnelStarted = true;
        }
    }

    // Stops the powershell script after program has shut down
    public void StopSshTunnel()
    {
        StartProcess(FindDirectory("StopSshTunnel.ps1"));
        _sshTunnelStarted = false;
    }

    // Method to find the powershell script
    public string FindDirectory(string scriptFileName)
    {
        string currentDirectory = Directory.GetCurrentDirectory();
        string targetDirectory = "InEenNotendop";
        string sshTunnelFile = "";
        while (!currentDirectory.EndsWith(targetDirectory) && !string.IsNullOrEmpty(currentDirectory))
        {
            if (Directory.Exists(sshTunnelFile))
            {
                break; // Found the target directory, exit the loop
            }
            currentDirectory = Path.GetDirectoryName(currentDirectory); // Move up one directory
            sshTunnelFile = $"{currentDirectory}\\{scriptFileName}";
        }
        return sshTunnelFile;
    }

    // Starts the powershell script to start/stop the ssh tunnel
    public void StartProcess(string sshTunnelFile)
    {
        ProcessStartInfo psi = new ProcessStartInfo
        {
            FileName = "powershell.exe",
            Arguments = sshTunnelFile,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        _sshTunnelProcess = Process.Start(psi);
    }
}