using NAudio.Midi;

namespace PianoHero.Business
{
    public static class MidiUtilities
    {
        public static TimeSpan GetTimeSpanFromMidiTicks(int ticks, int ticksPerQuarterNote, int microsecondsPerQuarterNote)
        {
            // Microseconden naar milliseconden
            double millisecondsPerQuarterNote = microsecondsPerQuarterNote / 1000.0;
            // Milliseconden per tick berekenen
            double millisecondsPerTick = millisecondsPerQuarterNote / ticksPerQuarterNote;
            // Ticks naar Milliseconden
            double totalMilliseconds = ticks * millisecondsPerTick;
            // Milliseconden naar TimeSpan
            return TimeSpan.FromMilliseconds(totalMilliseconds);
        }

        public static int GetMidiBPM(MidiFile midiFile)
        {
            double tempo = 120; // dit is standaard tempo als er geen tempoEvent is gevonden
            foreach (var track in midiFile.Events)
            {
                foreach (var midiEvent in track)
                {
                    if (midiEvent.CommandCode == MidiCommandCode.MetaEvent)
                    {
                        //zoeken naar tempoevent waar BPM in zit
                        var metaEvent = midiEvent as MetaEvent;
                        if (metaEvent.MetaEventType == MetaEventType.SetTempo)
                        {
                            var tempoEvent = (TempoEvent)metaEvent;
                            tempo = tempoEvent.Tempo; // Zodra een tempo event gevonden is waarde returnen
                            return (int)Math.Round(tempo);
                        }
                    }
                }
            }
            // Als geen waarde is gevonden standaard BPM (120 returnen)
            return (int)Math.Round(tempo);
        }

        public static TimeSpan GetSongLength(MidiFile midiFile)
        {
            // eerst BPM ophalen
            int bpm = GetMidiBPM(midiFile);

            // init waarde = 0
            long totalTime = 0;

            // Daarna gaan we van elke midi event kijken wanneer deze plaats vindt. 
            // de laatst voorkomende tijd wordt gebruikt om lengte te berekenen
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
            // nu gaan we eerder gedefinieerde functie gebruiken om een timespan te maken
            TimeSpan timeSpanMidiFile = GetTimeSpanFromMidiTicks((int)totalTime, midiFile.DeltaTicksPerQuarterNote, 60000000 / bpm);
            return timeSpanMidiFile;
        }
    }
}
