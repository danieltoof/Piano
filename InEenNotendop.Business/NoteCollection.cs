using NAudio.Midi;

namespace InEenNotendop.Business
{
    public class NoteCollection
    {
        public List<Note> Notes { get; set; }
        
        //Constructor voor object die input van user opslaat
        public NoteCollection()
        {
            Notes = [];
        }

        public NoteCollection (List<Note> notes)
        {
            Notes = notes;
        }


        //Constructor voor object aangemaakt met een midi-bestand
        public NoteCollection(MidiFile midiFile) 
        {
            Notes = MidiToListConverter.MidiToList(midiFile);
        }

        public void AddNote(Note note)
        {
            Notes.Add(note);
        }
    }
}
