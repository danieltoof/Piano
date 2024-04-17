using System.Collections;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.VisualBasic;
using Console = System.Console;


using System.Windows.Navigation;
using System.Windows.Shapes;

static class Constants
{
    public const string ROLAND_DEVICE = "Roland Digital Piano";
    public const string DEFAULT = "Microsoft GS Wavetable Synth";
}

namespace Testing.UIChangeInducedByMIDI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ArrayList midiInDevices = new ArrayList();
            ArrayList midiOutDevices = new ArrayList();

            MidiDevice.ConfigMidiDevices(midiInDevices, midiOutDevices);

            Console.WriteLine("MIDI 'IN' devices:");
            foreach (MidiInDevice dev in midiInDevices)
            {
                Console.WriteLine(dev.deviceName);
                if (dev.deviceName == Constants.ROLAND_DEVICE)
                {
                    Console.WriteLine("Opening 'IN' device " + dev.deviceName + "...");
                    dev.Open();
                    Thread.Sleep(5000); // pause for 5 sec.
                    Console.WriteLine("Closing 'IN' device " + dev.deviceName + "...");
                    dev.Close();
                }
            }

            Console.WriteLine("MIDI 'OUT' devices:");
            foreach (MidiOutDevice dev in midiOutDevices)
            {
                Console.WriteLine(dev.deviceName);
                if (dev.deviceName == Constants.ROLAND_DEVICE)
                {
                    dev.Open();

                    Console.WriteLine("Playing a couple of notes on channel 1 ...");
                    MidiMusicalNote middleC = new MidiMusicalNote();
                    middleC.BeginPlay(dev);
                    Thread.Sleep(1000);
                    middleC.EndPlay(dev);
                    MidiMusicalNote noteA = new(0, 69, 77);
                    noteA.BeginPlay(dev);
                    Thread.Sleep(3000);
                    noteA.EndPlay(dev);

                    dev.Close();
                }
            }
        }

    }
}