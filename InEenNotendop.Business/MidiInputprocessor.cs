using System;
using System.Collections.Generic;
using NAudio.Midi;

namespace InEenNotendop.Business
{
    public class MidiInputProcessor
    {
        public List<Note> ListOfNotesSong { get; set; }
        public List<Note> ListOfNotesPlayed { get; set; }

        public MidiInputProcessor() {
            ListOfNotesPlayed = [];
            ListOfNotesSong = [];
        }

        public List<Note> MidiToList(MidiFile midiFile)
        {
            ListOfNotesSong = new List<Note>();
            if (midiFile != null)
            {
                int ticksPerQuarterNote = midiFile.DeltaTicksPerQuarterNote;
                int tempo = 500000; // Default tempo in microseconds per quarter note (120 BPM)

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

    public static class MidiUtilities
    {
        public static TimeSpan GetTimeSpanFromMidiTicks(int ticks, int ticksPerQuarterNote, int microsecondsPerQuarterNote)
        {
            // Convert microseconds to milliseconds
            double millisecondsPerQuarterNote = microsecondsPerQuarterNote / 1000.0;
            // Calculate milliseconds per tick
            double millisecondsPerTick = millisecondsPerQuarterNote / ticksPerQuarterNote;
            // Convert ticks to milliseconds
            double totalMilliseconds = ticks * millisecondsPerTick;
            // Convert milliseconds to TimeSpan
            return TimeSpan.FromMilliseconds(totalMilliseconds);
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

        public Note(int noteNumber, TimeSpan startTime, TimeSpan noteDuration) // voor midi playback met vertraging
        {
            
        }

        public void EndNote(TimeSpan endTime)
        {
            NoteDuration = endTime - NoteStartTime;
            IsPlaying = false;
        }
    }
}