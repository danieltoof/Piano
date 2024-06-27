using NAudio.Midi;

namespace InEenNotendop.Business
{
    public static class MidiUtilities
    {
        public static TimeSpan GetTimeSpanFromMidiTicks(int ticks, int ticksPerQuarterNote, int microsecondsPerQuarterNote)
        {
            // Microseconds to milliseconds
            double millisecondsPerQuarterNote = microsecondsPerQuarterNote / 1000.0;
            // Calculate milliseconds per tick
            double millisecondsPerTick = millisecondsPerQuarterNote / ticksPerQuarterNote;
            // Ticks to Milliseconds
            double totalMilliseconds = ticks * millisecondsPerTick;
            // Milliseconds to TimeSpan
            return TimeSpan.FromMilliseconds(totalMilliseconds);
        }

        public static int GetMidiBpm(MidiFile midiFile)
        {
            double tempo = 120; // this is the default tempo if there is no tempoEvent
            foreach (var track in midiFile.Events)
            {
                foreach (var midiEvent in track)
                {
                    if (midiEvent.CommandCode == MidiCommandCode.MetaEvent)
                    {
                        //look for temepoevent that has BPM
                        var metaEvent = midiEvent as MetaEvent;
                        if (metaEvent.MetaEventType == MetaEventType.SetTempo)
                        {
                            var tempoEvent = (TempoEvent)metaEvent;
                            tempo = tempoEvent.Tempo; // Return value when tempo event is found
                            return (int)Math.Round(tempo);
                        }
                    }
                }
            }
            // If no value is found return default BPM (return 120)
            return (int)Math.Round(tempo);
        }

        public static TimeSpan GetSongLength(MidiFile midiFile)
        {
            // first get BPM
            int bpm = GetMidiBpm(midiFile);

            // init waarde = 0
            long totalTime = 0;

            // Then we look at every midi event when they take place.
            // the last occuring time gets used to calculate length
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
            // now we use the predefined function to make a timespan
            TimeSpan timeSpanMidiFile = GetTimeSpanFromMidiTicks((int)totalTime, midiFile.DeltaTicksPerQuarterNote, 60000000 / bpm);
            return timeSpanMidiFile;
        }
        public static int FindMidiDevice(string deviceName, InOrOut inOrOut)
        {
            if (inOrOut == InOrOut.OUT)
            {
                for (int deviceId = 0; deviceId < MidiOut.NumberOfDevices; deviceId++)
                {
                    if (MidiOut.DeviceInfo(deviceId).ProductName.Contains(deviceName))
                    {
                        return deviceId;
                    }
                }
            }
            else if (inOrOut == InOrOut.IN)
            {
                for (int deviceId = 0; deviceId < MidiIn.NumberOfDevices; deviceId++)
                {
                    if (MidiIn.DeviceInfo(deviceId).ProductName.Contains(deviceName))
                    {
                        return deviceId;
                    }
                }
            }
            return -1;
        }
    }
    public enum InOrOut
    {
        IN,
        OUT
    }
}
