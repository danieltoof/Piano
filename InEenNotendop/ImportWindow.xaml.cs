using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Microsoft.Data.SqlClient;
using InEenNotendop.Data;

namespace InEenNotendop.UI
{
    /// <summary>
    /// Interaction logic for ImportWindow.xaml
    /// </summary>
    public partial class ImportWindow : Window
    {
        private string FilePath;
        private string FileName;
        private int lightmode;
        DataProgram data = new DataProgram();
        public ImportWindow(int lightmodeImport)
        {
            InitializeComponent();
            lightmode = lightmodeImport;
            CheckDarkOrLight();

        }

        private void selectFile_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.DefaultExt = ".midi"; // Default file extension
            dialog.Filter = "midi documenten (.mid)|*.mid"; // Filter files by extension

            // Show open file dialog box
            bool? result = dialog.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Get the data from selected file
                FilePath = dialog.FileName;
                FileName = dialog.SafeFileName;
            }
        }

        private async void saveButton_Click(object sender, RoutedEventArgs e)
        {
            int error = 1;
            int diffecultyCheckbox = 1;
            string myText = ImportName.Text;
            var checkedValue = "Easy";
            string songName;
            string songArtist;

            RadioButton rb = FindVisualChildren<RadioButton>(ImportDiffeculty).FirstOrDefault(x => x.IsChecked == true);
            if (rb != null)
            {
                checkedValue = rb.Content.ToString();
                switch (checkedValue)
                {
                    case string x when x.StartsWith("Easy"):
                        diffecultyCheckbox = 1;
                        break;
                    case string x when x.StartsWith("Medium"):
                        diffecultyCheckbox = 2;
                        break;
                    case string x when x.StartsWith("Hard"):
                        diffecultyCheckbox = 3;
                        break;
                }
            }
            // check if song is not null
            if (string.IsNullOrEmpty(ImportName.Text))
            {
                error = 1;
                MessageBox.Show("Enter a name");
            }
            else { songName = ImportName.Text; error = 0; }

            // check if artist is not null
            if (string.IsNullOrEmpty(ImportArtist.Text))
            {
                error = 1;
                MessageBox.Show("Enter an artist");
            }
            else { songArtist = ImportArtist.Text; error = 0; }

            try
            {
                // move selected file
                if (string.IsNullOrEmpty(FilePath) && string.IsNullOrEmpty(FileName))
                {
                    error = 1;
                    MessageBox.Show("Select a file");
                }
                else { File.Copy(FilePath, @"..\..\..\Resources\Songs\Song_" + FileName); error = 0; }
            }
            catch (Exception exception)
            {
                Console.WriteLine("Name duplicate");
            }


            int songLength = GetLength(@"..\..\..\Resources\Songs\Song_" + FileName);
            int bpm = GetStartTempo(@"..\..\..\Resources\Songs\Song_" + FileName);
            string filepath = $"/home/student/Music/{ImportArtist.Text} - {ImportName.Text}.mid";


            //import func here
            data.UploadsongToDataBase(ImportName.Text, ImportArtist.Text, songLength, bpm, diffecultyCheckbox, filepath);
            data.UploadSong(ImportName.Text, ImportArtist.Text, FilePath);


            // check if there are errors
            if (error == 0)
            {
                MessageBox.Show("Upload success!");
                await Task.Delay(1000);
                Close();
            }
        }

        static int GetLength(string MidiFileName)
        {
            // check of bestandsnaam opgegeven is
            if (!MidiFileName.EndsWith(".mid"))
            {
                //voeg bestandsnaam toe aan string
                MidiFileName = MidiFileName + ".mid";
            }

            try
            {
                // inladen midi
                var midi = MidiFile.Read(MidiFileName);

                // lees lengte van midi
                TimeSpan MidiFileDuration = midi.GetDuration<MetricTimeSpan>();

                // return final waarde
                return (int)MidiFileDuration.TotalSeconds;

            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("ERROR: MIDI file not found.");
            }

            return 0;
        }

        static int GetStartTempo(string MidiFileName)
        {
            // inladen midi
            var midi = MidiFile.Read(MidiFileName);

            // haal tempo van midi op
            using (var tempoMapManager = new TempoMapManager(
                       midi.TimeDivision,
                       midi.GetTrackChunks().Select(c => c.Events)))
            {
                // maken tempomap
                TempoMap tempoMap = tempoMapManager.TempoMap;

                // lezen start tempo
                int tempoStart = (int)Math.Round(tempoMap.GetTempoAtTime((MidiTimeSpan)1).BeatsPerMinute);

                return tempoStart;
            }
        }
        private static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                int NbChild = VisualTreeHelper.GetChildrenCount(depObj);

                for (int i = 0; i < NbChild; i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);

                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childNiv2 in FindVisualChildren<T>(child))
                    {
                        yield return childNiv2;
                    }
                }
            }
        }

        private void CheckDarkOrLight() // veranderd light mode naar dark mode en dark mode naar light mode
        {
            if (lightmode == 1)
            {
                SetLightMode();
            }
            else if (lightmode == 0)
            {
                SetDarkMode();
            }
        }

        private void SetLightMode()
        {
            ImportGrid.Background = Brushes.White;
            SettingsText.Foreground = Brushes.Black;
            ImportArtistLabel.Foreground = Brushes.Black;
            ImportDiffecultyLabel.Foreground = Brushes.Black; ;
            ImportnameLabel.Foreground = Brushes.Black;

            EasyButton.Foreground = Brushes.Black;
            MediumButton.Foreground = Brushes.Black;
            HardButton.Foreground = Brushes.Black;
        }

        private void SetDarkMode()
        {
            ImportGrid.Background = new SolidColorBrush(Color.FromRgb(25, 44, 49));
            SettingsText.Foreground = Brushes.White;
            ImportArtistLabel.Foreground = Brushes.White;
            ImportDiffecultyLabel.Foreground = Brushes.White;
            ImportnameLabel.Foreground = Brushes.White;

            EasyButton.Foreground = Brushes.White;
            MediumButton.Foreground = Brushes.White;
            HardButton.Foreground = Brushes.White;
        }
    }
}
