using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InEenNotendop.Business
{
    public class UIChangeMessage
    {
        public int noteNumber { get; private set; } // int van midi event (nootnummer)
        public bool isOnMessage { get; private set; } // als false dan is note uit message

        public UIChangeMessage(int NoteNumber, bool IsNoteOnMessage) 
        {
            noteNumber = NoteNumber;
            isOnMessage = IsNoteOnMessage;
        }
    }
}
