using System.Diagnostics;
using NAudio.Midi;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using InEenNotendop.Business;
using System.Windows.Media.Animation;
using System.IO;
using InEenNotendop.Data;
using Microsoft.VisualBasic;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace InEenNotendop.UI
{
    /// <summary>
    /// Interaction logic for MidiPlayWindow.xaml
    /// </summary>
    public partial class MidiPlayWindow : Window
    {
        public SongsWindow SongsWindow { get; set; }

        private bool _playMidiFile = false;
        private bool _songFinished = false;
        private int _currentScore;

        private MidiProcessor _midiProcessor;
        private TimeSpan _endLastNote;
        private static SqlDataAccess _sqlDataAccess = new();
        private PianoHeroService _pianoHeroService = new PianoHeroService(_sqlDataAccess);
        private int _songId;

        private Brush _noteHitBrush = Brushes.IndianRed; // Colour when key is pressed
        private Brush _whiteKeysBrush = Brushes.WhiteSmoke;
        private Brush _blackKeyBrush = Brushes.Black;
        private Brush _fallingBlockBrush;
        private Dictionary<int, ButtonData> _midiNoteToButton = new(); // int = Midi notonumber, Button = button that's assigned to that note.

        private DateTime _startTime; // We need this to calculate the time when the midi note is played.
        private object _value;
        private DispatcherTimer _timer;
        private const double _noteHeightPerSecond = 200; // Number for height block per second
        private const double _fallingSpeed = 200.0; // Speed of falling blocks in units per second
        private const double _timerInterval = 16; /* In ms, the lower the more accurate the code is when generating blocks and when a key has to be played.
                                                     When it's lower it can impact performance of the program. 
                                                     16ms is about 60fps.*/
        private double _fallingDuration;

        public MidiPlayWindow(string filePath, object sender, bool playMidiFile, int songId, SongsWindow? songsWindow, int currentScore)
        {
            _songId = songId;
            SongsWindow = songsWindow;
            _playMidiFile = playMidiFile;
            _currentScore = currentScore;

            try
            {
                _midiProcessor = new MidiProcessor(this, new MidiFile(filePath));
                _midiProcessor.MidiDeviceNotFound += MidiProcessor_MidiDeviceNotFound;
                _midiProcessor.NoteEvent += MidiProcessor_NoteEvent;
            }
            catch (FileNotFoundException e)
            {
                MessageBox.Show(e.Message);
                Close();
            }
            try
            {
                _endLastNote = _midiProcessor.SongForNotePlayback.Notes[_midiProcessor.SongForNotePlayback.Notes.Count - 1].NoteStartTime + 
                               _midiProcessor.SongForNotePlayback.Notes[_midiProcessor.SongForNotePlayback.Notes.Count - 1].NoteDuration;
                _endLastNote += TimeSpan.FromSeconds(1);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                Close();
            }


            InitializeComponent();
            _midiProcessor?.InitializeMidi("Microsoft GS Wavetable Synth");


            #region mapping midi notes to the buttons


            _midiNoteToButton.Add(36, new ButtonData(C1Button, _whiteKeysBrush));
            _midiNoteToButton.Add(37, new ButtonData(Cs1Button, _blackKeyBrush));
            _midiNoteToButton.Add(38, new ButtonData(D1Button, _whiteKeysBrush));
            _midiNoteToButton.Add(39, new ButtonData(Ds1Button, _blackKeyBrush));
            _midiNoteToButton.Add(40, new ButtonData(E1Button, _whiteKeysBrush));
            _midiNoteToButton.Add(41, new ButtonData(F1Button, _whiteKeysBrush));
            _midiNoteToButton.Add(42, new ButtonData(Fs1Button, _blackKeyBrush));
            _midiNoteToButton.Add(43, new ButtonData(G1Button, _whiteKeysBrush));
            _midiNoteToButton.Add(44, new ButtonData(Gs1Button, _blackKeyBrush));
            _midiNoteToButton.Add(45, new ButtonData(A1Button, _whiteKeysBrush));
            _midiNoteToButton.Add(46, new ButtonData(As1Button, _blackKeyBrush));
            _midiNoteToButton.Add(47, new ButtonData(B1Button, _whiteKeysBrush));

            _midiNoteToButton.Add(48, new ButtonData(C2Button, _whiteKeysBrush));
            _midiNoteToButton.Add(49, new ButtonData(Cs2Button, _blackKeyBrush));
            _midiNoteToButton.Add(50, new ButtonData(D2Button, _whiteKeysBrush));
            _midiNoteToButton.Add(51, new ButtonData(Ds2Button, _blackKeyBrush));
            _midiNoteToButton.Add(52, new ButtonData(E2Button, _whiteKeysBrush));
            _midiNoteToButton.Add(53, new ButtonData(F2Button, _whiteKeysBrush));
            _midiNoteToButton.Add(54, new ButtonData(Fs2Button, _blackKeyBrush));
            _midiNoteToButton.Add(55, new ButtonData(G2Button, _whiteKeysBrush));
            _midiNoteToButton.Add(56, new ButtonData(Gs2Button, _blackKeyBrush));
            _midiNoteToButton.Add(57, new ButtonData(A2Button, _whiteKeysBrush));
            _midiNoteToButton.Add(58, new ButtonData(As2Button, _blackKeyBrush));
            _midiNoteToButton.Add(59, new ButtonData(B2Button, _whiteKeysBrush));

            _midiNoteToButton.Add(60, new ButtonData(C3Button, _whiteKeysBrush));
            _midiNoteToButton.Add(61, new ButtonData(Cs3Button, _blackKeyBrush));
            _midiNoteToButton.Add(62, new ButtonData(D3Button, _whiteKeysBrush));
            _midiNoteToButton.Add(63, new ButtonData(Ds3Button, _blackKeyBrush));
            _midiNoteToButton.Add(64, new ButtonData(E3Button, _whiteKeysBrush));
            _midiNoteToButton.Add(65, new ButtonData(F3Button, _whiteKeysBrush));
            _midiNoteToButton.Add(66, new ButtonData(Fs3Button, _blackKeyBrush));
            _midiNoteToButton.Add(67, new ButtonData(G3Button, _whiteKeysBrush));
            _midiNoteToButton.Add(68, new ButtonData(Gs3Button, _blackKeyBrush));
            _midiNoteToButton.Add(69, new ButtonData(A3Button, _whiteKeysBrush));
            _midiNoteToButton.Add(70, new ButtonData(As3Button, _blackKeyBrush));
            _midiNoteToButton.Add(71, new ButtonData(B3Button, _whiteKeysBrush));

            _midiNoteToButton.Add(72, new ButtonData(C4Button, _whiteKeysBrush));
            _midiNoteToButton.Add(73, new ButtonData(Cs4Button, _blackKeyBrush));
            _midiNoteToButton.Add(74, new ButtonData(D4Button, _whiteKeysBrush));
            _midiNoteToButton.Add(75, new ButtonData(Ds4Button, _blackKeyBrush));
            _midiNoteToButton.Add(76, new ButtonData(E4Button, _whiteKeysBrush));
            _midiNoteToButton.Add(77, new ButtonData(F4Button, _whiteKeysBrush));
            _midiNoteToButton.Add(78, new ButtonData(Fs4Button, _blackKeyBrush));
            _midiNoteToButton.Add(79, new ButtonData(G4Button, _whiteKeysBrush));
            _midiNoteToButton.Add(80, new ButtonData(Gs4Button, _blackKeyBrush));
            _midiNoteToButton.Add(81, new ButtonData(A4Button, _whiteKeysBrush));
            _midiNoteToButton.Add(82, new ButtonData(As4Button, _blackKeyBrush));
            _midiNoteToButton.Add(83, new ButtonData(B4Button, _whiteKeysBrush));

            _midiNoteToButton.Add(84, new ButtonData(C5Button, _whiteKeysBrush));



            #endregion

            _startTime = DateTime.Now;
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(_timerInterval)
            };
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        internal class ButtonData(Button button, Brush brush)
        {
            public Button Button { get; set; } = button;
            public Brush ButtonColor { get; set; } = brush;
        }

        private void MidiProcessor_MidiDeviceNotFound()
        {
            Debug.WriteLine("MidiDeviceNotFound event received in MidiPlayWindow");
            Dispatcher.Invoke(() =>
            {
                NoMidiInText.Visibility = Visibility.Visible;
                Debug.WriteLine("NoMidiInText visibility set to Visible");
            });
        }

        private void MidiProcessor_NoteEvent(object sender, NotePlayedEventArgs e)
        {
            if (_midiNoteToButton.ContainsKey(e.NoteNumber))
            {
                if (e.IsOnMessage)
                {
                    // Colour key changes to defined colour for when it is pressed.
                    this.Dispatcher.Invoke(() => _midiNoteToButton[e.NoteNumber].Button.Background = _noteHitBrush);
                }
                else
                {
                    // When note is no longer pressed, change back to original colour.
                    this.Dispatcher.Invoke(() => _midiNoteToButton[e.NoteNumber].Button.Background = _midiNoteToButton[e.NoteNumber].ButtonColor);
                }
            }
        }

        // Code to process what happens each update tick.
        private void Timer_Tick(object sender, EventArgs e)
        {
            var currentTime = _midiProcessor.Stopwatch.Elapsed;
            foreach (var note in _midiProcessor.SongForNoteFalling.Notes)
            {
                if (!note.IsBlockGenerated && currentTime >= note.NoteStartTime && currentTime < note.NoteStartTime + note.NoteDuration)
                {
                    GenerateFallingBlock(note);
                    note.IsBlockGenerated = true;
                }
            }

            if (_playMidiFile)
            {
                foreach (var note in _midiProcessor.SongForNotePlayback.Notes)
                {
                    if (!note.IsPlaying && currentTime >= note.NoteStartTime &&
                        currentTime < note.NoteStartTime + note.NoteDuration)
                    {
                        _midiProcessor.MidiPlayer.PlayNote(note.NoteNumber);
                        note.IsPlaying = true;
                    }

                    if (note.IsPlaying && currentTime >= note.NoteStartTime + note.NoteDuration)
                    {
                        _midiProcessor.MidiPlayer.StopNote(note.NoteNumber);
                        note.IsPlaying = false;
                    }
                }
            }

            if (currentTime >= _endLastNote)
            {
                _songFinished = true;
                ReturnToSongList();
            }
        }

        // Returns to SongsWindow after song is done
        private void ReturnToSongList()
        {
            if (_midiProcessor != null)
            {
                _midiProcessor.OnSongFinished();
            }
            _timer.Stop();
            _midiProcessor.Name = Interaction.InputBox("Please enter your name:\nLeave empty or cancel to discard score", "Enter Name ",  "").Trim();
            if (_midiProcessor.Name.Length > 0)
            {
                _pianoHeroService.SaveHighscore(_songId, (int)_midiProcessor.Score, _currentScore, _midiProcessor.Name);
            }
            MessageBox.Show($"Score : {_midiProcessor.Score}");
            SongsWindow.SongIsFinished = true;
            Close();
            SongsWindow.Show();
        }

        // Returns to SongsWindow when closed and cleans up so no errors occur
        protected override void OnClosed(EventArgs e)
        {
            if (!_songFinished)
            {
                try
                {
                    // Dispose of midi processor or it wont open correctly when player starts another song
                    _midiProcessor?.Dispose();
                }
                catch (NullReferenceException ex)
                {
                    // When there's nothing to dispose, do nothing
                }
                SongsWindow.Show();
            }
        }

        private void GenerateFallingBlock(Note note)
        {
            /*
             * You'll notice sometimes a notenumber will have +36 or -35.
             * This is because the notenumber does not directly correspond to the grid number.
             * The midi keyboard we use (Impact GX49) starts at 36 with the first notenumber.
             * So this has to be corrected.
             */
            if (note.NoteNumber >= 36 && note.NoteNumber < NotesGrid.ColumnDefinitions.Count + 36) // Check if notes falls within grid, otherwise there'll be an exception
            {
                // Note width based on how wide the specific kolom is
                var actualWidthGrid = NotesGrid.ColumnDefinitions[note.NoteNumber - 36].ActualWidth;

                if (actualWidthGrid > 20) // Brown block for white key notes
                {
                    _fallingBlockBrush = Brushes.SaddleBrown;
                }
                else // Black block for black key notes
                {
                    _fallingBlockBrush = Brushes.Black;
                }

                Rectangle rect = new Rectangle
                {
                    Width = actualWidthGrid,
                    Height = _noteHeightPerSecond * note.NoteDuration.TotalSeconds,  // Calculate height of note with length it is pressed in song
                    Fill = _fallingBlockBrush // Gives the block a colour
                };

                // Calculate horizontal position of note based on the column it's in
                double leftPosition = GetLeftPositionForColumn(note.NoteNumber - 36);
                Canvas.SetLeft(rect, leftPosition);

                /*
                 * Position of the top of the note. It's set up so that every block is generated above the visible screen.
                 * This is done so the user can't see the notes being generated.
                 * This makes the experience smoother and gives a nicer impression.
                 */
                double startTopPosition = AnimationCanvas.ActualHeight;

                // Add block to the canvas
                AnimationCanvas.Children.Add(rect);

                double fallingDistance = AnimationCanvas.ActualHeight + (rect.Height);

                // Calculate how long it should take for block to reach the piano on screen
                _fallingDuration = fallingDistance / _fallingSpeed;

                // Endposition of the block so it's under the key when it disappears
                double endTopPosition = startTopPosition + rect.Height + 10;

                // Make animation
                DoubleAnimation animation = new DoubleAnimation
                {
                    From = -rect.Height,  // Start just above canvas
                    To = AnimationCanvas.ActualHeight,  // End at the bottom of the canvas
                    Duration = TimeSpan.FromSeconds(_fallingDuration)  // Constant duration for the animation
                };

                // Start the animation             
                rect.BeginAnimation(Canvas.TopProperty, animation);
            }
        }

        private double GetLeftPositionForColumn(int column)
        {
            double left = 0;
            for (int i = 0; i < column; i++)
            {
                left += NotesGrid.ColumnDefinitions[i].ActualWidth;
            }
            return left;
        }
    }
}