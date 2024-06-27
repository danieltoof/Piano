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
        
        public SettingsWindow SettingsWindow { get; set; }
        private SqlDataAccess _sshScript = new();

        // Default constructor
        public MainWindow() 
        {
            InitializeComponent();
            SettingsWindow = new SettingsWindow(this);
            _sshScript.StartSshTunnel();
            CheckLightMode();
            SettingsWindow.CheckDarkOrLight(this);
        }

        // Constructor used for main menu button
        public MainWindow(SettingsWindow settingsWindow) 
        {
            InitializeComponent();
            this.SettingsWindow = settingsWindow;
            settingsWindow.ChangeSettingsOwner(this);
            settingsWindow.CheckDarkOrLight(this);
            this.SettingsWindow.MainMenuButton.Visibility = Visibility.Hidden;

        }

        // Checks system setting for lightmode
        public bool CheckLightMode() 
        {
            if ((int)Microsoft.Win32.Registry.GetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize", "AppsUseLightTheme", 1) == 1)
            {
                return SettingsWindow.Lightmode = true;
            }
            else
            {
                return SettingsWindow.Lightmode = false;
            }
        }

        private void Start_Button_OnClick(object sender, RoutedEventArgs e)
        {
            SongsWindow songsWindow = new SongsWindow(SettingsWindow);
            songsWindow.Show();
            Close();
        }

        // Exits program and closes the ssh tunnel
        private void ExitButton_OnClick(object sender, RoutedEventArgs e)
        {
            _sshScript.StopSshTunnel();
            Environment.Exit(0);
        }

        private void Settings_Button_OnClick(object sender, RoutedEventArgs e)
        {
            SettingsWindow.OpenSettings();
        }

        private void HighscoreButton_OnClick(object sender, RoutedEventArgs e)
        {
            HighscoreList highscoreList = new HighscoreList(SettingsWindow);
            highscoreList.Show();
            Close();

        }
    }
}