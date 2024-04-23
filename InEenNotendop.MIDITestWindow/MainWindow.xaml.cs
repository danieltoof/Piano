using NAudio.Midi;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace InEenNotendop.MIDITestWindow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MidiIn midiIn;
        private List<ActiveMidiNote> activeMidiNotes = new List<ActiveMidiNote>();
        private System.Windows.Media.Brush originalButtonColor;
        public MainWindow()
        {
            InitializeComponent();
            originalButtonColor = midiButton.Background; // Store original color
            InitializeMidi();
        }

        private void InitializeMidi()
        {
            int numDevices = MidiIn.NumberOfDevices;
            int desiredDeviceIndex = 1; // Replace with the index you want
            if (desiredDeviceIndex >= 0 && desiredDeviceIndex < numDevices)
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

        private void MidiIn_MessageReceived(object sender, MidiInMessageEventArgs e)
        {
            // Change the button color
            {
                if (e.MidiEvent is NoteOnEvent noteOnEvent)
                {
                    activeMidiNotes.Add(new ActiveMidiNote() { NoteNumber = noteOnEvent.NoteNumber, IsPressed = true });
                }
                else if (e.MidiEvent is NoteEvent noteEvent && noteEvent.Velocity == 0)
                {
                    var noteToRelease = activeMidiNotes.FirstOrDefault(note => note.NoteNumber == noteEvent.NoteNumber);
                    if (noteToRelease != null)
                    {
                        noteToRelease.IsPressed = false;
                    }
                }

                UpdateButtonColor();
            }
        }


        private void UpdateButtonColor()
        {
            midiButton.Dispatcher.Invoke(() =>
            {
                if (activeMidiNotes.Any(note => note.IsPressed))
                {
                    midiButton.Background = System.Windows.Media.Brushes.Green;
                }
                else
                {
                    midiButton.Background = originalButtonColor;
                }
            });
        }

        // (Important) Dispose of the MidiIn object when the app closes
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            midiIn.Stop();
            midiIn.Dispose();
        }
    }
}
class ActiveMidiNote
{
    public int NoteNumber { get; set; }
    public bool IsPressed { get; set; }
}