using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
        public System.Windows.Window Owner { get; set; }
        private MainWindow mainWindow;
        public SettingsWindow(MainWindow mainWindowGive)
        {
            mainWindow = mainWindowGive;
            InitializeComponent();
    
        }

        private void DarkOrLightMode_OnClick(object sender, RoutedEventArgs e)
        {
            CheckDarkOrLight();
        }

        private void CheckDarkOrLight() // veranderd light mode naar dark mode en dark mode naar light mode
        {
            if (mainWindow.lightmode == 1)
            {
                SetLightMode();
            }
            else if (mainWindow.lightmode == 0)
            {
                SetDarkMode();
            }
        }


        public void SetLightMode() // zet hier alle dingen die veranderen van kleur
        {
            //Alles van Start menu
            mainWindow.MainGrid.Background = Brushes.White;
            DarkOrLightMode.Content = "Light mode";




            //Alles van main settings menu
            SettingsGrid.Background = Brushes.White;
            SettingsText.Foreground = Brushes.Black;


            //Setten van lightmode
            mainWindow.lightmode = 0;
        }

        public void SetDarkMode()
        {

            mainWindow.MainGrid.Background = new SolidColorBrush(Color.FromRgb(25, 44, 49));
            DarkOrLightMode.Content = "Dark mode";

           


            //Alles van main settings menu
            SettingsGrid.Background = new SolidColorBrush(Color.FromRgb(25, 44, 49));
            SettingsText.Foreground = Brushes.White;

            //Setten van lightmode 
            mainWindow.lightmode = 1;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
            e.Cancel = true;
        }
    }
}
