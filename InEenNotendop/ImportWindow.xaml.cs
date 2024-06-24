using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Diagnostics;
using InEenNotendop.Data;
using NAudio.Midi;
using InEenNotendop.Business;

namespace InEenNotendop.UI
{
    /// <summary>
    /// Interaction logic for ImportWindow.xaml
    /// </summary>
    public partial class ImportWindow : Window
    {
        private string _filePath;
        private string _fileName;
        private int _lightmode;
        private SqlDataAccess _sqlDataAccess = new();
        private MidiDataAccess _midiDataAccess = new();
        public ImportWindow(int lightmodeImport)
        {
            InitializeComponent();
            _lightmode = lightmodeImport;
            CheckDarkOrLight();
        }

        // Code to process selecting file
        private void SelectFile_Click(object sender, RoutedEventArgs e) 
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
                _filePath = dialog.FileName;
                _fileName = dialog.SafeFileName;
            }
        }

        // Code to process save button
        private async void SaveButton_Click(object sender, RoutedEventArgs e) 
        {
            int diffecultyCheckbox = 1;
            string myText = ImportName.Text;
            var checkedValue = "Easy";
            string songName = "";
            string songArtist = "";
            bool fileLocation = false;

            RadioButton rb = FindVisualChildren<RadioButton>(ImportDiffeculty).FirstOrDefault(x => x.IsChecked == true); // Gets selected difficulty from radio button
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
            string messageName = "";
            string messageArtist = "";
            string messageFile = "";
            // check if song is not null
            if (string.IsNullOrEmpty(ImportName.Text))
            {
                messageName = "Enter an artist";
            }
            else { songName = ImportName.Text;  messageName = null; }

            // check if artist is not null
            if (string.IsNullOrEmpty(ImportArtist.Text))
            {
                messageArtist = "Enter an artist";
            }
            else { songArtist = ImportArtist.Text; messageArtist = null; }

            // Checks if file is selected
            if (string.IsNullOrEmpty(_filePath) && string.IsNullOrEmpty(_fileName)) 
            {
                fileLocation = false;
                messageFile = "Select a file";
            }
            else
            {
                fileLocation = true;
            }

            // Final check before uploading
            if (!string.IsNullOrEmpty(songName) && !string.IsNullOrEmpty(songArtist) && fileLocation == true) {
                int songLength = GetLength(_filePath);
                int bpm = GetStartTempo(_filePath);
                string filepath = @"..\..\..\Resources\Song\" + songArtist + " - " + songName + ".mid";

                _sqlDataAccess.UploadSongToDataBase(songName, songArtist, songLength, bpm, diffecultyCheckbox, filepath);
                _midiDataAccess.UploadSongToServer(songName, songArtist, _filePath);

                MessageBox.Show("Upload success!");
                await Task.Delay(1000);
                Close();
            }
            else
            {
                string msg = messageName + Environment.NewLine + messageArtist + Environment.NewLine + messageFile;
                MessageBox.Show(msg);
            }
        }

        static int GetLength(string midiFileName)
        {
            // check of bestandsnaam opgegeven is
            if (!midiFileName.EndsWith(".mid"))
            {
                //voeg bestandsnaam toe aan string
                midiFileName = midiFileName + ".mid";
            }
           
            try
            {
                // inladen midi
                MidiFile midiFile = new MidiFile(midiFileName);
               
                return (int)MidiUtilities.GetSongLength(midiFile).TotalSeconds;

            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("ERROR: MIDI file not found.");
            }
            return 0;
        }

        static int GetStartTempo(string midiFileName)
        {
            try
            {             
                MidiFile midiFile = new MidiFile(midiFileName);

                return MidiUtilities.GetMidiBpm(midiFile);
            }
            catch (FileNotFoundException)
            {
                Debug.WriteLine("ERROR: MIDI file not found.");
                return 0;
            }
            catch (Exception)
            {
                Debug.WriteLine("Overige error.");
                return 0;
            }
        }
        private static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject // Code to process the finding of the value from the radio button
        {
            if (depObj != null)
            {
                int nbChild = VisualTreeHelper.GetChildrenCount(depObj);

                for (int i = 0; i < nbChild; i++)
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

        private void CheckDarkOrLight() // Checks lightmode value and changes between dark- and lightmode
        {
            if (_lightmode == 1)
            {
                SetLightMode();
            }
            else if (_lightmode == 0)
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