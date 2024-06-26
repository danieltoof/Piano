using InEenNotendop.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using InEenNotendop.Data;
using System.Diagnostics;

namespace InEenNotendop.UI
{
    /// <summary>
    /// Interaction logic for SongsWindow.xaml
    /// </summary>
    public partial class HighscoreList : Window
    {
        public SettingsWindow SettingsWindow;
        SqlDataAccess _sqlDataAccess = new();
        public bool SongIsFinished;
        
        public HighscoreList(SettingsWindow settingsWindow)
        {
            InitializeComponent();

            this.SettingsWindow = settingsWindow;
            this.SettingsWindow.ChangeSettingsOwner(this);

            ListOfHighestScores.ItemsSource = _sqlDataAccess.MakeHighscoreList();

            SettingsWindow.CheckDarkOrLight(this);
        }

        // Creates pop-up window with detailed song information
        //AANPASSEN NAAR SCORE SCREEN
        private void OnNumberClicked(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement clickedElement)
            {
                var nummer = clickedElement.DataContext as Nummer;
                if (nummer != null)
                {
                    int nummerId = nummer.Id;
                    String title = nummer.Title;
                    String artiest = nummer.Artiest;
                    int fullTime = nummer.FullTime;
                    int bpm = nummer.Bpm;
                    String filePath = nummer.Filepath;
                    string convertedTime = nummer.ConvertedTime;
                    MoeilijkheidConverter moeilijkheidConverter = new MoeilijkheidConverter();
                    int currentScore = nummer.Score;
                    string moeilijkheidText = moeilijkheidConverter.Convert(nummer.Moeilijkheid);

                    HighscoreDetails detailsWindow = new HighscoreDetails(nummerId, moeilijkheidText, title, artiest, fullTime, bpm, filePath, convertedTime, this, currentScore);
                    detailsWindow.Owner = this;
                    detailsWindow.ShowDialog();
                }
            }
        }

        // Goes back to main menu
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow.MainMenu();
            Close();

        }
    }
}