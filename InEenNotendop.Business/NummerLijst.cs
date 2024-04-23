using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
