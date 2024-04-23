using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using InEenNotendop.Data;
using System.Diagnostics;

namespace InEenNotendop.UI
{
    /// <summary>
    /// Interaction logic for SongsWindow.xaml
    /// </summary>
    public partial class SongsWindow : Window
    {
        private SettingsWindow settingsWindow;


        public SongsWindow(SettingsWindow settingsWindow)
        {
            InitializeComponent();
            DataProgram dataProgram = new DataProgram();
            dataProgram.StartDataBase();
            Nummer.ItemsSource = dataProgram.MaakLijst();
            this.settingsWindow = settingsWindow;
            this.settingsWindow.ChangeSettingsOwner(this);

            

            CheckDarkOrLight();

        }
        private void SettingsButton_OnClick(object sender, RoutedEventArgs e)
        {
            settingsWindow.OpenSettings();
        }

        private void OnNumberClicked(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement clickedElement)
            {
                var nummer = clickedElement.DataContext as Nummer;
                if (nummer != null)
                {
                    int nummerId = nummer.Id;
                    MessageBox.Show($"Clicked on Nummer with ID: {nummerId}");
                }

            }
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
