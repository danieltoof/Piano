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
using System.Diagnostics.Eventing.Reader;
using System.Windows.Forms;

namespace InEenNotendop.UI
{
    /// <summary>
    /// Interaction logic for SongsWindow.xaml
    /// </summary>
    public partial class SongsWindow : Window
    {
        private SettingsWindow settingsWindow;
        private DataProgram dataProgram;


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

                    // Use the MoeilijkheidConverter to get the readable Moeilijkheid text
                    MoeilijkheidConverter moeilijkheidConverter = new MoeilijkheidConverter();
                    string moeilijkheidText = moeilijkheidConverter.Convert(nummer.Moeilijkheid, typeof(string), null, CultureInfo.InvariantCulture) as string;

                    //MessageBox.Show($"Clicked on Nummer with ID: {nummerId}\nDifficulty: {moeilijkheidText}", nummerId.ToString(), MessageBoxButton.YesNo);

                    // MessageBox met Yes/No buttons (zou ideaal zijn als we er een 3e kunnen maken en ze andere namen kunnen geven) - "Yes" gaat nu naar Play scherm en speelt de meegegeven midi
                    if (System.Windows.MessageBox.Show($"Clicked on Nummer with ID: {nummerId}\nDifficulty: {moeilijkheidText}", nummerId.ToString(), MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        PlayWindow playWindow = new PlayWindow();
                        playWindow.Show();
                        Close();
                        //playWindow.StartPlay(@"..\..\..\..\midi-test\midis\07_Flower_Garden_GM.mid"); //TODO: geef variabele mee
                    }
                    else
                    {
                        // If no
                    }
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
            ImportWindow import = new ImportWindow();
            import.ShowDialog();
            Nummer.ItemsSource = dataProgram.MaakLijst();
        }
    }
}
