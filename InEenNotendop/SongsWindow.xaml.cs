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


            FilterBox.Items.Add("No Filter");
            FilterBox.Items.Add("Easy");
            FilterBox.Items.Add("Medium");
            FilterBox.Items.Add("Hard");


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

                    MessageBox.Show($"Clicked on Nummer with ID: {nummerId}\nDifficulty: {moeilijkheidText}");
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


        private void FilterBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string Filter = (sender as ComboBox).SelectedItem as string;
            int Difficulty = 0;
            switch (Filter)
            {
                case "Easy":
                    Difficulty = 1;
                    break;
                case "Medium":
                    Difficulty = 2;
                    break;
                case "Hard":
                    Difficulty = 3;
                    break;
                case "No FIlter":
                    Difficulty = 0;
                    break;
            }

            if (Difficulty != 0)
            {
                Nummer.ItemsSource = dataProgram.MaakFilteredLijst(Difficulty);
            }
            else
            {
                Nummer.ItemsSource = dataProgram.MaakLijst();
            }
            
        }
    }
}
