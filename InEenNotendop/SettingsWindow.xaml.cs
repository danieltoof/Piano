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
        private MidiPlayWindow midiPlayWindow;
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
                case MidiPlayWindow:
                    midiPlayWindow = (MidiPlayWindow)sender;
                    Owner = midiPlayWindow;
                    Owner.Closing += Owner_Closing;
                    break;
            }

            Owner.Closing += Owner_Closing;
        }

        private void Owner_Closing(object? sender, CancelEventArgs e)
        {
            if (sender.Equals(Owner))
            {
                isOkToClose = 1;
                Close();
            }

        }
        protected override void OnClosing(CancelEventArgs e)
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

        public void OpenSettings()
        {
            if (Owner is MainWindow)
            {
                MainMenuButton.Visibility = Visibility.Hidden;
            } else if (Owner is not MainWindow)
            {
                MainMenuButton.Visibility = Visibility.Visible;
            }
            ShowDialog();
        }


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

        private void DarkOrLightMode_OnClick(object sender, RoutedEventArgs e)
        {
            CheckDarkOrLight(Owner);
        }

        private void CheckDarkOrLight(object sender) // veranderd light mode naar dark mode en dark mode naar light mode
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


        public void SetLightMode(object sender) // zet hier alle dingen die veranderen van kleur
        {

            if (sender is MainWindow)
            {
                //Alles van Start menu
                mainWindow.MainGrid.Background = Brushes.White;
                
            }

            if (sender is SongsWindow)
            {
                //Alles van de songs menu
                songWindow.SongsGrid.Background = Brushes.White;
            }


            //Alles van main settings menu
            DarkOrLightMode.Content = "Light mode";
            SettingsGrid.Background = Brushes.White;
            SettingsText.Foreground = Brushes.Black;


            //Setten van lightmode
            lightmode = 1;
        }

        public void SetDarkMode(object sender)
        {

            if (sender is MainWindow)
            {
                mainWindow.MainGrid.Background = new SolidColorBrush(Color.FromRgb(25, 44, 49));
                
            }

            if (sender is SongsWindow)
            {
                songWindow.SongsGrid.Background = new SolidColorBrush(Color.FromRgb(25, 44, 49));
            }



            //Alles van main settings menu
            DarkOrLightMode.Content = "Dark mode";
            SettingsGrid.Background = new SolidColorBrush(Color.FromRgb(25, 44, 49));
            SettingsText.Foreground = Brushes.White;

            //Setten van lightmode 
            lightmode = 0;
        }

        public int GetLightMode()
        {
            return lightmode;
        }




        private void ExitButton_OnClick(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void MainMenuButton_OnClick(object sender, RoutedEventArgs e)
        {
            MainMenu();
        }

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
