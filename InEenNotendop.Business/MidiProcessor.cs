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
        public Stopwatch Stopwatch { get; set; } // More accurate than DateTime.Now
        private DateTime _startTime;


        //Event for when a note gets played
        public delegate void NoteEventHandler(object sender, NoteEventArgs e);
        public event NoteEventHandler NoteEvent;

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

        //For event when midi input has to be detected
        private void MidiInMessageReceived(object? sender, MidiInMessageEventArgs e)
        {
            if (e.MidiEvent is NoteOnEvent noteOnEvent)
            {
                MidiPlayer.PlayNote(noteOnEvent.NoteNumber);
                _songPlayed.AddNote(new Note(noteOnEvent, Stopwatch.Elapsed));
                OnMidiInMessageReceived(new NoteEventArgs(noteOnEvent.NoteNumber, true));
            }
            // a notevent that is not a noteonevent is in this case always an event that ends the note.
            else if (e.MidiEvent is NoteEvent noteEvent) 
            {
                MidiPlayer.StopNote(noteEvent.NoteNumber);
                OnMidiInMessageReceived(new NoteEventArgs(noteEvent.NoteNumber, false));
            }
        }

        protected virtual void OnMidiInMessageReceived(NoteEventArgs e)
        {
            NoteEvent?.Invoke(this, e);
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

        //For event when song is done
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
