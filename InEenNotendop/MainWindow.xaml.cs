using System.Windows;
using InEenNotendop.Data;
using InEenNotendop.UI;

namespace InEenNotendop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        public SettingsWindow settingsWindow;
        private SqlDataAccess _sshScript = new();

        // Default constructor
        public MainWindow() 
        {
            InitializeComponent();
            settingsWindow = new SettingsWindow(this);
            _sshScript.StartSshTunnel();
            CheckLightMode();
            SettingsWindow.CheckDarkOrLight(this);
        }

        // Constructor used for main menu button
        public MainWindow(SettingsWindow settingsWindow) 
        {
            InitializeComponent();
            this.settingsWindow = settingsWindow;
            settingsWindow.ChangeSettingsOwner(this);
            SettingsWindow.CheckDarkOrLight(this);
            this.SettingsWindow.MainMenuButton.Visibility = Visibility.Hidden;

        }

        // Checks system setting for lightmode
        public int CheckLightMode() 
        {
            return settingsWindow.Lightmode = (int)Microsoft.Win32.Registry.GetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize", "AppsUseLightTheme", 1);
        }

        private void Start_Button_OnClick(object sender, RoutedEventArgs e)
        {
            SongsWindow songsWindow = new SongsWindow(settingsWindow);
            songsWindow.Show();
            Close();
        }

        private void Exit_Button_OnClick(object sender, RoutedEventArgs e)
        {
            _sshScript.StopSshTunnel();
            Environment.Exit(0);
        }

        private void Settings_Button_OnClick(object sender, RoutedEventArgs e)
        {
            settingsWindow.OpenSettings();
        }

        private void HighscoreButton_OnClick(object sender, RoutedEventArgs e)
        {
            HighscoreList highscoreList = new HighscoreList(SettingsWindow);
            highscoreList.Show();
            Close();

        }
    }
}