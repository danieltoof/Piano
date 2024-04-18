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
        internal int lightmode;
        private SettingsWindow settingsWindow;
        

        public MainWindow()
        {
            settingsWindow = new SettingsWindow(this);
            settingsWindow.Owner = this;
            InitializeComponent();
            CheckLightMode();
            CheckDarkOrLight();
        }

        public int CheckLightMode() // checkt systeem instellingen
        {
            return lightmode = (int)Microsoft.Win32.Registry.GetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize", "AppsUseLightTheme", 1);

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
            SelectSongWindow songWindow = new SelectSongWindow();
            songWindow.Show();
            this.Hide();
        }

        private void ExitButton_OnClick(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            Environment.Exit(0);
        }

        private void SettingsButton_OnClick(object sender, RoutedEventArgs e)
        {
            settingsWindow.Height = 350;
            settingsWindow.Width = 700;
            settingsWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            settingsWindow.ShowDialog();
        }
    }
}