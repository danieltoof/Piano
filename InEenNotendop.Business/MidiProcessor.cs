using NAudio.Midi;
using System.Diagnostics;
using System.Runtime.Versioning;
using System.Security.Cryptography;
namespace InEenNotendop.Business
{
    public class MidiProcessor
    {
        private readonly int _delayNoteFalling = 2200;
        private readonly int _delayNotePlayback = 7845;

        public int Score;
        public string Name;

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

        public event Action MidiDeviceNotFound;

        //Constructor
        public MidiProcessor(object Owner, MidiFile midiFile)
        {
            Stopwatch = new Stopwatch();
            Stopwatch.Start();
            _startTime = DateTime.Now;

            _song = new NoteCollection(midiFile);
            _songPlayed = new NoteCollection();
            SongForNoteFalling = new NoteCollection(midiFile);
            SongForNotePlayback = new NoteCollection(midiFile);

            SongForNoteFalling = NoteTimeManipulator.GenerateDelayedNoteCollection(SongForNoteFalling, _delayNoteFalling);
            SongForNotePlayback = NoteTimeManipulator.GenerateDelayedNoteCollection(SongForNotePlayback, _delayNotePlayback);

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
            // een noteevent wat geen noteonevent is is in dit geval altijd een event die een noot eindigt.
            else if (e.MidiEvent is NoteEvent noteEvent) 
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
            try
            {
                MidiDeviceNotFound = null;

                MidiPlayer?.Dispose();
                Stopwatch?.Stop();
                Stopwatch?.Reset();

                if (_midiIn != null)
                {
                    _midiIn.Stop();
                    _midiIn.Dispose();
                    _midiIn = null;
                }
            }
            catch (Exception e)
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

        public void InitializeMidi(string desiredOutDevice)
        {
            MidiPlayer = new MidiPlayer(desiredOutDevice, this);

            var numDevices = MidiIn.NumberOfDevices;
            Debug.WriteLine($"numDevices: {numDevices}");
            var deviceIndex = MidiUtilities.FindMidiDevice("Impact GX49", InOrOut.IN);

            if (deviceIndex != -1)
            {
                try
                {
                    _midiIn = new MidiIn(deviceIndex);
                    _midiIn.MessageReceived += MidiInMessageReceived;
                    _midiIn.Start();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error initializing MIDI device: {ex.Message}");
                    OnMidiDeviceNotFound();
                }
            }
            else
            {
                Debug.WriteLine("MIDI device not found");
                OnMidiDeviceNotFound();
            }
        }

        protected virtual void OnMidiDeviceNotFound()
        {
            Debug.WriteLine("Raising MidiDeviceNotFound event");
            MidiDeviceNotFound?.Invoke();
        }
    }
}
