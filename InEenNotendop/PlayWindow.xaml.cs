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

            StartPlay(@FilePath);
        }

        public void StartPlay(String MidiFileName)
        {
            Task.Run(async () =>
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
                            // wacht 2 seconden zodat de midi niet direct begint met spelen
                            Thread.Sleep(2000);
                            // run async thread zodat programma niet lockt tot de midi klaar is
                        
                            midi.Play(output);
                        }
                }
            } catch (FileNotFoundException ex)
            {
                System.Windows.Forms.MessageBox.Show("File not found");
                }
            });
        }
    }
}
