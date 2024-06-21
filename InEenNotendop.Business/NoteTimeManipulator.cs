namespace InEenNotendop.Business
{
    public static class NoteTimeManipulator
    {
        public static Song GenerateDelayedSong(Song song, int milliSecondsDelay)
        {
            Song delayedSong = new();
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
    }
}

