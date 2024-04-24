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

namespace InEenNotendop.UI
{
    /// <summary>
    /// Interaction logic for ImportWindow.xaml
    /// </summary>
    public partial class ImportWindow : Window
    {
        private string FilePath;
        private string FileName;
        public ImportWindow()
        {
            InitializeComponent();
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
                        diffecultyCheckbox |= 1;
                        break;
                    case string x when x.StartsWith("Medium"):
                        diffecultyCheckbox |= 2;
                        break;
                    case string x when x.StartsWith("Hard"):
                        diffecultyCheckbox |= 3;
                        break;
                }
            }
            // check if song is not null
            if (string.IsNullOrEmpty(ImportName.Text))
            {
                error = 1;
                MessageBox.Show("Vul een naam in");
            }
            else { songName = ImportName.Text; error = 0; }

            // check if artist is not null
            if (string.IsNullOrEmpty(ImportArtist.Text))
            {
                error = 1;
                MessageBox.Show("Vul een artiest in");
            }
            else { songArtist = ImportArtist.Text; error = 0; }

            try
            {
                // move selected file
                if (string.IsNullOrEmpty(FilePath) && string.IsNullOrEmpty(FileName))
                {
                    error = 1;
                    MessageBox.Show("Selecteer een bestand");
                }
                else { File.Copy(FilePath, @"..\..\..\Resources\Songs\Song_" + FileName); error = 0; }
            }
            catch (Exception exception)
            {
                Console.WriteLine("Name duplicate");
            }


            int songLength = GetLength(@"..\..\..\Resources\Songs\Song_" + FileName);
            int bpm = GetStartTempo(@"..\..\..\Resources\Songs\Song_" + FileName);

            string DBname = "PianoHeroDB";
            string user = System.Net.Dns.GetHostName() + "\\" + Environment.UserName;
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = "(localdb)\\MSSQLLocalDB";
            builder.IntegratedSecurity = true;
            builder.UserID = user;
            builder.Password = "";
            builder.ApplicationIntent = ApplicationIntent.ReadWrite;



            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();

                // Check if the database exists
                string checkDatabaseExistsQuery = $"SELECT 1 FROM sys.databases WHERE name = '{DBname}'";
                using (SqlCommand checkDatabaseExistsCommand = new SqlCommand(checkDatabaseExistsQuery, connection))
                {
                    object result = checkDatabaseExistsCommand.ExecuteScalar();
                    if (result != null)
                    {
                        // add song
                        string insertSongQuery = $@"
                            USE PianoHeroDB
                            INSERT INTO Nummers (Title, Artiest, Lengte, Bpm, Moeilijkheid)
                            VALUES ('{ImportName.Text}', '{ImportArtist.Text}', {songLength}, {bpm}, {diffecultyCheckbox});
                        ";
                        using (SqlCommand insertSongCommand = new SqlCommand(insertSongQuery, connection))
                        {
                            insertSongCommand.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        // Database exists, switch to it
                        builder.InitialCatalog = DBname;
                        connection.ChangeDatabase(DBname);
                    }
                }
                connection.Close();
            }



            // check if there are errors
            if (error == 0)
            {
                MessageBox.Show("succes");
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
                Console.WriteLine("ERROR: MIDI file niet gevonden.");
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
    }
}
