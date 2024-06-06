using System.ComponentModel;
using PianoHero.Data;

namespace PianoHero.Business
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
