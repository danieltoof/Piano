using InEenNotendop.Data;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace InEenNotendop.UI
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        internal bool Lightmode;
        public Window Owner { get; set; }
        private MainWindow _mainWindow;
        private SongsWindow _songsWindow;
        private HighscoreList _highscoreList;

        private MidiPlayWindow _midiPlayWindow;
        private int _isOkToClose = 0;
        SshScript _sshScript = new();

        public SettingsWindow(object sender)
        {
            InitializeComponent();
            switch (sender)
            {
                case MainWindow:
                    _mainWindow = (MainWindow)sender;
                    Owner = _mainWindow;
                    break;
                case SongsWindow:
                    _songsWindow = (SongsWindow)sender;
                    Owner = _songsWindow;
                    break;
                case HighscoreList:
                    _highscoreList = (HighscoreList)sender;
                    Owner = _highscoreList;
                    break;
                case MidiPlayWindow:
                    _midiPlayWindow = (MidiPlayWindow)sender;
                    Owner = _midiPlayWindow;
                    Owner.Closing += Owner_Closing;
                    break;
            }
            Owner.Closing += Owner_Closing;
        }

        // Makes sure settings window is closed when application is closed.
        private void Owner_Closing(object? sender, CancelEventArgs e) 
        {
            if (sender.Equals(Owner))
            {
                _isOkToClose = 1;
                Close();
            }
        }

        // Makes sure settings window is not closed when not needed.
        protected override void OnClosing(CancelEventArgs e) 
        {
            if (_isOkToClose != 1)
            {
                this.Visibility = Visibility.Hidden;
                e.Cancel = true;
            }
            else
            {
                _sshScript.StopSshTunnel();
                e.Cancel = false;
            }
        }

        // Opens settings window.
        public void OpenSettings() 
        {
            // Makes sure main menu button is not visible when on main menu.
            if (Owner is MainWindow) 
            {
                MainMenuButton.Visibility = Visibility.Hidden;
            } else if (Owner is not MainWindow)
            {
                MainMenuButton.Visibility = Visibility.Visible;
            }
            ShowDialog();
        }

        // Changes owner of settings window when switching between main menu and songs window.
        public void ChangeSettingsOwner(object sender) 
        {
            Owner.Closing -= Owner_Closing;
            switch (sender)
            {
                case MainWindow:
                    _mainWindow = (MainWindow)sender;
                    Owner = _mainWindow;
                    Owner.Closing += Owner_Closing;
                    break;
                case SongsWindow:
                    _songsWindow = (SongsWindow)sender;
                    Owner = _songsWindow;
                    Owner.Closing += Owner_Closing;
                    break;
                case MidiPlayWindow:
                    _midiPlayWindow = (MidiPlayWindow)sender;
                    Owner = _midiPlayWindow;
                    Owner.Closing += Owner_Closing;
                    break;
                case HighscoreList:
                    _highscoreList = (HighscoreList)sender;
                    Owner = _highscoreList;
                    Owner.Closing += Owner_Closing;
                    break;
            }
        }

        // Gets the lightmode value of its current owner.
        private void DarkOrLightMode_OnClick(object sender, RoutedEventArgs e)
        {
            Lightmode = !Lightmode;
            CheckDarkOrLight(Owner);
        }

        // Checks lightmode value, and switches between dark- and lightmode.
        public void CheckDarkOrLight(object sender) 
        {
            if (Lightmode)
            {
                SetLightMode(sender);
            }
            else 
            {
                SetDarkMode(sender);
            }
        }

        // Code to change application to light mode.
        public void SetLightMode(object sender) 
        {
            // Makes sure the correct background is changed.
            switch (sender) 
            {
                case MainWindow:
                    _mainWindow.MainGrid.Background = Brushes.White;
                    break;
                case SongsWindow:
                    _songsWindow.SongsGrid.Background = Brushes.White;
                    _songsWindow.MenuPanelGrid.Background = Brushes.White;
                    break;
                case HighscoreList:
                    _highscoreList.SongsGrid.Background = Brushes.White;
                    break;
            }

            DarkOrLightMode.Content = "Light mode";
            SettingsGrid.Background = Brushes.White;
            SettingsText.Foreground = Brushes.Black;
        }

        public void SetDarkMode(object sender)
        {
            // Makes sure the correct background is changed.
            switch (sender) 
            {
                case MainWindow:
                    _mainWindow.MainGrid.Background = new SolidColorBrush(Color.FromRgb(25, 44, 49));
                    break;
                case SongsWindow:
                    _songsWindow.SongsGrid.Background = new SolidColorBrush(Color.FromRgb(25, 44, 49));
                    _songsWindow.MenuPanelGrid.Background = new SolidColorBrush(Color.FromRgb(25, 44, 49));
                    break;
                case HighscoreList:
                    _highscoreList.SongsGrid.Background = new SolidColorBrush(Color.FromRgb(25, 44, 49));
                    break;
            }

            DarkOrLightMode.Content = "Dark mode";
            SettingsGrid.Background = new SolidColorBrush(Color.FromRgb(25, 44, 49));
            SettingsText.Foreground = Brushes.White;
        }
        public bool GetLightMode()
        {
            return Lightmode;
        }

        // Closes entire application when exit button is pressed.
        private void ExitButton_OnClick(object sender, RoutedEventArgs e) 
        {
            Environment.Exit(0);
        }

        // Calls method to go back to main menu.
        private void MainMenuButton_OnClick(object sender, RoutedEventArgs e) 
        {
            MainMenu();
        }

        // Logic to go back to main menu.
        public void MainMenu() 
        {
            Window previousOwner = Owner;
            MainWindow mainWindow = new MainWindow(this);
            mainWindow.Show();
            previousOwner.Close();
            Visibility = Visibility.Hidden;
        }
    }
}