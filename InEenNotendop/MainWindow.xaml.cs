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
        
        public SettingsWindow SettingsWindow;
        DataProgram _data = new DataProgram();

        // Default constructor
        public MainWindow() 
        {
            InitializeComponent();
            SettingsWindow = new SettingsWindow(this);
            _data.StartSshTunnel();
            CheckLightMode();
            CheckDarkOrLight();
        }

        // Constructor used for main menu button
        public MainWindow(SettingsWindow settingsWindow) 
        {
            InitializeComponent();
            this.SettingsWindow = settingsWindow;
            settingsWindow.ChangeSettingsOwner(this);
            CheckDarkOrLight();
            this.SettingsWindow.MainMenuButton.Visibility = Visibility.Hidden;
        }

        // Checks system setting for lightmode
        public int CheckLightMode() 
        {
            return SettingsWindow.Lightmode = (int)Microsoft.Win32.Registry.GetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize", "AppsUseLightTheme", 1);
        }

        private void StartButton_OnClick(object sender, RoutedEventArgs e)
        {
            SongsWindow songsWindow = new SongsWindow(SettingsWindow);
            songsWindow.Show();
            Close();
        }

        private void ExitButton_OnClick(object sender, RoutedEventArgs e)
        {
            _data.StopSshTunnel();
            Environment.Exit(0);
        }

        private void SettingsButton_OnClick(object sender, RoutedEventArgs e)
        {
            SettingsWindow.OpenSettings();
        }

        // Checks lightmode value and changes between dark- and lightmode
        private void CheckDarkOrLight() 
        {
            if (SettingsWindow.Lightmode == 1)
            {
                SettingsWindow.SetLightMode(this);
            }
            else if (SettingsWindow.Lightmode == 0)
            {
                SettingsWindow.SetDarkMode(this);
            }
        }
    }
}