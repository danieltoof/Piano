using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.Multimedia;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace InEenNotendop.UI
{
    public partial class PlayWindow : Window
    {
        // Properties
        public Window Owner { get; set; }
        private SelectingWindow selectingWindow; // Reference to the selecting window
        private List<NoteBlock> _noteBlocks; // List to store information about note blocks
        private DispatcherTimer _timer; // Timer for updating the visual representation
        private double _time; // Current time in seconds
        private const int NoteWidth = 20; // Width of each note block

        // Constructor
        public PlayWindow(string FilePath, object sender)
        {
            InitializeComponent();
            selectingWindow = (SelectingWindow)sender; // Set the selecting window reference

            _noteBlocks = new List<NoteBlock>(); // Initialize the list of note blocks
            InitializePianoKeys(); // Initialize the piano keys grid

            Task.Run(async () =>
            {
                // Wait 2 seconds before starting playback
                await Task.Delay(2000);
                StartPlay(FilePath); // Start playback of the MIDI file
            });

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(30); // Set the timer interval
            _timer.Tick += Timer_Tick; // Set the event handler for the timer tick
            _timer.Start(); // Start the timer
        }

        // Initialize the piano keys grid
        private void InitializePianoKeys()
        {
            int whiteKeyCount = 28; // Number of white keys
            double keyWidth = PianoKeysGrid.ActualWidth / whiteKeyCount; // Calculate the width of each key

            for (int i = 0; i < 49; i++)
            {
                // Create a rectangle representing each piano key
                Rectangle key = new Rectangle
                {
                    Width = keyWidth,
                    Height = PianoKeysGrid.ActualHeight,
                    Fill = IsBlackKey(i) ? Brushes.Black : Brushes.White,
                    Stroke = Brushes.Black
                };

                if (IsBlackKey(i))
                {
                    // Adjust width and position for black keys
                    key.Width = keyWidth * 0.6;
                    key.Height = PianoKeysGrid.ActualHeight * 0.6;
                    Canvas.SetZIndex(key, 1); // Ensure black keys are on top
                    Canvas.SetLeft(key, i * keyWidth - keyWidth * 0.2); // Adjust for black key position
                }
                else
                {
                    Canvas.SetLeft(key, i * keyWidth); // Set position for white keys
                }

                PianoKeysGrid.Children.Add(key); // Add the key to the piano keys grid
            }
        }

        // Check if a key is black
        private bool IsBlackKey(int index)
        {
            int[] blackKeys = { 1, 3, 6, 8, 10, 13, 15, 18, 20, 22, 25, 27, 30, 32, 34, 37, 39, 42, 44, 46 };
            return Array.Exists(blackKeys, element => element == index % 12);
        }

        // Start playback of the MIDI file
        private void StartPlay(string MidiFileName)
        {
            var midiFile = MidiFile.Read(MidiFileName); // Read the MIDI file
            var tempoMap = midiFile.GetTempoMap(); // Get the tempo map of the MIDI file
            var notes = midiFile.GetNotes(); // Get all the notes in the MIDI file

            foreach (var note in notes)
            {
                // Convert note time to seconds and add to the list of note blocks
                double startTime = TimeConverter.ConvertTo<MetricTimeSpan>(note.Time, tempoMap).TotalMicroseconds / 1000000.0;
                _noteBlocks.Add(new NoteBlock
                {
                    NoteNumber = note.NoteNumber,
                    StartTime = startTime,
                    Speed = 300 // Adjust speed as needed
                });
            }

            try
            {
                // Start playback using the default MIDI output device
                using (var outputDevice = OutputDevice.GetByName("Microsoft GS Wavetable Synth"))
                using (var playback = midiFile.GetPlayback(outputDevice))
                {
                    playback.Play();
                }
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("File not found");
            }
        }

        // Event handler for the timer tick
        private void Timer_Tick(object sender, EventArgs e)
        {
            _time += 0.03; // Increase time (30 ms per tick)
            UpdateBlocks(); // Update the visual representation of note blocks
        }

        // Update the visual representation of note blocks
        private void UpdateBlocks()
        {
            // Update positions of existing note blocks
            for (int i = PianoCanvas.Children.Count - 1; i >= 0; i--)
            {
                if (PianoCanvas.Children[i] is Rectangle block)
                {
                    NoteBlock noteBlock = (NoteBlock)block.Tag;
                    double newTop = Canvas.GetTop(block) + noteBlock.Speed * 0.03;
                    Canvas.SetTop(block, newTop);

                    // Remove note blocks that have reached the bottom of the grid
                    if (newTop >= PianoCanvas.ActualHeight)
                    {
                        PianoCanvas.Children.RemoveAt(i);
                    }

                    else
                    {
                        // inladen midi
                        var midi = MidiFile.Read(MidiFileName);

                        using (var output = OutputDevice.GetByName("Microsoft GS Wavetable Synth"))
                        //using (var playback = midi.GetPlayback(output))
                        {
                            // wacht 2 seconden zodat de midi niet direct begint met spelen
                            Thread.Sleep(2000);
                            // run async thread zodat programma niet lockt tot de midi klaar is
                        
                            midi.Play(output);
                        }
                }
            }

            // Add new note blocks that are due to start falling
            foreach (var noteBlock in _noteBlocks.Where(nb => _time >= nb.StartTime && !nb.IsFalling))
            {
                Rectangle block = new Rectangle
                {
                    Width = NoteWidth,
                    Height = 20,
                    Fill = Brushes.Blue,
                    Tag = noteBlock
                };

                double left = (noteBlock.NoteNumber - 21) * NoteWidth; // Adjust position based on note number
                Canvas.SetLeft(block, left);
                Canvas.SetTop(block, 0);

                PianoCanvas.Children.Add(block);
                noteBlock.IsFalling = true;
            }
        }
    }

    // Class representing a note block
    public class NoteBlock
    {
        public int NoteNumber { get; set; } // MIDI note number
        public double StartTime { get; set; } // Start time of the note in seconds
        public double Speed { get; set; } // Speed of the falling note
        public bool IsFalling { get; set; } = false; // Flag indicating if the note is currently falling
    }
}

