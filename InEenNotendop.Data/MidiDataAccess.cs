using Renci.SshNet;

namespace InEenNotendop.Data;

public class MidiDataAccess : IMidiInterface
{
    // Uploads the .mid file to the ubuntu machine
    public void UploadSongToServer(string name, string artist, string localPath)
    {
        string remoteFilePath = Path.Combine("/home/student/Music", $"{artist} - {name}.mid");
        string localSavePath = Path.Combine(localPath);

        using (var sftp = new SftpClient(ConfigClass.s_Host, ConfigClass.s_Username, ConfigClass.s_Password))
        {
            sftp.Connect();

            if (!File.Exists(localSavePath))
            {
                throw new FileNotFoundException($"The file does not exist at the specified path: {localSavePath}");
            }

            try
            {
                using (Stream fileStream = File.OpenRead(localSavePath))
                {
                    remoteFilePath = remoteFilePath.Replace("\\", "/");
                    sftp.UploadFile(fileStream, remoteFilePath);
                    Console.WriteLine($"Successfully uploaded '{artist}' by '{name}' to the server.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while uploading the file: {ex.Message}");
            }
            finally
            {
                sftp.Disconnect();
            }
        }
    }

    // Downloads selected song from the database
    public void DownloadSong(string artist, string title)
    {
        string remoteFilePath = $"/home/student/Music/{artist} - {title}.mid";


        // Get the absolute path of the current working directory
        string currentDirectory = Directory.GetCurrentDirectory();

        // Navigate up directories until the target directory "Song" is found
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

        // Downloads the .mid file from the ubuntu machine
        try
        {
            using (var sftp = new SftpClient(ConfigClass.s_Host, ConfigClass.s_Username, ConfigClass.s_Password))
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