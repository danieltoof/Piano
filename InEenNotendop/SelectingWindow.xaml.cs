using InEenNotendop.Data;
using System.Windows;

namespace InEenNotendop.UI
{
    /// <summary>
    /// Interaction logic for SelectingWindow.xaml
    /// </summary>
    public partial class SelectingWindow : Window
    {
        public Window Owner { get; set; }
        private SongsWindow songsWindow;
        DataProgram dataProgram = new DataProgram();
        string FilePath;
        private int nummerID;
        private int currentScore;
        public SelectingWindow(int nummerId, string moeilijkheidText, string Title, string Artiest, int Lengte, int Bpm, string FilePath, string ConvertedTime, object sender, int currentScore)
        {
            InitializeComponent();
            songsWindow = (SongsWindow)sender;
            Owner = songsWindow;
            this.FilePath = FilePath;
            DataContext = new NummerDetailsViewModel(nummerId, moeilijkheidText, Title, Artiest, Lengte, Bpm, ConvertedTime, currentScore);
            FillDataGrid(nummerId);
            this.nummerID = nummerId;
            this.currentScore = currentScore;
        }

        private void OnCloseClicked(object sender, RoutedEventArgs e)
        {
            Close();
        }

        // Class and constructor for detailed song screen
        public class NummerDetailsViewModel 
        {
            public string NummerIdText { get; }
            public string MoeilijkheidText { get; }
            public string Title { get; }
            public string Artiest { get; }
            public int Lengte { get; }
            public int Bpm { get; }
            public string ConvertedTime { get; }
            public int CurrentScore {get; }

            public NummerDetailsViewModel(int nummerId, string moeilijkheidText, string title, string artiest, int lengte, int bpm, string convertedTime, int currentScore)
            {
                NummerIdText = $"Clicked on Nummer with ID: {nummerId}";
                MoeilijkheidText = $"Difficulty: {moeilijkheidText}";
                Title = title;
                Artiest = artiest;
                Lengte = lengte;
                Bpm = bpm;
                ConvertedTime = convertedTime;
                CurrentScore = currentScore;

            }
        }
        private void PLAY_Button_Click(object sender, RoutedEventArgs e)
        {

            MidiPlayWindow midiPlayWindow = new MidiPlayWindow(FilePath, this, false, nummerID, songsWindow, currentScore);
            Owner.Hide();

            midiPlayWindow.Show();
            Close();
        }
        private void AUTOPLAY_Button_Click(object sender, RoutedEventArgs e)
        {

            MidiPlayWindow midiPlayWindow = new MidiPlayWindow(FilePath, this, true, nummerID, songsWindow, currentScore);

            //SongsWindow songsWindow = SongsWindow();
            Owner.Hide();
            midiPlayWindow.Show();
            Close();

        }

        private void OnDownloadClicked(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as NummerDetailsViewModel;

            dataProgram.DownloadSong(viewModel.Artiest,viewModel.Title);  
        }

        private void FillDataGrid(int nummerId)
        {
            HighScoresGrid.ItemsSource = dataProgram.GetDataForGrid(nummerId);
        }
    }
}