using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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

        public MainWindow()
        {
            InitializeComponent();
            settingsWindow = new SettingsWindow(this);
            CheckLightMode();
            CheckDarkOrLight();
        }

        public MainWindow(SettingsWindow settingsWindow)
        {
            InitializeComponent();
            this.settingsWindow = settingsWindow;
            settingsWindow.ChangeSettingsOwner(this);
            CheckDarkOrLight();
            this.settingsWindow.MainMenuButton.Visibility = Visibility.Hidden;
        }



        public int CheckLightMode() // checkt systeem instellingen
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
            Environment.Exit(0);
        }

        private void SettingsButton_OnClick(object sender, RoutedEventArgs e)
        {
            settingsWindow.OpenSettings();
        }

        private void CheckDarkOrLight() // veranderd light mode naar dark mode en dark mode naar light mode
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