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
using InEenNotendop.UI;

namespace InEenNotendop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        private SettingsWindow settingsWindow;
        

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

        private void CheckDarkOrLight() // veranderd light mode naar dark mode en dark mode naar light mode
        {
            if (lightmode == 1)
            {
                settingsWindow.SetLightMode();
            }
            else if (lightmode == 0)
            {
                settingsWindow.SetDarkMode();
            }
        }


        private void PlayButton_OnClick(object sender, RoutedEventArgs e)
        {
<<<<<<< HEAD
            SelectSongWindow songWindow = new SelectSongWindow();
            songWindow.Show();
            this.Hide();
=======
            SongsWindow songsWindow = new SongsWindow(settingsWindow);
            songsWindow.Show();
            Close();
>>>>>>> demo
        }

        private void ExitButton_OnClick(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void SettingsButton_OnClick(object sender, RoutedEventArgs e)
        {
            settingsWindow.OpenSettings();
        }
<<<<<<< HEAD
=======

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
>>>>>>> demo
    }
}