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
using System.Globalization;

namespace InEenNotendop.UI
{
    /// <summary>
    /// Interaction logic for SongsWindow.xaml
    /// </summary>
    public partial class SongsWindow : Window
    {
        private SettingsWindow settingsWindow;
        private DataProgram dataProgram;
        private int lightmodeImport;


        public SongsWindow(SettingsWindow settingsWindow)
        {
            InitializeComponent();
            dataProgram = new DataProgram();
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
                    String Title = nummer.Title;
                    String Artiest = nummer.Artiest;
                    int Lengte = nummer.Lengte;
                    int Bpm = nummer.Bpm;
                    MoeilijkheidConverter moeilijkheidConverter = new MoeilijkheidConverter();
                    string moeilijkheidText = moeilijkheidConverter.Convert(nummer.Moeilijkheid, typeof(string), null, CultureInfo.InvariantCulture) as string;

                    SelectingWindow detailsWindow = new SelectingWindow(nummerId, moeilijkheidText, Title, Artiest, Lengte, Bpm);
                    detailsWindow.Owner = this;
                    detailsWindow.ShowDialog();
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

        private void ImportButton_OnClick(object sender, RoutedEventArgs e)
        {
            lightmodeImport = settingsWindow.lightmode;
            ImportWindow import = new ImportWindow(lightmodeImport);
            import.ShowDialog();
            Nummer.ItemsSource = dataProgram.MaakLijst();
        }
    }
}
