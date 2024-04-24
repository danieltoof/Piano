using System;
using System.Collections.Generic;
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
    /// Interaction logic for SongsWindow.xaml
    /// </summary>
    public partial class SongsWindow : Window
    {
        private SettingsWindow settingsWindow;
        
        public SongsWindow(SettingsWindow settingsWindow)
        {
            InitializeComponent();
            
            this.settingsWindow = settingsWindow;
            this.settingsWindow.ChangeSettingsOwner(this);

            CheckDarkOrLight();

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

        private void ImportButton_OnClick(object sender, RoutedEventArgs e)
        {
            ImportWindow import = new ImportWindow();
            import.ShowDialog();
        }
    }
}
