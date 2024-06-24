namespace InEenNotendop.Business
{
    public static class NoteTimeManipulator
    {
        public static NoteCollection GenerateDelayedNoteCollection(NoteCollection song, int milliSecondsDelay)
        {
            NoteCollection delayedSong = new();
            foreach (Note n in song.Notes)
            {
                delayedSong.Notes.Add(AddDelayToNote(n, milliSecondsDelay));
            }
            return delayedSong;
        }

        public static Note AddDelayToNote(Note n, int milliSecondsDelay)
        {
            Note delayedNote = new(n.NoteNumber, n.NoteStartTime.Add(TimeSpan.FromMilliseconds(milliSecondsDelay)), n.NoteDuration);
            return delayedNote;
        }

        public static NoteCollection RemoveDelayNoteCollection(NoteCollection song, int milliSecondsDelay)
        {
            NoteCollection delayedSong = new();
            foreach (Note n in song.Notes)
            {
                delayedSong.Notes.Add(RemoveDelayFromNote(n, milliSecondsDelay));
            }
            return delayedSong;
        }

        public static Note RemoveDelayFromNote(Note n, int milliSecondsDelay)
        {
            Note delayedNote = new(n.NoteNumber, n.NoteStartTime.Subtract(TimeSpan.FromMilliseconds(milliSecondsDelay)), n.NoteDuration);
            return delayedNote;
        }
    }
}

