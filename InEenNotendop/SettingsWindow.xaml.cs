using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
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
        private int isOkToClose = 0;
        
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
            }

            Owner.Closing += Owner_Closing;
        }

        private void Owner_Closing(object? sender, CancelEventArgs e) // Makes sure settings window is closed when application is closed
        {
            if (sender.Equals(Owner))
            {
                isOkToClose = 1;
                Close();
            }
        }
        protected override void OnClosing(CancelEventArgs e) // Makes sure settings window is not closed when not needed
        {
            if (isOkToClose != 1)
            {
                this.Visibility = Visibility.Hidden;
                e.Cancel = true;
            }
            else
            {
                e.Cancel = false;
            }
        }

        public void OpenSettings() // Opens settings window
        {
            if (Owner is MainWindow) // Makes sure main menu button is not visible when on main menu
            {
                MainMenuButton.Visibility = Visibility.Hidden;
            } else if (Owner is not MainWindow)
            {
                MainMenuButton.Visibility = Visibility.Visible;
            }
            ShowDialog();
        }


        public void ChangeSettingsOwner(object sender) // Changes owner of settings window when switching between main menu and songs window
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
            }

        }

        private void DarkOrLightMode_OnClick(object sender, RoutedEventArgs e) // Gets the lightmode value of its current owner
        {
            CheckDarkOrLight(Owner);
        }

        private void CheckDarkOrLight(object sender) // Checks lightmode value, and switches between dark- and lightmode
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


        public void SetLightMode(object sender) // Code to change application to light mode
        {
            switch (sender) // Makes sure the correct background is changed
            {
                case MainWindow:
                    mainWindow.MainGrid.Background = Brushes.White;
                    break;
                case SongsWindow:
                    songWindow.SongsGrid.Background = Brushes.White;
                    break;
            }

            DarkOrLightMode.Content = "Light mode";
            SettingsGrid.Background = Brushes.White;
            SettingsText.Foreground = Brushes.Black;

            lightmode = 1;
        }

        public void SetDarkMode(object sender)
        {
            switch (sender) // Makes sure the correct background is changed
            {
                case MainWindow:
                    mainWindow.MainGrid.Background = new SolidColorBrush(Color.FromRgb(25, 44, 49)); ;
                    break;
                case SongsWindow:
                    songWindow.SongsGrid.Background = new SolidColorBrush(Color.FromRgb(25, 44, 49)); ;
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

        private void ExitButton_OnClick(object sender, RoutedEventArgs e) // Closes entire application when exit button is pressed
        {
            Environment.Exit(0);
        }

        private void MainMenuButton_OnClick(object sender, RoutedEventArgs e) // Calls method to go back to main menu
        {
            MainMenu();
        }

        public void MainMenu() // Logic to go back to main menu
        {
            Window previousOwner = Owner;
            MainWindow mainWindow = new MainWindow(this);
            mainWindow.Show();
            previousOwner.Close();
            Visibility = Visibility.Hidden;
        }
    }
}
