using Renci.SshNet;

namespace InEenNotendop.Data;

public class UploadMidi
{
    public void UploadSongToServer(string name, string artist, string localPath)
    {
        string remoteFilePath = Path.Combine("/home/student/Music", $"{artist} - {name}.mid");
        string localSavePath = Path.Combine(localPath);

        using (var sftp = new SftpClient(ConfigClass.Host, ConfigClass.Username, ConfigClass.Password))
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
}