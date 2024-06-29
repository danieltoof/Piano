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
        private int _songId;
        private int _currentScore;
        public SelectingWindow(int songId, string difficultyText, string title, string artist, int length, int bpm, string filePath, string convertedTime, object sender, int currentScore)
        {
            InitializeComponent();
            _songsWindow = (SongsWindow)sender;
            Owner = _songsWindow;
            this._filePath = filePath;
            DataContext = new SongDetailsViewModel(songId, difficultyText, title, artist, length, bpm, convertedTime, currentScore);
            FillDataGrid(songId);
            this._songId = songId;
            this._currentScore = currentScore;
        }

        // Class and constructor for detailed song screen
        public class SongDetailsViewModel 
        {
            public string SongIdText { get; }
            public string DifficultyText { get; }
            public string Title { get; }
            public string Artist { get; }
            public int Length { get; }
            public int Bpm { get; }
            public string ConvertedTime { get; }
            public int CurrentScore {get; }


            public SongDetailsViewModel(int songId, string difficultyText, string title, string artist, int length, int bpm, string convertedTime, int currentScore)
            {
                SongIdText = $"Clicked on Nummer with ID: {songId}";
                DifficultyText = $"Difficulty: {difficultyText}";
                Title = title;
                Artist = artist;
                Length = length;
                Bpm = bpm;
                ConvertedTime = convertedTime;
                CurrentScore = currentScore;

            }
        }
        private void PLAY_Button_OnClick(object sender, RoutedEventArgs e)
        {

            MidiPlayWindow midiPlayWindow = new MidiPlayWindow(_filePath, this, false, _songId, _songsWindow, _currentScore);
            Owner.Hide();

            midiPlayWindow.Show();
            Close();
        }
        private void AUTOPLAY_Button_OnClick(object sender, RoutedEventArgs e)
        {

            MidiPlayWindow midiPlayWindow = new MidiPlayWindow(_filePath, this, true, _songId, _songsWindow, _currentScore);

            Owner.Hide();
            midiPlayWindow.Show();
            Close();

        }

        private void Download_Button_OnClick(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as SongDetailsViewModel;

            _midiDataAccess.DownloadSong(viewModel.Artist,viewModel.Title);  
        }

        private void FillDataGrid(int songId)
        {
            HighScoresGrid.ItemsSource = _sqlDataAccess.GetTopScores(songId, 5);
        }
    }
}