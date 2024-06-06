﻿using System.Diagnostics;
using NAudio.Midi;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using PianoHero.Business;
using System.Windows.Media.Animation;
using System.IO;
using PianoHero.Data;

namespace PianoHero.UI
{
    /// <summary>
    /// Interaction logic for MidiPlayWindow.xaml
    /// </summary>
    public partial class MidiPlayWindow : Window
    {
        public SongsWindow songsWindow { get; set; }

        private bool playMidiFile = false;
        private bool songFinished = false;
        private int currentScore;

        private MidiIn midiIn;
        private MidiPlayer midiPlayer;
        private MidiFile midiFileSong;
        private TimeSpan endLastNote;

        DataProgram dataProgram = new DataProgram();
        private int nummerID;

        private string? desiredOutDevice { get; set; }
        //Kleurtjes van keys
        private System.Windows.Media.Brush noteHitBrush = Brushes.IndianRed; // Wanneer key wordt aangeslagen
        private System.Windows.Media.Brush whiteKeysBrush = Brushes.WhiteSmoke; // Witte toetsen
        private System.Windows.Media.Brush blackKeyBrush = Brushes.Black; // Zwartse toetsen
        private System.Windows.Media.Brush fallingBlockBrush = Brushes.Red;
        private Dictionary<int, ButtonData> midiNoteToButton = new(); // int = Midi notonumber, Button = button die wordt toegewezen aan die noot.

        private MidiInputProcessor midiInputProcessor; // Klasse nodig om midi noten op te slaan in List en score te berekenen
        private MidiInputScoreCalculator midiInputScoreCalculator;
        private DateTime _startTime; // Deze hebben we nodog om de tijd te berekenen van wanneer de midi noot is gespeeld, 
        private object value;
        private DispatcherTimer timer;
        private Stopwatch stopwatch; // Acurater dan DateTime.Now
        private const double NoteHeightPerSecond = 200; // Eenheid voor hoogte blok per seconde
        //private const double FallingDuration = 1.5; // Duration of the falling animation for all notes (in seconds)
        private const double FallingSpeed = 200.0; // Speed of falling blocks in units per second

        private const double TimerInterval = 10; // in MS, hoe lager des te accurater de de code checkt wanneer een blok gegenereerd een een note gespeeld moet worden, maar kan meer performance kosten

        private double fallingDuration;

        private List<Note> notesOfSongList = [];
        private List<Note> notesOfSongToBePlayed = [];

        public class ButtonData(Button button, Brush brush)
        {
            public Button Button { get; set; } = button;
            public System.Windows.Media.Brush ButtonColor { get; set; } = brush;
        }

        public MidiPlayWindow(string FilePath, object sender, bool playMidiFile, int nummerID, SongsWindow? songsWindow, int currentScore)
        {
            this.nummerID = nummerID;
            this.songsWindow = songsWindow;
            this.playMidiFile = playMidiFile;
            this.currentScore = currentScore;
            midiInputProcessor = new MidiInputProcessor();

            stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {

                midiFileSong = new MidiFile(FilePath);

            } catch (FileNotFoundException e)
            {
                MessageBox.Show(e.Message);
            }
            
            TimeSpan increment = TimeSpan.FromMilliseconds(2000); // Dit voegt een delay toe aan noten genereren. Ander hebben we een bug met teveel noten weergeven aan het begin
            TimeSpan increment2 = TimeSpan.FromMilliseconds(7800); // Dit voegt een delay toe aan get afspelen van midi noten, correspondeerd met vanaf waar de noten gegenereerd worden tot aan pianotoetsen op scherm

            notesOfSongList = midiInputProcessor.MidiToList(midiFileSong);

            for (int i = 0; i < notesOfSongList.Count; i++) // Een delay toegevoegd aan de midi, omdat anders de eerste paar blokken niet goed gerenderd worden.
            {
                notesOfSongList[i].NoteStartTime = notesOfSongList[i].NoteStartTime.Add(increment);
            }

            notesOfSongToBePlayed = [.. midiInputProcessor.MidiToList(midiFileSong)]; // shallow koepie

            if(playMidiFile) { 
                for (int i = 0; i < notesOfSongToBePlayed.Count; i++) // Nog een keer delay toevoegen op de liste met noten die gebruikt worden voor de midi playback
                {
                    notesOfSongToBePlayed[i].NoteStartTime = notesOfSongToBePlayed[i].NoteStartTime.Add(increment2);
                }
            }

            try
            {
                endLastNote = notesOfSongToBePlayed[notesOfSongToBePlayed.Count - 1].NoteStartTime + notesOfSongToBePlayed[notesOfSongToBePlayed.Count - 1].NoteDuration;
                endLastNote = endLastNote + TimeSpan.FromSeconds(10);
            } 
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                Close();
            }

            desiredOutDevice = "Microsoft GS Wavetable Synth";

            InitializeComponent();
            InitializeMidi(desiredOutDevice);

            midiInputScoreCalculator = new MidiInputScoreCalculator(midiInputProcessor);

            #region mapping midi notes to the buttons


            midiNoteToButton.Add(36, new ButtonData(C1Button, whiteKeysBrush));
            midiNoteToButton.Add(37, new ButtonData(Cs1Button, blackKeyBrush));
            midiNoteToButton.Add(38, new ButtonData(D1Button, whiteKeysBrush));
            midiNoteToButton.Add(39, new ButtonData(Ds1Button, blackKeyBrush));
            midiNoteToButton.Add(40, new ButtonData(E1Button, whiteKeysBrush));
            midiNoteToButton.Add(41, new ButtonData(F1Button, whiteKeysBrush));
            midiNoteToButton.Add(42, new ButtonData(Fs1Button, blackKeyBrush));
            midiNoteToButton.Add(43, new ButtonData(G1Button, whiteKeysBrush));
            midiNoteToButton.Add(44, new ButtonData(Gs1Button, blackKeyBrush));
            midiNoteToButton.Add(45, new ButtonData(A1Button, whiteKeysBrush));
            midiNoteToButton.Add(46, new ButtonData(As1Button, blackKeyBrush));
            midiNoteToButton.Add(47, new ButtonData(B1Button, whiteKeysBrush));

            midiNoteToButton.Add(48, new ButtonData(C2Button, whiteKeysBrush));
            midiNoteToButton.Add(49, new ButtonData(Cs2Button, blackKeyBrush));
            midiNoteToButton.Add(50, new ButtonData(D2Button, whiteKeysBrush));
            midiNoteToButton.Add(51, new ButtonData(Ds2Button, blackKeyBrush));
            midiNoteToButton.Add(52, new ButtonData(E2Button, whiteKeysBrush));
            midiNoteToButton.Add(53, new ButtonData(F2Button, whiteKeysBrush));
            midiNoteToButton.Add(54, new ButtonData(Fs2Button, blackKeyBrush));
            midiNoteToButton.Add(55, new ButtonData(G2Button, whiteKeysBrush));
            midiNoteToButton.Add(56, new ButtonData(Gs2Button, blackKeyBrush));
            midiNoteToButton.Add(57, new ButtonData(A2Button, whiteKeysBrush));
            midiNoteToButton.Add(58, new ButtonData(As2Button, blackKeyBrush));
            midiNoteToButton.Add(59, new ButtonData(B2Button, whiteKeysBrush));

            midiNoteToButton.Add(60, new ButtonData(C3Button, whiteKeysBrush));
            midiNoteToButton.Add(61, new ButtonData(Cs3Button, blackKeyBrush));
            midiNoteToButton.Add(62, new ButtonData(D3Button, whiteKeysBrush));
            midiNoteToButton.Add(63, new ButtonData(Ds3Button, blackKeyBrush));
            midiNoteToButton.Add(64, new ButtonData(E3Button, whiteKeysBrush));
            midiNoteToButton.Add(65, new ButtonData(F3Button, whiteKeysBrush));
            midiNoteToButton.Add(66, new ButtonData(Fs3Button, blackKeyBrush));
            midiNoteToButton.Add(67, new ButtonData(G3Button, whiteKeysBrush));
            midiNoteToButton.Add(68, new ButtonData(Gs3Button, blackKeyBrush));
            midiNoteToButton.Add(69, new ButtonData(A3Button, whiteKeysBrush));
            midiNoteToButton.Add(70, new ButtonData(As3Button, blackKeyBrush));
            midiNoteToButton.Add(71, new ButtonData(B3Button, whiteKeysBrush));

            midiNoteToButton.Add(72, new ButtonData(C4Button, whiteKeysBrush));
            midiNoteToButton.Add(73, new ButtonData(Cs4Button, blackKeyBrush));
            midiNoteToButton.Add(74, new ButtonData(D4Button, whiteKeysBrush));
            midiNoteToButton.Add(75, new ButtonData(Ds4Button, blackKeyBrush));
            midiNoteToButton.Add(76, new ButtonData(E4Button, whiteKeysBrush));
            midiNoteToButton.Add(77, new ButtonData(F4Button, whiteKeysBrush));
            midiNoteToButton.Add(78, new ButtonData(Fs4Button, blackKeyBrush));
            midiNoteToButton.Add(79, new ButtonData(G4Button, whiteKeysBrush));
            midiNoteToButton.Add(80, new ButtonData(Gs4Button, blackKeyBrush));
            midiNoteToButton.Add(81, new ButtonData(A4Button, whiteKeysBrush));
            midiNoteToButton.Add(82, new ButtonData(As4Button, blackKeyBrush));
            midiNoteToButton.Add(83, new ButtonData(B4Button, whiteKeysBrush));

            midiNoteToButton.Add(84, new ButtonData(C5Button, whiteKeysBrush));



            #endregion

            _startTime = DateTime.Now;
            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(TimerInterval)
            };
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            var currentTime = stopwatch.Elapsed;
            foreach (var note in notesOfSongList)
            {
                if (!note.IsBlockGenerated && currentTime >= note.NoteStartTime && currentTime < note.NoteStartTime + note.NoteDuration)
                {
                    GenerateFallingBlock(note);
                    note.IsBlockGenerated = true;
                }
            }

            if(playMidiFile) { 
                foreach (var note in notesOfSongToBePlayed)
                {
                    if (!note.IsPlaying && currentTime >= note.NoteStartTime &&
                        currentTime < note.NoteStartTime + note.NoteDuration)
                    {
                        midiPlayer.PlayNote(note.NoteNumber);
                        note.IsPlaying = true;
                    }

                    if (note.IsPlaying && currentTime >= note.NoteStartTime + note.NoteDuration)
                    {
                        midiPlayer.StopNote(note.NoteNumber);
                        note.IsPlaying = false;
                    }
                }
            }

            if(currentTime >= endLastNote)
            {
                songFinished = true;
                ReturnToSongList();
            }
        }

        // (Important) Dispose of the MidiIn object when the app closes
        private void ReturnToSongList()
        {
            if (midiIn != null)
            {
                midiIn.Stop();
                midiIn.Dispose();
            }
            timer.Stop();
            midiPlayer.Dispose();
            midiInputScoreCalculator.CalculateScoreAfterSongCompleted();
            dataProgram.ChangeHighscore(nummerID, (int)midiInputScoreCalculator.Score, currentScore);
            MessageBox.Show($"Score : {midiInputScoreCalculator.Score}");
            songsWindow.songIsFinished = true;
            Close();
            songsWindow.Show();
        }

        //Checks if midi device is connected so the application doesn't crash
        protected override void OnClosed(EventArgs e)
        {
            if (!songFinished)
            {
                if (midiIn != null)
                {
                    midiIn.Stop();
                    midiIn.Dispose();
                }
                if(timer != null)
                {
                    timer.Stop();
                }
                if(midiPlayer != null)
                {
                    midiPlayer.Dispose();
                }
                songsWindow.Show();
            }
        }

        private void InitializeMidi(string desiredOutDevice)
        {
            midiPlayer = new MidiPlayer(desiredOutDevice);

            var numDevices = MidiIn.NumberOfDevices;
            for (int i = 0; i < numDevices; i++)
            {
                Debug.WriteLine($"Midi In Device {i}: {MidiIn.DeviceInfo(i)}");
            }
            var desiredDeviceIndex = 0; // DEZE KAN VERANDEREN SOMS SPONTAAN
            if (desiredDeviceIndex < numDevices)
            {
                midiIn = new MidiIn(desiredDeviceIndex);
                midiIn.MessageReceived += MidiIn_MessageReceived;
                midiIn.Start();
            }
            else
            {
                MessageBox.Show("Invalid MIDI device index or no devices found.");
            }
        }

        private void MidiIn_MessageReceived(object? sender, MidiInMessageEventArgs e)
        {
            if (e.MidiEvent is NoteOnEvent noteOnEvent)
            {
                if (midiNoteToButton.ContainsKey(noteOnEvent.NoteNumber))
                {
                    midiNoteToButton[noteOnEvent.NoteNumber].Button.Dispatcher.Invoke(() =>
                    {
                        // kleur toets veranderen
                        midiNoteToButton[noteOnEvent.NoteNumber].Button.Background = noteHitBrush;
                        // noot beginnnen met spelen
                        midiPlayer.PlayNote(noteOnEvent.NoteNumber);

                        // Tijd berekenen sinds start spelen
                        TimeSpan startTimeNotePlayed = stopwatch.Elapsed;
                        // Noot toevoegen aan list in midiInputProcessor's list voor score berekening
                        midiInputProcessor.ListOfNotesPlayed.Add(new Note(noteOnEvent, startTimeNotePlayed));
                    });
                }
            }
            else if (e.MidiEvent is NoteEvent noteEvent) // een noteevent wat geen noteonevent is is in dit geval altijd een event die een noot eindigt.
            {
                if (midiNoteToButton.ContainsKey(noteEvent.NoteNumber))
                {
                    //noot stoppen met spelen
                    midiPlayer.StopNote(noteEvent.NoteNumber);
                    // toets terugveranderen naar originele kleur
                    midiNoteToButton[noteEvent.NoteNumber].Button.Dispatcher.Invoke(() =>
                    {
                        midiNoteToButton[noteEvent.NoteNumber].Button.Background = midiNoteToButton[noteEvent.NoteNumber].ButtonColor;
                    });
                }
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
                    fallingBlockBrush = Brushes.SaddleBrown;
                }
                else // anders zwarte toets, zwart blokje
                {
                    fallingBlockBrush = Brushes.Black;
                }

                Rectangle rect = new Rectangle
                {
                    Width = actualWidthGrid,  
                    Height = NoteHeightPerSecond * note.NoteDuration.TotalSeconds,  // hoogte van noot is varierend, om het accuraat te maken gebruiken we deze formule
                    Fill = fallingBlockBrush // kleurtje
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
                fallingDuration = fallingDistance / FallingSpeed;

                // eindpositie van het blokkje zodat het onder de pianotoetsen terecht komt en niet meer zichtbaar is.
                double endTopPosition = startTopPosition + rect.Height + 10; 

                // animatie aanmekn
                DoubleAnimation animation = new DoubleAnimation
                {
                    From = -rect.Height,  // net boven het canvas beginnen
                    To = AnimationCanvas.ActualHeight,  // eindigen op de bodem van canvas
                    Duration = TimeSpan.FromSeconds(fallingDuration)  // Constant duration for the animation
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