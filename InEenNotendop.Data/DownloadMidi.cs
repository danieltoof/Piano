using Renci.SshNet;

namespace InEenNotendop.Data;

public class DownloadMidi
{
    // Downloads selected song from the database
    public void DownloadSong(string artist, string title)
    {
        string remoteFilePath = $"/home/student/Music/{artist} - {title}.mid";


        // Get the absolute path of the current working directory
        string currentDirectory = Directory.GetCurrentDirectory();

        // Navigate up directories until the target directory "Songs" is found
        string targetDirectory = "InEenNotendop";
        string localSavePath = "";
        while (!currentDirectory.EndsWith(targetDirectory) && !string.IsNullOrEmpty(currentDirectory))
        {
            if (Directory.Exists(localSavePath))
            {
                break; // Found the target directory, exit the loop
            }
            currentDirectory = Path.GetDirectoryName(currentDirectory); // Move up one directory
            localSavePath = $"{currentDirectory}" + $"\\Resources\\Song\\{artist} - {title}.mid";
        }

        if (string.IsNullOrEmpty(localSavePath))
        {
            Console.WriteLine($"Target directory '{targetDirectory}' not found.");
            return;
        }

        try
        {
            using (var sftp = new SftpClient(ConfigClass.Host, ConfigClass.Username, ConfigClass.Password))
            {
                sftp.Connect();

                if (!sftp.Exists(remoteFilePath))
                {
                    Console.WriteLine($"File does not exist on the server: {remoteFilePath}");
                    return;
                }

                using (Stream fileStream = File.Create(localSavePath))
                {
                    sftp.DownloadFile(remoteFilePath, fileStream);
                }

                sftp.Disconnect();
                Console.WriteLine($"Downloaded song '{title}' by '{artist}' to {localSavePath}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}