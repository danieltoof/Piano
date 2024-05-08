using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Multimedia;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace InEenNotendop.UI
{
    /// <summary>
    /// Interaction logic for PlayWindow.xaml
    /// </summary>
    public partial class PlayWindow : Window
    {
        public PlayWindow()
        {
            InitializeComponent();

            // run async thread zodat programma niet lockt tot de midi klaar is
            Task.Run(async () =>
            {
                // wacht 2 seconden zodat de midi niet direct begint met spelen
                Thread.Sleep(2000);
                StartPlay(@"..\..\..\..\midi-test\midis\07_Flower_Garden_GM.mid"); // todo: zorgen dat er een meegegeven midi gespeelt wordt
            });
            


        }

        public void StartPlay(String MidiFileName)
        {
            // check of midi meegegeven
            if (MidiFileName == null) { 
                Console.WriteLine("geen midi");
                return;
            }
            else
            {
                // inladen midi
                var midi = MidiFile.Read(MidiFileName);

                using (var output = OutputDevice.GetByName("Microsoft GS Wavetable Synth"))
                //using (var playback = midi.GetPlayback(output))
                {
                    //playback.Speed = 5;
                    //playback.Play();
                    midi.Play(output);
                }
            }
        }
    }
}
