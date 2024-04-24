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
using System.Windows.Threading;

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
        private Dictionary<int, ButtonColorData> midiNoteToButton = new Dictionary<int, ButtonColorData>(); // int = Midi note int, Button = button to assign to that Midi note

        public class ButtonColorData
        {
            public Button Button { get; set; }
            public System.Windows.Media.Brush OriginalButtonColor { get; set; }

        }
        public MainWindow()
        {
            InitializeComponent();
            originalButtonColor = midiButton.Background; // Store original color
            InitializeMidi();
            

            // Mapping Midi notes - Buttons
            midiNoteToButton.Add(72, new ButtonColorData(){ Button = C1Button,OriginalButtonColor = C1Button.Background}); 
            midiNoteToButton.Add(74, new ButtonColorData() { Button =D1Button, OriginalButtonColor = D1Button.Background });
            midiNoteToButton.Add(76, new ButtonColorData() { Button = E1Button, OriginalButtonColor = E1Button.Background });
            midiNoteToButton.Add(77, new ButtonColorData() { Button = F1Button, OriginalButtonColor = F1Button.Background });
            midiNoteToButton.Add(79, new ButtonColorData() { Button = F1Button, OriginalButtonColor = F1Button.Background });
            midiNoteToButton.Add(81, new ButtonColorData() { Button = A1Button, OriginalButtonColor = A1Button.Background });
            midiNoteToButton.Add(83, new ButtonColorData() { Button = B1Button, OriginalButtonColor = B1Button.Background });



        }

        private void InitializeMidi()
        {
            int numDevices = MidiIn.NumberOfDevices;
            int desiredDeviceIndex = 0; // Replace with the index you want
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
            if (e.MidiEvent is NoteOnEvent noteOnEvent)
            {
                if (midiNoteToButton.ContainsKey(noteOnEvent.NoteNumber))
                {
                    var buttonData = midiNoteToButton[noteOnEvent.NoteNumber];
                    buttonData.Button.Dispatcher.Invoke(() =>
                    {
                        buttonData.Button.Background = System.Windows.Media.Brushes.Green; // Example color
                    });
                }
            }
            else if (e.MidiEvent is NoteEvent noteEvent && noteEvent.Velocity == 0)
            {
                if (midiNoteToButton.ContainsKey(noteEvent.NoteNumber))
                {
                    var buttonData = midiNoteToButton[noteEvent.NoteNumber];
                    buttonData.Button.Dispatcher.Invoke(() =>
                    {
                        buttonData.Button.Background = buttonData.OriginalButtonColor; // Restore original color
                    });
                }
            }
            UpdateButtonColor();

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