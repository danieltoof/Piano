using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using InEenNotendop.Business;
using InEenNotendop.Data;

namespace InEenNotendop.UI
{
    /// <summary>
    /// Interaction logic for SongsWindow.xaml
    /// </summary>
    public partial class SongsWindow : Window
    {
        public SettingsWindow SettingsWindow;
        private SqlDataAccess _sqlDataAccess = new();
        private bool _lightmodeImport;
        private int _difficulty = 0;
        public bool SongIsFinished { get; set; }

        public SongsWindow(SettingsWindow settingsWindow)
        {
            InitializeComponent();
            
            this.SettingsWindow = settingsWindow;
            this.SettingsWindow.ChangeSettingsOwner(this);


            FilterBox.Items.Add("No Filter");
            FilterBox.Items.Add("Easy");
            FilterBox.Items.Add("Medium");
            FilterBox.Items.Add("Hard");

            SortBox.Items.Add("A-Z");
            SortBox.Items.Add("Z-A");
            SortBox.Items.Add("Diff. ascending");
            SortBox.Items.Add("Diff. descending");

            Song.ItemsSource = _sqlDataAccess.MakeDefaultList();
            SettingsWindow.CheckDarkOrLight(this);

        }

        // Updates the song list so the new score is shown, also keeps the selected filter.
        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            if (SongIsFinished)
            {
                SongIsFinished = false;
                if (_difficulty != 0)
                {
                    Song.ItemsSource = _sqlDataAccess.MakeFilteredList(_difficulty);
                }
                else
                {
                    Song.ItemsSource = _sqlDataAccess.MakeDefaultList();
                }
            }
        }

        private void MenuToggleButton_OnClick(object sender, RoutedEventArgs e)
        {
            // Toggle visibility of the MenuPanel.
            if (MenuPanel.Visibility == Visibility.Visible)
            {
                MenuPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                MenuPanel.Visibility = Visibility.Visible;
            }
        }

        private void SettingsButton_OnClick(object sender, RoutedEventArgs e)
        {
            SettingsWindow.OpenSettings();
        }

        // Creates pop-up window with detailed song information.
        private void OnNumberClicked(object sender, MouseButtonEventArgs e) 
        {
            if (sender is FrameworkElement clickedElement)
            {
                var song = clickedElement.DataContext as Song;
                if (song != null)
                {
                    int nummerId = song.Id;
                    String title = song.Title;
                    String artist = song.Artist;
                    int fullTime = song.FullTime;
                    int bpm = song.Bpm;
                    String filePath = song.Filepath;
                    string convertedTime = song.ConvertedTime;
                    DifficultyConverter difficultyConverter = new DifficultyConverter();
                    int currentScore = song.Score;
                    string difficultyText = difficultyConverter.Convert(song.Difficulty);

                    SelectingWindow detailsWindow = new SelectingWindow(nummerId, difficultyText, title, artist, fullTime, bpm, filePath, convertedTime, this, currentScore);
                    detailsWindow.Owner = this;
                    detailsWindow.ShowDialog();
                }
            }
        }


        // Opens window to import song and refreshes list.
        private void ImportButton_OnClick(object sender, RoutedEventArgs e) 
        {
            _lightmodeImport = SettingsWindow.Lightmode;
            ImportWindow import = new ImportWindow(_lightmodeImport);
            import.ShowDialog();
            Song.ItemsSource = _sqlDataAccess.MakeDefaultList();
        }

        // Changes list to filtered list.
        private void FilterBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e) 
        {
            string filter = (sender as System.Windows.Controls.ComboBox).SelectedItem as string;
            switch (filter)
            {
                case "Easy":
                    _difficulty = 1;
                    break;
                case "Medium":
                    _difficulty = 2;
                    break;
                case "Hard":
                    _difficulty = 3;
                    break;
                case "No Filter":
                    _difficulty = 0;
                    break;
            }
            if (_difficulty != 0)
            {
                Song.ItemsSource = _sqlDataAccess.MakeFilteredList(_difficulty);
            }
            else
            {
                Song.ItemsSource = _sqlDataAccess.MakeDefaultList();
            }
        }

        // Changes list to be sorted by chosen sorting method.
        private void SortBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e) 
        {
            string sort = (sender as System.Windows.Controls.ComboBox).SelectedItem as string;
            string completeSort = "";
            switch (sort)
            {
                case "A-Z":
                    completeSort = "Title ASC";
                    break;
                case "Z-A":
                    completeSort = "Title DESC";
                    break;
                case "Diff. ascending":
                    completeSort = "Moeilijkheid ASC";
                    break;
                case "Diff. descending":
                    completeSort = "Moeilijkheid DESC";
                    break;
            }
            Song.ItemsSource = _sqlDataAccess.MakeSortedList(_difficulty, completeSort);
        }

        // Goes back to main menu.
        private void BackButton_OnClick(object sender, RoutedEventArgs e) 
        {
            SettingsWindow.MainMenu();
        }
    }
}