using NAudio.Midi;
using System.Diagnostics;
namespace InEenNotendop.Business
{
    public class MidiProcessor
    {
        public int Score;

        private Song song;
        private Song songPlayed;
        public Song SongForNoteFalling;
        public Song SongForNotePlayback;


        public MidiPlayer MidiPlayer;
        private MidiIn midiIn;
        public Stopwatch Stopwatch { get; set; } // Acurater dan DateTime.Now
        private DateTime startTime;


        //Event voor wanneer noot gespeeld wordt
        public delegate void NotePlayedEventHandler(object sender, NotePlayedEventArgs e);
        public event NotePlayedEventHandler NotePlayed;

        public delegate void SongFinishedEventHandler(object sender);
        public event SongFinishedEventHandler SongFinished;

        public delegate void MidiDeviceNotFoundHandler(object sender);
        public event MidiDeviceNotFoundHandler MidiDeviceNotFound;

        //Constructor
        public MidiProcessor(object Owner, MidiFile midiFile)
        {
            InitializeMidi("Microsoft GS Wavetable Synth");
            Stopwatch = new Stopwatch();
            Stopwatch.Start();
            startTime = DateTime.Now;

            song = new Song(midiFile);
            songPlayed = new Song();
            SongForNoteFalling = new();
            SongForNotePlayback = new();
            SongForNoteFalling.Notes = NoteTimeManipulator.AddDelayToStartSong(song.Notes);
            SongForNotePlayback.Notes = NoteTimeManipulator.AddDelayToSongPlayback(song.Notes);

        }

        //Voor event wanneer midi input gedetecteerd wordt
        private void MidiInMessageReceived(object? sender, MidiInMessageEventArgs e)
        {
            if (e.MidiEvent is NoteOnEvent noteOnEvent)
            {
                MidiPlayer.PlayNote(noteOnEvent.NoteNumber);
                songPlayed.AddNote(new Note(noteOnEvent, Stopwatch.Elapsed));
                OnMidiInMessageReceived(new NotePlayedEventArgs(noteOnEvent.NoteNumber, true));
            }
            else if (e.MidiEvent is NoteEvent noteEvent) // een noteevent wat geen noteonevent is is in dit geval altijd een event die een noot eindigt.
            {
                MidiPlayer.StopNote(noteEvent.NoteNumber);
                OnMidiInMessageReceived(new NotePlayedEventArgs(noteEvent.NoteNumber, false));
            }
        }

        protected virtual void OnMidiInMessageReceived(NotePlayedEventArgs e)
        {
            NotePlayed?.Invoke(this, e);
        }

        public void Dispose()
        {
            try {
                midiIn?.Dispose();
                MidiPlayer.Dispose();
                Stopwatch.Stop();
                Stopwatch.Reset();
            } catch (NullReferenceException e) 
            { 
            }
            
        }

        //Voor event wanneer nummer klaar is
        public void OnSongFinished()
        {
            this.Dispose();
            Score = ScoreCalculator.CalculateScore(song, songPlayed);

        }
        public void LastNoteOfSongElapsed(object? sender)
        {
            SongFinished?.Invoke(this);
        }

        private void InitializeMidi(string desiredOutDevice)
        {
            MidiPlayer = new MidiPlayer(desiredOutDevice, this);

            var numDevices = MidiIn.NumberOfDevices;
            Debug.WriteLine($"numDevices: {numDevices}");
            var desiredDeviceIndex = 0; // DEZE KAN VERANDEREN SOMS SPONTAAN
            if(numDevices < 0)
            {
                numDevices = 0;
            }
            if (desiredDeviceIndex < numDevices)
            {
                midiIn = new MidiIn(desiredDeviceIndex);
                midiIn.MessageReceived += MidiInMessageReceived;
                midiIn.Start();
            }
            else
            {
                MidiDeviceNotFound?.Invoke(this);
            }
        }
    }
}
