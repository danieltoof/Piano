using System.Diagnostics;

namespace InEenNotendop.Data;

public class SshScript
{
    private Process _sshTunnelProcess;
    private bool _sshtunnelStarted = false;

    // Starts the powershell script to connect to the database
    public void StartSshTunnel()
    {
        if (!_sshtunnelStarted)
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
                sshTunnelFile = $"{currentDirectory}\\sshtunnel.ps1";
            }

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
            _sshtunnelStarted = true;
        }
    }

    // Stops the powershell script after program has shut down
    public void StopSshTunnel()
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
            sshTunnelFile = $"{currentDirectory}\\StopSshTunnel.ps1";
        }

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
        _sshtunnelStarted = true;
    }
}