using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Midi;

namespace InEenNotendop.Business
{
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

        public static int GetMidiBPM(MidiFile midiFile)
        {
            double defaultTempo = 500000; // Default to 120 BPM
            foreach (var track in midiFile.Events)
            {
                foreach (var midiEvent in track)
                {
                    if (midiEvent.CommandCode == MidiCommandCode.MetaEvent)
                    {
                        var metaEvent = midiEvent as MetaEvent;
                        if (metaEvent.MetaEventType == MetaEventType.SetTempo)
                        {
                            var tempoEvent = (TempoEvent)metaEvent;
                            defaultTempo = tempoEvent.Tempo; // Use the last tempo event found
                        }
                    }
                }
            }
            return (int)Math.Round(defaultTempo);
        }


        public static int GetMidiTempoEventValue(MidiFile midiFile)
        {
            // Default tempo in microseconds per quarter note (120 BPM)
            double tempo = 500000;
            foreach (var track in midiFile.Events)
            {
                foreach (var midiEvent in track)
                {
                    if (midiEvent.CommandCode == MidiCommandCode.MetaEvent)
                    {
                        var metaEvent = midiEvent as MetaEvent;
                        if (metaEvent.MetaEventType == MetaEventType.SetTempo)
                        {
                            var tempoEvent = (TempoEvent)metaEvent;
                            tempo = tempoEvent.Tempo; // Update tempo with the found value
                        }
                    }
                }
            }

            return (int)Math.Round(tempo);
        }

        public static TimeSpan GetSongLength(MidiFile midiFile)
        {
            int bpm = GetMidiBPM(midiFile);

            long totalTime = 0;

            foreach (var track in midiFile.Events)
            {
                foreach (var midiEvent in track)
                {
                    if (midiEvent.AbsoluteTime > totalTime)
                    {
                        totalTime = midiEvent.AbsoluteTime;
                    }
                }
            }

            double ticksPerQuarterNote = midiFile.DeltaTicksPerQuarterNote;
            double millisecondsPerBeat = 60000 / bpm;
            double millisecondsPerTick = millisecondsPerBeat / ticksPerQuarterNote;
            double totalMilliseconds = totalTime * millisecondsPerTick;

            return TimeSpan.FromMilliseconds(totalMilliseconds);
        }

    }

    
}
