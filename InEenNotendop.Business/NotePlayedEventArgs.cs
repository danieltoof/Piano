namespace InEenNotendop.Business
{
    public class NotePlayedEventArgs : EventArgs
    {
        public int NoteNumber { get; private set; } // int van midi event (nootnummer)
        public bool IsOnMessage { get; private set; } // als false dan is note uit message

        public NotePlayedEventArgs(int NoteNumber, bool IsNoteOnMessage) 
        {
            this.NoteNumber = NoteNumber;
            IsOnMessage = IsNoteOnMessage;
        }
    }
}
