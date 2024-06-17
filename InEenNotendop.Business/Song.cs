using NAudio.Midi;

namespace InEenNotendop.Business
{
    public class Song
    {
        public List<Note> Notes { get; set; }
        
        //Constructor voor object die input van user opslaat
        public Song()
        {
            Notes = [];
        }


        //Constructor voor object aangemaakt met een midi-bestand
        public Song(MidiFile midiFile) 
        {
            Notes = MidiToListConverter.MidiToList(midiFile);
        }

        public void AddNote(Note note)
        {
            Notes.Add(note);
        }
    }
}
