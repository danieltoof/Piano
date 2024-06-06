using System.ComponentModel;
using InEenNotendop.Data;

namespace InEenNotendop.Business
{
    public class NummerLijst : INotifyPropertyChanged
    {
        DataProgram dataProgram = new DataProgram();
        public int AantalNummers()
        {
            return dataProgram.getNummersAmount();
        }
        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
