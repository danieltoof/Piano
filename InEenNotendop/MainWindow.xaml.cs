﻿using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace InEenNotendop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int lightmode;
        public MainWindow()
        {

            InitializeComponent();
            CheckLightMode();   
            CheckDarkOrLight();

        }

        public int CheckLightMode() // checkt systeem instellingen
        {
            return lightmode = (int)Microsoft.Win32.Registry.GetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize", "AppsUseLightTheme", 1);
        }


        private void StartButton_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ExitButton_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SettingsButton_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void DarkOrLightMode_OnClick(object sender, RoutedEventArgs e)
        {
            CheckDarkOrLight();
        }

        private void CheckDarkOrLight() // veranderd light mode naar dark mode en dark mode naar light mode
        {
            if (lightmode == 1)
            {
                SetLightMode();
            }
            else if (lightmode == 0)
            {
                SetDarkMode();
            }
        }


        public void SetLightMode() // zet hier alle dingen die veranderen van kleur
        {
            MainGrid.Background = Brushes.White;
            DarkOrLightMode.Content = "Light mode";

            MainTextBlock.Foreground = Brushes.Black;

            lightmode = 0;
        }

        public void SetDarkMode()
        {
            MainGrid.Background = new SolidColorBrush(Color.FromRgb(25, 44 ,49));
            DarkOrLightMode.Content = "Dark mode";

            MainTextBlock.Foreground = Brushes.White;

            lightmode = 1;
        }
    }
}