using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InEenNotendop.Business
{
    public static class NoteTimeManipulator
    {
        private static TimeSpan incrementStartSong = TimeSpan.FromMilliseconds(2000);
        private static TimeSpan incrementMidiPlayback = TimeSpan.FromMilliseconds(7000);


        public static List<Note> AddDelayToStartSong(List<Note> ListOfNotes)
        {
            foreach (var note in ListOfNotes)
            {
                note.NoteStartTime = note.NoteStartTime.Add(incrementStartSong);
            }
            return ListOfNotes;
        }


        public static List<Note> AddDelayToSongPlayback(List<Note> ListOfNotes)
        {
            foreach (var note in ListOfNotes)
            {
                note.NoteStartTime = note.NoteStartTime.Add(incrementStartSong).Add(incrementMidiPlayback);
            }
            return ListOfNotes;
        }
    }
}

