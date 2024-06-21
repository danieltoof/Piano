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
        // Delegate
        public delegate NotePlayedEventArgs UIChangeMessageHandler(object sender, int NoteNumber, bool IsNoteOnEvent);

        // Event
        public event UIChangeMessageHandler UIChangeMessageReceived;



        //
        public SongsWindow songsWindow { get; set; }

        private bool _playMidiFile = false;
        private bool _songFinished = false;
        private int _currentScore;

        private MidiProcessor midiProcessor;
        //private MidiFile midiFileSong;
        private TimeSpan endLastNote;

        private SqlDataAccess _sqlDataAccess = new();
        private int _nummerId;

        private string? _desiredOutDevice { get; set; }
        //Kleurtjes van keys
        private System.Windows.Media.Brush noteHitBrush = Brushes.IndianRed; // Wanneer key wordt aangeslagen
        private System.Windows.Media.Brush whiteKeysBrush = Brushes.WhiteSmoke; // Witte toetsen
        private System.Windows.Media.Brush blackKeyBrush = Brushes.Black; // Zwartse toetsen
        private System.Windows.Media.Brush fallingBlockBrush;
        public Dictionary<int, ButtonData> MidiNoteToButton { get; set; } // int = Midi notonumber, Button = button die wordt toegewezen aan die noot.

        private DateTime startTime; // Deze hebben we nodog om de tijd te berekenen van wanneer de midi noot is gespeeld, 
        private object value;
        private DispatcherTimer timer;
        private const double NoteHeightPerSecond = 200; // Eenheid voor hoogte blok per seconde
        private const double FallingSpeed = 200.0; // Speed of falling blocks in units per second
        private const double TimerInterval = 10; // in MS, hoe lager des te accurater de de code checkt wanneer een blok gegenereerd een een note gespeeld moet worden, maar kan meer performance kosten

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
            this.nummerID = nummerID;
            this.songsWindow = songsWindow;
            this.playMidiFile = playMidiFile;
            this.currentScore = currentScore;
            MidiNoteToButton = [];

            try
            {

                MidiFile midiFileSong = new MidiFile(FilePath);
                midiProcessor = new MidiProcessor(this, midiFileSong);
                midiProcessor.NotePlayed += MidiProcessor_NotePlayed;

            }
            catch (FileNotFoundException e)
            {
                MessageBox.Show(e.Message);
                Close();
            }

            midiProcessor.MidiDeviceNotFound += MidiProcessor_MidiDeviceNotFound    ;


     
            
            //notesOfSongList = MidiToListConverter.MidiToList(midiFileSong);



            if(playMidiFile) { 

            }

            try
            {
                endLastNote = midiProcessor.SongForNotePlayback.Notes[midiProcessor.SongForNotePlayback.Notes.Count - 1].NoteStartTime + midiProcessor.SongForNotePlayback.Notes[midiProcessor.SongForNotePlayback.Notes.Count - 1].NoteDuration;
                endLastNote += TimeSpan.FromSeconds(3);
            } 
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                Close();
            }


            InitializeComponent();


            #region mapping midi notes to the buttons


            MidiNoteToButton.Add(36, new ButtonData(C1Button, whiteKeysBrush));
            MidiNoteToButton.Add(37, new ButtonData(Cs1Button, blackKeyBrush));
            MidiNoteToButton.Add(38, new ButtonData(D1Button, whiteKeysBrush));
            MidiNoteToButton.Add(39, new ButtonData(Ds1Button, blackKeyBrush));
            MidiNoteToButton.Add(40, new ButtonData(E1Button, whiteKeysBrush));
            MidiNoteToButton.Add(41, new ButtonData(F1Button, whiteKeysBrush));
            MidiNoteToButton.Add(42, new ButtonData(Fs1Button, blackKeyBrush));
            MidiNoteToButton.Add(43, new ButtonData(G1Button, whiteKeysBrush));
            MidiNoteToButton.Add(44, new ButtonData(Gs1Button, blackKeyBrush));
            MidiNoteToButton.Add(45, new ButtonData(A1Button, whiteKeysBrush));
            MidiNoteToButton.Add(46, new ButtonData(As1Button, blackKeyBrush));
            MidiNoteToButton.Add(47, new ButtonData(B1Button, whiteKeysBrush));

            MidiNoteToButton.Add(48, new ButtonData(C2Button, whiteKeysBrush));
            MidiNoteToButton.Add(49, new ButtonData(Cs2Button, blackKeyBrush));
            MidiNoteToButton.Add(50, new ButtonData(D2Button, whiteKeysBrush));
            MidiNoteToButton.Add(51, new ButtonData(Ds2Button, blackKeyBrush));
            MidiNoteToButton.Add(52, new ButtonData(E2Button, whiteKeysBrush));
            MidiNoteToButton.Add(53, new ButtonData(F2Button, whiteKeysBrush));
            MidiNoteToButton.Add(54, new ButtonData(Fs2Button, blackKeyBrush));
            MidiNoteToButton.Add(55, new ButtonData(G2Button, whiteKeysBrush));
            MidiNoteToButton.Add(56, new ButtonData(Gs2Button, blackKeyBrush));
            MidiNoteToButton.Add(57, new ButtonData(A2Button, whiteKeysBrush));
            MidiNoteToButton.Add(58, new ButtonData(As2Button, blackKeyBrush));
            MidiNoteToButton.Add(59, new ButtonData(B2Button, whiteKeysBrush));

            MidiNoteToButton.Add(60, new ButtonData(C3Button, whiteKeysBrush));
            MidiNoteToButton.Add(61, new ButtonData(Cs3Button, blackKeyBrush));
            MidiNoteToButton.Add(62, new ButtonData(D3Button, whiteKeysBrush));
            MidiNoteToButton.Add(63, new ButtonData(Ds3Button, blackKeyBrush));
            MidiNoteToButton.Add(64, new ButtonData(E3Button, whiteKeysBrush));
            MidiNoteToButton.Add(65, new ButtonData(F3Button, whiteKeysBrush));
            MidiNoteToButton.Add(66, new ButtonData(Fs3Button, blackKeyBrush));
            MidiNoteToButton.Add(67, new ButtonData(G3Button, whiteKeysBrush));
            MidiNoteToButton.Add(68, new ButtonData(Gs3Button, blackKeyBrush));
            MidiNoteToButton.Add(69, new ButtonData(A3Button, whiteKeysBrush));
            MidiNoteToButton.Add(70, new ButtonData(As3Button, blackKeyBrush));
            MidiNoteToButton.Add(71, new ButtonData(B3Button, whiteKeysBrush));

            MidiNoteToButton.Add(72, new ButtonData(C4Button, whiteKeysBrush));
            MidiNoteToButton.Add(73, new ButtonData(Cs4Button, blackKeyBrush));
            MidiNoteToButton.Add(74, new ButtonData(D4Button, whiteKeysBrush));
            MidiNoteToButton.Add(75, new ButtonData(Ds4Button, blackKeyBrush));
            MidiNoteToButton.Add(76, new ButtonData(E4Button, whiteKeysBrush));
            MidiNoteToButton.Add(77, new ButtonData(F4Button, whiteKeysBrush));
            MidiNoteToButton.Add(78, new ButtonData(Fs4Button, blackKeyBrush));
            MidiNoteToButton.Add(79, new ButtonData(G4Button, whiteKeysBrush));
            MidiNoteToButton.Add(80, new ButtonData(Gs4Button, blackKeyBrush));
            MidiNoteToButton.Add(81, new ButtonData(A4Button, whiteKeysBrush));
            MidiNoteToButton.Add(82, new ButtonData(As4Button, blackKeyBrush));
            MidiNoteToButton.Add(83, new ButtonData(B4Button, whiteKeysBrush));

            MidiNoteToButton.Add(84, new ButtonData(C5Button, whiteKeysBrush));



            #endregion

            startTime = DateTime.Now;
            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(TimerInterval)
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
            if (MidiNoteToButton.ContainsKey(e.noteNumber))
            {
                if (e.isOnMessage)
                {
                    // kleur toets veranderen naar gedefinieerde kleur voor wanneer note gespeeld wordt
                    this.Dispatcher.Invoke(() => MidiNoteToButton[e.noteNumber].Button.Background = noteHitBrush);
                }
                else
                {
                    // als noot uit gaat dan terugveranderen naar originele kleur
                    this.Dispatcher.Invoke(() => MidiNoteToButton[e.noteNumber].Button.Background = MidiNoteToButton[e.noteNumber].ButtonColor);
                }
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            var currentTime = midiProcessor.Stopwatch.Elapsed;
            foreach (var note in midiProcessor.SongForNoteFalling.Notes)
            {
                if (!note.IsBlockGenerated && currentTime >= note.NoteStartTime && currentTime < note.NoteStartTime + note.NoteDuration)
                {
                    GenerateFallingBlock(note);
                    note.IsBlockGenerated = true;
                }
            }

            if(playMidiFile) { 
                foreach (var note in midiProcessor.SongForNotePlayback.Notes)
                {
                    if (!note.IsPlaying && currentTime >= note.NoteStartTime &&
                        currentTime < note.NoteStartTime + note.NoteDuration)
                    {
                        midiProcessor.MidiPlayer.PlayNote(note.NoteNumber);
                        note.IsPlaying = true;
                    }

                    if (note.IsPlaying && currentTime >= note.NoteStartTime + note.NoteDuration)
                    {
                        midiProcessor.MidiPlayer.StopNote(note.NoteNumber);
                        note.IsPlaying = false;
                    }
                }
            }

            if(currentTime >= _endLastNote)
            {
                _songFinished = true;
                ReturnToSongList();
            }
        }

        // (Important) Dispose of the MidiIn object when the app closes
        private void ReturnToSongList()
        {
            if (midiProcessor != null)
            {
                // deze functie doet dispose en score berekenen
                midiProcessor.OnSongFinished();
            }
            timer.Stop();
            dataProgram.ChangeHighscore(nummerID, (int)midiProcessor.Score, currentScore);
            MessageBox.Show($"Score : {midiProcessor.Score}");
            songsWindow.songIsFinished = true;
            Close();
            SongsWindow.Show();
        }

        //Checks if midi device is connected so the application doesn't crash

        //private void OnMidiInputReceived(object sender, NotePlayedEventArgs e)
        //{
        //    if(MidiNoteToButton.ContainsKey(e.noteNumber))
        //    {
        //        if(e.isOnMessage)
        //        {
        //            // kleur toets veranderen naar gedefinieerde kleur voor wanneer note gespeeld wordt
        //            MidiNoteToButton[e.noteNumber].Button.Background = noteHitBrush;
        //        }
        //        else
        //        {
        //            // als noot uit gaat dan terugveranderen naar originele kleur
        //            MidiNoteToButton[e.noteNumber].Button.Background = MidiNoteToButton[e.noteNumber].ButtonColor;
        //        }
        //    }
        //}
        protected override void OnClosed(EventArgs e)
        {
            if (!_songFinished)
            {
                try
                {
                    midiProcessor.Dispose();

                }
                catch (NullReferenceException ex)
                {
                    // niks om te disposen dus hoeft niks te doen
                }
                songsWindow.Show();
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
                    Height = NoteHeightPerSecond * note.NoteDuration.TotalSeconds,  // hoogte van noot is varierend, om het accuraat te maken gebruiken we deze formule
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
                _fallingDuration = fallingDistance / FallingSpeed;

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