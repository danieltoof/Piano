namespace InEenNotendop.Business
{
    public class NotePlayedEventArgs : EventArgs
    {
        public int NoteNumber { get; private set; } // int from midi event (nootnumber)
        public bool IsOnMessage { get; private set; } // if false, note is not on message

        public NotePlayedEventArgs(int NoteNumber, bool IsNoteOnMessage) 
        {
            this.NoteNumber = NoteNumber;
            IsOnMessage = IsNoteOnMessage;
        }
    }
}
