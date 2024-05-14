using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Multimedia;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
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
        public Window Owner { get; set; }
        private SelectingWindow selectingWindow;
        public PlayWindow(string FilePath, object sender)
        {
            InitializeComponent();
            selectingWindow = (SelectingWindow)sender;

            // run async thread zodat programma niet lockt tot de midi klaar is
            Task.Run(async () =>
            {
                // wacht 2 seconden zodat de midi niet direct begint met spelen
                StartPlay(@FilePath);
                                
                //StartPlay(@"..\..\..\..\midi-test\midis\final-bowser-theme.mid"); // todo: zorgen dat er een meegegeven midi gespeelt wordt
            });
            


        }

        public void StartPlay(String MidiFileName)
        {
            try
            {
                // check of midi meegegeven
                if (MidiFileName == null)
                {
                    throw new FileNotFoundException("ERROR: FILE NOT FOUND");
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
                        Thread.Sleep(2000);
                        midi.Play(output);
                    }
                }
            } catch (FileNotFoundException ex)
            {
                System.Windows.Forms.MessageBox.Show("File not found");
            }
        }
    }
}
