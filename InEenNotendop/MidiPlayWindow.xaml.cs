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
        private MidiFile _midiFileSong;
        private TimeSpan _endLastNote;


        private SqlDataAccess _sqlDataAccess = new();
        private int _nummerId;

        private string? _desiredOutDevice { get; set; }
        //Kleurtjes van keys
        private Brush _noteHitBrush = Brushes.IndianRed; // Wanneer key wordt aangeslagen
        private Brush _whiteKeysBrush = Brushes.WhiteSmoke; // Witte toetsen
        private Brush _blackKeyBrush = Brushes.Black; // Zwartse toetsen
        private Brush _fallingBlockBrush = Brushes.Red;
        private Dictionary<int, ButtonData> _midiNoteToButton = new(); // int = Midi notonumber, Button = button die wordt toegewezen aan die noot.

        private DateTime _startTime; // Deze hebben we nodog om de tijd te berekenen van wanneer de midi noot is gespeeld, 
        private object _value;
        private DispatcherTimer _timer;
        private const double _noteHeightPerSecond = 200; // Eenheid voor hoogte blok per seconde
        private const double _fallingSpeed = 200.0; // Speed of falling blocks in units per second

        private const double _timerInterval = 10; // in MS, hoe lager des te accurater de de code checkt wanneer een blok gegenereerd een een note gespeeld moet worden, maar kan meer performance kosten

        private double _fallingDuration;

        private List<Note> _notesOfSongList = [];
        private List<Note> _notesOfSongToBePlayed = [];

        public class ButtonData(Button button, Brush brush)
        {
            public Button Button { get; set; } = button;
            public System.Windows.Media.Brush ButtonColor { get; set; } = brush;
        }

        public MidiPlayWindow(string filePath, object sender, bool playMidiFile, int nummerId, SongsWindow? songsWindow, int currentScore)
        {
            _nummerId = nummerId;
            SongsWindow = songsWindow;
            _playMidiFile = playMidiFile;
            _currentScore = currentScore;

            try
            {
                _midiProcessor = new MidiProcessor(this, new MidiFile(filePath));
                _midiProcessor.NotePlayed += MidiProcessor_NotePlayed;
                _midiProcessor.MidiDeviceNotFound += MidiProcessor_MidiDeviceNotFound;
            }
            catch (FileNotFoundException e)
            {
                MessageBox.Show(e.Message);
                Close();
            }

            if (playMidiFile)
            {

            }

            try
            {
                _endLastNote = _midiProcessor.SongForNotePlayback.Notes[_midiProcessor.SongForNotePlayback.Notes.Count - 1].NoteStartTime + _midiProcessor.SongForNotePlayback.Notes[_midiProcessor.SongForNotePlayback.Notes.Count - 1].NoteDuration;
                _endLastNote += TimeSpan.FromSeconds(3);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                Close();
            }


            InitializeComponent();


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

        private void MidiProcessor_MidiDeviceNotFound(object sender)
        {
            MessageBox.Show("Invalid MIDI device index or no devices found.");
        }

        private void MidiProcessor_NotePlayed(object sender, NotePlayedEventArgs e)
        {
            if (_midiNoteToButton.ContainsKey(e.NoteNumber))
            {
                if (e.IsOnMessage)
                {
                    // kleur toets veranderen naar gedefinieerde kleur voor wanneer note gespeeld wordt
                    this.Dispatcher.Invoke(() => _midiNoteToButton[e.NoteNumber].Button.Background = _noteHitBrush);
                }
                else
                {
                    // als noot uit gaat dan terugveranderen naar originele kleur
                    this.Dispatcher.Invoke(() => _midiNoteToButton[e.NoteNumber].Button.Background = _midiNoteToButton[e.NoteNumber].ButtonColor);
                }
            }
        }

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

        // (Important) Dispose of the MidiIn object when the app closes
        private void ReturnToSongList()
        {
            if (_midiProcessor != null)
            {
                _midiProcessor.OnSongFinished();
            }
            _timer.Stop();
            _sqlDataAccess.ChangeHighscore(_nummerId, (int)_midiProcessor.Score, _currentScore);
            MessageBox.Show($"Score : {_midiProcessor.Score}");
            SongsWindow.SongIsFinished = true;
            Close();
            SongsWindow.Show();
        }
        protected override void OnClosed(EventArgs e)
        {
            if (!_songFinished)
            {
                try
                {
                    _midiProcessor.Dispose();

                }
                catch (NullReferenceException ex)
                {
                    // niks om te disposen dus hoeft niks te doen
                }
                SongsWindow.Show();
            }
        }

        private void GenerateFallingBlock(Note note)
        {
            // het zal wellicht opvallen dat soms bij een notenummer +36 of -35 staat. Dat komt omdat de nootnummer niet direct correspondeerd aan het grid nummer.
            // mijn midi keyboard (Impact GX49) begint namelijk bij 36 als eerste nootnummer. vandaar dat dit gecorrigeerd wordt.
            if (note.NoteNumber >= 36 && note.NoteNumber < NotesGrid.ColumnDefinitions.Count + 36) // noten moeten binnen de range vallen anders krijgen we exceptions
            {
                // noot breedte op basis van hoe breed de specifieke kolom is.
                var actualWidthGrid = NotesGrid.ColumnDefinitions[note.NoteNumber - 36].ActualWidth;

                if (actualWidthGrid > 20) // Als noot witte toets is, bruin blokje
                {
                    _fallingBlockBrush = Brushes.SaddleBrown;
                }
                else // anders zwarte toets, zwart blokje
                {
                    _fallingBlockBrush = Brushes.Black;
                }

                Rectangle rect = new Rectangle
                {
                    Width = actualWidthGrid,
                    Height = _noteHeightPerSecond * note.NoteDuration.TotalSeconds,  // hoogte van noot is varierend, om het accuraat te maken gebruiken we deze formule
                    Fill = _fallingBlockBrush // kleurtje
                };

                // horizontale positie bepalen van noot op basis van het kolomnummer
                double leftPosition = GetLeftPositionForColumn(note.NoteNumber - 36);
                Canvas.SetLeft(rect, leftPosition);

                // positie van top van blokje. hij is zo ingesteld dat elke blok boven het zichtbare scherm wordt gegenereerd. Dit zodat de gebruiker niet kan zien dat
                // de noten gegenereerd worden. dit geeft een mooiere indruk en een gestroomlijnder ervaring.
                double startTopPosition = AnimationCanvas.ActualHeight;

                // blokje toevoegen aan canvas
                AnimationCanvas.Children.Add(rect);

                double fallingDistance = AnimationCanvas.ActualHeight + (rect.Height);

                // hoe lang duur thet voordat blokje de piano moet bereiken?
                _fallingDuration = fallingDistance / _fallingSpeed;

                // eindpositie van het blokkje zodat het onder de pianotoetsen terecht komt en niet meer zichtbaar is.
                double endTopPosition = startTopPosition + rect.Height + 10;

                // animatie aanmekn
                DoubleAnimation animation = new DoubleAnimation
                {
                    From = -rect.Height,  // net boven het canvas beginnen
                    To = AnimationCanvas.ActualHeight,  // eindigen op de bodem van canvas
                    Duration = TimeSpan.FromSeconds(_fallingDuration)  // Constant duration for the animation
                };

                // starten van de animatie              
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