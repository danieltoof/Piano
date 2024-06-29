namespace InEenNotendop.Business;

public interface IMidiInterface
{
    void UploadSongToServer(string name, string artist, string localPath);
    void DownloadSong(string artist, string title);
}