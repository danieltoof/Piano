using NAudio.Midi;

namespace InEenNotendop.Business
{
    public class NoteCollection
    {
        public List<Note> Notes { get; set; }
        public int MillisecondsDelayAdded { get; set; }
        
        //Constructor for object that saves input from user
        public NoteCollection()
        {
            Notes = [];
        }

        public NoteCollection (List<Note> notes)
        {
            Notes = notes;
        }

        //Constructor for object made with a midi-file
        public NoteCollection(MidiFile midiFile) 
        {
            Notes = MidiToListConverter.MidiToList(midiFile);
        }
        public NoteCollection(MidiFile midiFile, int millissecondsDelay)
        {
            Notes = MidiToListConverter.MidiToList(midiFile);
            MillisecondsDelayAdded = millissecondsDelay;
        }

        public void AddNote(Note note)
        {
            Notes.Add(note);
        }
    }
}
