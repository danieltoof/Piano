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
        internal int lightmode;
        public Window Owner { get; set; }
        private MainWindow mainWindow;
        private SongsWindow songWindow;
        private MidiPlayWindow midiPlayWindow;
        private int isOkToClose = 0;
        private DataProgram data = new DataProgram();
        
        public SettingsWindow(object sender)
        {
            InitializeComponent();
            switch (sender)
            {
                case MainWindow:
                    mainWindow = (MainWindow)sender;
                    Owner = mainWindow;
                    break;
                case SongsWindow:
                    songWindow = (SongsWindow)sender;
                    Owner = songWindow;
                    break;
                case MidiPlayWindow:
                    midiPlayWindow = (MidiPlayWindow)sender;
                    Owner = midiPlayWindow;
                    Owner.Closing += Owner_Closing;
                    break;
            }
            Owner.Closing += Owner_Closing;
        }

        // Makes sure settings window is closed when application is closed
        private void Owner_Closing(object? sender, CancelEventArgs e) 
        {
            if (sender.Equals(Owner))
            {
                isOkToClose = 1;
                Close();
            }
        }

        // Makes sure settings window is not closed when not needed
        protected override void OnClosing(CancelEventArgs e) 
        {
            if (isOkToClose != 1)
            {
                this.Visibility = Visibility.Hidden;
                e.Cancel = true;
            }
            else
            {
                data.StopSshTunnel();
                e.Cancel = false;
            }
        }

        // Opens settings window
        public void OpenSettings() 
        {
            // Makes sure main menu button is not visible when on main menu
            if (Owner is MainWindow) 
            {
                MainMenuButton.Visibility = Visibility.Hidden;
            } else if (Owner is not MainWindow)
            {
                MainMenuButton.Visibility = Visibility.Visible;
            }
            ShowDialog();
        }

        // Changes owner of settings window when switching between main menu and songs window
        public void ChangeSettingsOwner(object sender) 
        {
            Owner.Closing -= Owner_Closing;
            switch (sender)
            {
                case MainWindow:
                    mainWindow = (MainWindow)sender;
                    Owner = mainWindow;
                    Owner.Closing += Owner_Closing;
                    break;
                case SongsWindow:
                    songWindow = (SongsWindow)sender;
                    Owner = songWindow;
                    Owner.Closing += Owner_Closing;
                    break;
                case MidiPlayWindow:
                    midiPlayWindow = (MidiPlayWindow)sender;
                    Owner = midiPlayWindow;
                    Owner.Closing += Owner_Closing;
                    break;
            }
        }

        // Gets the lightmode value of its current owner
        private void DarkOrLightMode_OnClick(object sender, RoutedEventArgs e) 
        {
            CheckDarkOrLight(Owner);
        }

        // Checks lightmode value, and switches between dark- and lightmode
        private void CheckDarkOrLight(object sender) 
        {
            if (lightmode == 0)
            {
                SetLightMode(sender);
            }
            else if (lightmode == 1)
            {
                SetDarkMode(sender);
            }
        }

        // Code to change application to light mode
        public void SetLightMode(object sender) 
        {
            // Makes sure the correct background is changed
            switch (sender) 
            {
                case MainWindow:
                    mainWindow.MainGrid.Background = Brushes.White;
                    break;
                case SongsWindow:
                    songWindow.SongsGrid.Background = Brushes.White;
                    songWindow.MenuPanelGrid.Background = Brushes.White;
                    break;
            }

            DarkOrLightMode.Content = "Light mode";
            SettingsGrid.Background = Brushes.White;
            SettingsText.Foreground = Brushes.Black;

            lightmode = 1;
        }

        public void SetDarkMode(object sender)
        {
            // Makes sure the correct background is changed
            switch (sender) 
            {
                case MainWindow:
                    mainWindow.MainGrid.Background = new SolidColorBrush(Color.FromRgb(25, 44, 49));
                    break;
                case SongsWindow:
                    songWindow.SongsGrid.Background = new SolidColorBrush(Color.FromRgb(25, 44, 49));
                    songWindow.MenuPanelGrid.Background = new SolidColorBrush(Color.FromRgb(25, 44, 49));
                    break;
            }

            DarkOrLightMode.Content = "Dark mode";
            SettingsGrid.Background = new SolidColorBrush(Color.FromRgb(25, 44, 49));
            SettingsText.Foreground = Brushes.White;

            lightmode = 0;
        }
        public int GetLightMode()
        {
            return lightmode;
        }

        // Closes entire application when exit button is pressed
        private void ExitButton_OnClick(object sender, RoutedEventArgs e) 
        {
            Environment.Exit(0);
        }

        // Calls method to go back to main menu
        private void MainMenuButton_OnClick(object sender, RoutedEventArgs e) 
        {
            MainMenu();
        }

        // Logic to go back to main menu
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