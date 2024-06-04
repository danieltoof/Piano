using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using InEenNotendop.Data;
using System.Diagnostics;
using System.Globalization;
using System.Diagnostics.Eventing.Reader;
using System.Windows.Forms;

namespace InEenNotendop.UI
{
    /// <summary>
    /// Interaction logic for SongsWindow.xaml
    /// </summary>
    public partial class SongsWindow : Window
    {
        private SettingsWindow settingsWindow;
        private DataProgram dataProgram;
        private int lightmodeImport;
        private int Difficulty = 0;

        public SongsWindow(SettingsWindow settingsWindow)
        {
            InitializeComponent();
            dataProgram = new DataProgram();
            dataProgram.StartSshTunnel();
            
            this.settingsWindow = settingsWindow;
            this.settingsWindow.ChangeSettingsOwner(this);


            FilterBox.Items.Add("No Filter");
            FilterBox.Items.Add("Easy");
            FilterBox.Items.Add("Medium");
            FilterBox.Items.Add("Hard");

            SortBox.Items.Add("A-Z");
            SortBox.Items.Add("Z-A");
            SortBox.Items.Add("Diff. ascending");
            SortBox.Items.Add("Diff. descending");

            Nummer.ItemsSource = dataProgram.MaakLijst();
            CheckDarkOrLight();

        }

        private void MenuToggleButton_Click(object sender, RoutedEventArgs e)
        {
            // Toggle visibility of the MenuPanel
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
            settingsWindow.OpenSettings();
        }

        // Creates pop-up window with detailed song information
        private void OnNumberClicked(object sender, MouseButtonEventArgs e) 
        {
            if (sender is FrameworkElement clickedElement)
            {
                var nummer = clickedElement.DataContext as Nummer;
                if (nummer != null)
                {
                    int nummerId = nummer.Id;
                    String Title = nummer.Title;
                    String Artiest = nummer.Artiest;
                    int FullTime = nummer.FullTime;
                    int Bpm = nummer.Bpm;
                    String FilePath = nummer.Filepath;
                    MoeilijkheidConverter moeilijkheidConverter = new MoeilijkheidConverter();
                    int currentScore = nummer.Score;
                    string moeilijkheidText = moeilijkheidConverter.Convert(nummer.Moeilijkheid, typeof(string), null, CultureInfo.InvariantCulture) as string;

                    SelectingWindow detailsWindow = new SelectingWindow(nummerId, moeilijkheidText, Title, Artiest, FullTime, Bpm, FilePath, this, currentScore);
                    detailsWindow.Owner = this;
                    detailsWindow.ShowDialog();
                }
            }
        }

        // Checks lightmode value and changes between dark- and lightmode
        private void CheckDarkOrLight() 
        {
            if (settingsWindow.lightmode == 1)
            {
                settingsWindow.SetLightMode(this);
            }
            else if (settingsWindow.lightmode == 0)
            {
                settingsWindow.SetDarkMode(this);
            }
        }

        // Opens window to import song and refreshes list
        private void ImportButton_OnClick(object sender, RoutedEventArgs e) 
        {
            lightmodeImport = settingsWindow.lightmode;
            ImportWindow import = new ImportWindow(lightmodeImport);
            import.ShowDialog();
            Nummer.ItemsSource = dataProgram.MaakLijst();
        }

        // Changes list to filtered list
        private void FilterBox_SelectionChanged(object sender, SelectionChangedEventArgs e) 
        {
            string Filter = (sender as System.Windows.Controls.ComboBox).SelectedItem as string;
            switch (Filter)
            {
                case "Easy":
                    Difficulty = 1;
                    break;
                case "Medium":
                    Difficulty = 2;
                    break;
                case "Hard":
                    Difficulty = 3;
                    break;
                case "No Filter":
                    Difficulty = 0;
                    break;
            }
            if (Difficulty != 0)
            {
                Nummer.ItemsSource = dataProgram.MaakFilteredLijst(Difficulty);
            }
            else
            {
                Nummer.ItemsSource = dataProgram.MaakLijst();
            }
        }

        // Changes list to be sorted by chosen sorting method
        private void SortBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e) 
        {
            string Sort = (sender as System.Windows.Controls.ComboBox).SelectedItem as string;
            string CompleteSort = "";
            switch (Sort)
            {
                case "A-Z":
                    CompleteSort = "Title ASC";
                    break;
                case "Z-A":
                    CompleteSort = "Title DESC";
                    break;
                case "Diff. ascending":
                    CompleteSort = "Moeilijkheid ASC";
                    break;
                case "Diff. descending":
                    CompleteSort = "Moeilijkheid DESC";
                    break;
            }
            
            Nummer.ItemsSource = dataProgram.MakeSortedList(Difficulty, CompleteSort);
        }

        // Goes back to main menu
        private void BackButton_Click(object sender, RoutedEventArgs e) 
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show(); 
            Close(); 
        }
    }
}
