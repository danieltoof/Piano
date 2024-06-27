using NAudio.Midi;
using System.Diagnostics;

namespace InEenNotendop.Business
{
    public static class MidiToListConverter
    {
        //This function converts a midi-file to a List.
        //For easier score calcution
        public static List<Note> MidiToList(MidiFile? midiFile)
        {
            List<Note> ListOfNotesSong = new List<Note>();
            if (midiFile != null)
            {
                int ticksPerQuarterNote = midiFile.DeltaTicksPerQuarterNote;
                int tempo = 500000; // Default microseconds per quarternote(120 BPM)

                foreach (var track in midiFile.Events)
                {
                    int absoluteTime = 0;
                    foreach (MidiEvent midiEvent in track)
                    {
                        absoluteTime += midiEvent.DeltaTime;

                        //If there is a tempo event this one gets adjusted
                        if (midiEvent is TempoEvent tempoEvent)
                        {
                            tempo = tempoEvent.MicrosecondsPerQuarterNote;
                        }

                        //If midi event is a NoteOneEvent, a new note gets created
                        if (midiEvent.CommandCode == MidiCommandCode.NoteOn)
                        {
                            var noteOnEvent = (NoteOnEvent)midiEvent;
                            var startTimeSpan = MidiUtilities.GetTimeSpanFromMidiTicks(absoluteTime, ticksPerQuarterNote, tempo);
                            ListOfNotesSong.Add(new Note(noteOnEvent, startTimeSpan));
                        }

                        //If midi event is NoteOff event end time gets added to corresponding Note that got created by NoteOnEvent
                        if (midiEvent.CommandCode == MidiCommandCode.NoteOff)
                        {
                            var noteOffEvent = (NoteEvent)midiEvent;
                            var endTimeSpan = MidiUtilities.GetTimeSpanFromMidiTicks(absoluteTime, ticksPerQuarterNote, tempo);
                            var note = ListOfNotesSong.Find(note => note.NoteNumber == noteOffEvent.NoteNumber && note.IsPlaying);
                            if (note != null)
                            {
                                note.EndNote(endTimeSpan);
                            }
                            else
                            {
                                Debug.WriteLine($"Note not found for NoteNumber: {noteOffEvent.NoteNumber}, List size: {ListOfNotesSong.Count}");
                            }
                        }
                    }
                }
            }
            return ListOfNotesSong;
        }
    }
}