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
    }
}
