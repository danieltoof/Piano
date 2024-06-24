using NAudio.Midi;
using System.Diagnostics;
using System.Runtime.Versioning;
namespace InEenNotendop.Business
{
    public class MidiProcessor
    {
        public int Score;

        private NoteCollection _song;
        private NoteCollection _songPlayed;
        public NoteCollection SongForNoteFalling { get;  set; }
        public NoteCollection SongForNotePlayback { get; set; }


        public MidiPlayer MidiPlayer;
        private MidiIn _midiIn;
        public Stopwatch Stopwatch { get; set; } // Acurater dan DateTime.Now
        private DateTime _startTime;


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
            _startTime = DateTime.Now;

            _song = new NoteCollection(midiFile);
            _songPlayed = new NoteCollection();
            SongForNoteFalling = new NoteCollection(midiFile);
            SongForNotePlayback = new NoteCollection(midiFile);

            SongForNoteFalling = NoteTimeManipulator.GenerateDelayedSong(SongForNoteFalling, 2200);
            SongForNotePlayback = NoteTimeManipulator.GenerateDelayedSong(SongForNotePlayback, 7900);

        }

        //Voor event wanneer midi input gedetecteerd wordt
        private void MidiInMessageReceived(object? sender, MidiInMessageEventArgs e)
        {
            if (e.MidiEvent is NoteOnEvent noteOnEvent)
            {
                MidiPlayer.PlayNote(noteOnEvent.NoteNumber);
                _songPlayed.AddNote(new Note(noteOnEvent, Stopwatch.Elapsed));
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
                MidiPlayer?.Dispose();
                Stopwatch?.Stop();
                Stopwatch?.Reset();
            } catch (Exception e) 
            {
                Debug.WriteLine($"Error during disposal: {e.Message}");
            }
            
        }

        //Voor event wanneer nummer klaar is
        public void OnSongFinished()
        {
            Score = ScoreCalculator.CalculateScore(SongForNotePlayback, _songPlayed);
            this.Dispose();

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
                try
                {
                    _midiIn?.Dispose();
                    _midiIn = new MidiIn(desiredDeviceIndex);
                    _midiIn.MessageReceived += MidiInMessageReceived;
                    _midiIn.Start();
                } catch (NAudio.MmException e)
                {
                    Debug.WriteLine($"Error: {e.Message}");
                }

            }
            else
            {
                MidiDeviceNotFound?.Invoke(this);
            }
        }
    }
}
