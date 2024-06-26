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
        private SongsWindow _songsWindow;
        private SqlDataAccess _sqlDataAccess = new();
        private MidiDataAccess _midiDataAccess = new();
        private string _filePath;
        private int _nummerId;
        private int _currentScore;
        public SelectingWindow(int nummerId, string moeilijkheidText, string title, string artiest, int lengte, int bpm, string filePath, string convertedTime, object sender, int currentScore)
        {
            InitializeComponent();
            _songsWindow = (SongsWindow)sender;
            Owner = _songsWindow;
            this._filePath = filePath;
            DataContext = new NummerDetailsViewModel(nummerId, moeilijkheidText, title, artiest, lengte, bpm, convertedTime, currentScore);
            FillDataGrid(nummerId);
            this._nummerId = nummerId;
            this._currentScore = currentScore;
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
        private void PLAY_Button_OnClick(object sender, RoutedEventArgs e)
        {

            MidiPlayWindow midiPlayWindow = new MidiPlayWindow(_filePath, this, false, _nummerId, _songsWindow, _currentScore);
            Owner.Hide();

            midiPlayWindow.Show();
            Close();
        }
        private void AUTOPLAY_Button_OnClick(object sender, RoutedEventArgs e)
        {

            MidiPlayWindow midiPlayWindow = new MidiPlayWindow(_filePath, this, true, _nummerId, _songsWindow, _currentScore);

            //SongsWindow songsWindow = SongsWindow();
            Owner.Hide();
            midiPlayWindow.Show();
            Close();

        }

        private void Download_Button_OnClick(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as NummerDetailsViewModel;

            _midiDataAccess.DownloadSong(viewModel.Artiest,viewModel.Title);  
        }

        private void FillDataGrid(int nummerId)
        {
            HighScoresGrid.ItemsSource = _sqlDataAccess.GetDataForPreviewGrid(nummerId);
        }
    }
}