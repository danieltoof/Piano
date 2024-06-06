using System.Windows;
using PianoHero.Data;
using PianoHero.UI;

namespace PianoHero
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        public SettingsWindow settingsWindow;
        DataProgram data = new DataProgram();

        // Default constructor
        public MainWindow() 
        {
            InitializeComponent();
            settingsWindow = new SettingsWindow(this);
            data.StartSshTunnel();
            CheckLightMode();
            CheckDarkOrLight();
        }

        // Constructor used for main menu button
        public MainWindow(SettingsWindow settingsWindow) 
        {
            InitializeComponent();
            this.settingsWindow = settingsWindow;
            settingsWindow.ChangeSettingsOwner(this);
            CheckDarkOrLight();
            this.settingsWindow.MainMenuButton.Visibility = Visibility.Hidden;
        }

        // Checks system setting for lightmode
        public int CheckLightMode() 
        {
            return settingsWindow.lightmode = (int)Microsoft.Win32.Registry.GetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize", "AppsUseLightTheme", 1);
        }

        private void StartButton_OnClick(object sender, RoutedEventArgs e)
        {
            SongsWindow songsWindow = new SongsWindow(settingsWindow);
            songsWindow.Show();
            Close();
        }

        private void ExitButton_OnClick(object sender, RoutedEventArgs e)
        {
            data.StopSshTunnel();
            Environment.Exit(0);
        }

        private void SettingsButton_OnClick(object sender, RoutedEventArgs e)
        {
            settingsWindow.OpenSettings();
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
    }
}