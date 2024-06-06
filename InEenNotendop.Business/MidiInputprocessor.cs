using NAudio.Midi;

namespace PianoHero.Business
{
    public class MidiInputProcessor
    {
        public List<Note> ListOfNotesSong { get; set; }
        public List<Note> ListOfNotesPlayed { get; set; }

        public MidiInputProcessor() {
            ListOfNotesPlayed = [];
            ListOfNotesSong = [];
        }

        public List<Note> MidiToList(MidiFile? midiFile)
        {
            ListOfNotesSong = new List<Note>();
            if (midiFile != null)
            {
                int ticksPerQuarterNote = midiFile.DeltaTicksPerQuarterNote;
                int tempo = 500000; // Default microseconds per kwartnoot (120 BPM)

                foreach (var track in midiFile.Events)
                {
                    int absoluteTime = 0;
                    foreach (MidiEvent midiEvent in track)
                    {
                        absoluteTime += midiEvent.DeltaTime;

                        if (midiEvent is TempoEvent tempoEvent)
                        {
                            tempo = tempoEvent.MicrosecondsPerQuarterNote;
                        }

                        if (midiEvent.CommandCode == MidiCommandCode.NoteOn)
                        {
                            var noteOnEvent = (NoteOnEvent)midiEvent;
                            var startTimeSpan = MidiUtilities.GetTimeSpanFromMidiTicks(absoluteTime, ticksPerQuarterNote, tempo);
                            ListOfNotesSong.Add(new Note(noteOnEvent, startTimeSpan));
                        }

                        if (midiEvent.CommandCode == MidiCommandCode.NoteOff)
                        {
                            var noteOffEvent = (NoteEvent)midiEvent;
                            var endTimeSpan = MidiUtilities.GetTimeSpanFromMidiTicks(absoluteTime, ticksPerQuarterNote, tempo);
                            var note = ListOfNotesSong.Find(note => note.NoteNumber == noteOffEvent.NoteNumber && note.IsPlaying);
                            if (note != null)
                            {
                                note.EndNote(endTimeSpan);
                            }
                        }
                    }
                }
            }
            return ListOfNotesSong;
        }
    }

    public class Note
    {
        public int NoteNumber { get; set; }
        public TimeSpan NoteStartTime { get; set; }
        public TimeSpan NoteDuration { get; set; } 
        public bool IsPlaying { get; set; }
        public bool IsBlockGenerated { get; set; } = false;
        public bool ScoreIsCalculated = false;

        public Note(NoteOnEvent noteOnEvent, TimeSpan startTime) // voor midi input
        {
            NoteNumber = noteOnEvent.NoteNumber;
            NoteStartTime = startTime;
            IsPlaying = true;
            
        }

        public Note(int noteNumber, TimeSpan noteStartTime) // voor noot aanmaken zonder midi event
        {
            NoteNumber = noteNumber;
            NoteStartTime = noteStartTime;
        }

        public void EndNote(TimeSpan endTime)
        {
            NoteDuration = endTime - NoteStartTime;
            IsPlaying = false;
        }
    }
}


#region ThisRegionCanBeIgnored
public class Program
{
    public static void Main()
    {
        // NIET VERWIJDEREN, HET MOET EEN MAIN HEBBEN
        // WAAROM? VRAAG NIET AAN MIJ
        // NEGEER DIT DUS
    }
}

#endregion