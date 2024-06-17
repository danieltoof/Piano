using System.ComponentModel;
using InEenNotendop.Data;

namespace InEenNotendop.Business
{
    public class NummerLijst : INotifyPropertyChanged
    {
        DataProgram _dataProgram = new DataProgram();
        public int AantalNummers()
        {
            return _dataProgram.GetNummersAmount();
        }
        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
