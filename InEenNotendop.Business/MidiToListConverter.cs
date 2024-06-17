using NAudio.Midi;

namespace InEenNotendop.Business
{
    public static class MidiToListConverter
    {
        //Deze functie zet een midi-bestand om naar een List.
        //Voor makkelijkere score berekening
        public static List<Note> MidiToList(MidiFile? midiFile)
        {
            List<Note> ListOfNotesSong = new List<Note>();
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

                        //Als er wel een tempo event is dan wordt deze aangepast
                        if (midiEvent is TempoEvent tempoEvent) 
                        {
                            tempo = tempoEvent.MicrosecondsPerQuarterNote;
                        }

                        //Als midi event een NoteOnEvent is, wordt er een nieuwe noot aangemaakt
                        if (midiEvent.CommandCode == MidiCommandCode.NoteOn)
                        {
                            var noteOnEvent = (NoteOnEvent)midiEvent;
                            var startTimeSpan = MidiUtilities.GetTimeSpanFromMidiTicks(absoluteTime, ticksPerQuarterNote, tempo);
                            ListOfNotesSong.Add(new Note(noteOnEvent, startTimeSpan));
                        }

                        //Als midi event een NoteOff event is dan wordt de eindtijd toegevoegd aan corresponderente Note die bij NoteOnEvent is aangemaakt
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
}