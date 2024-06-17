using System.ComponentModel;
using InEenNotendop.Data;

namespace InEenNotendop.Business
{
    public class NummerLijst : INotifyPropertyChanged
    {
        DownloadDatabase _downloadDatabase = new DownloadDatabase();
        public int AantalNummers()
        {
            return _downloadDatabase.GetNummersAmount();
        }
        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
