namespace InEenNotendop.Business
{
    public class NotePlayedEventArgs : EventArgs
    {
        public int noteNumber { get; private set; } // int van midi event (nootnummer)
        public bool isOnMessage { get; private set; } // als false dan is note uit message

        public NotePlayedEventArgs(int NoteNumber, bool IsNoteOnMessage) 
        {
            noteNumber = NoteNumber;
            isOnMessage = IsNoteOnMessage;
        }
    }
}
