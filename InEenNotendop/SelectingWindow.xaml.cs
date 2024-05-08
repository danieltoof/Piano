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
    /// Interaction logic for SelectingWindow.xaml
    /// </summary>
    public partial class SelectingWindow : Window
    {
        public Window Owner { get; set; }
        private SongsWindow songsWindow;
        public SelectingWindow(int nummerId, string moeilijkheidText, string Title, string Artiest, int Lengte, int Bpm, object sender)
        {
            InitializeComponent();
            songsWindow = (SongsWindow)sender;
            Owner = songsWindow;
            DataContext = new NummerDetailsViewModel(nummerId, moeilijkheidText, Title, Artiest, Lengte, Bpm);
        }

        private void OnCloseClicked(object sender, RoutedEventArgs e)
        {
            Close();
        }


        public class NummerDetailsViewModel
        {
            public string NummerIdText { get; }
            public string MoeilijkheidText { get; }
            public string Title { get; }
            public string Artiest { get; }
            public int Lengte { get; }
            public int Bpm { get; }

            public NummerDetailsViewModel(int nummerId, string moeilijkheidText, string title, string artiest, int lengte, int bpm)
            {
                NummerIdText = $"Clicked on Nummer with ID: {nummerId}";
                MoeilijkheidText = $"Difficulty: {moeilijkheidText}";
                Title = title;
                Artiest = artiest ;
                Lengte = lengte;
                Bpm = bpm ;

            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            PlayWindow playWindow = new PlayWindow();
            //SongsWindow songsWindow = SongsWindow();
            Owner.Close();
            playWindow.Show();
            Close();
            //SongsWindow.Close();
            //playWindow.StartPlay(@"..\..\..\..\midi-test\midis\Coldplay - Viva La Vida.mid"); //TODO: geef variabele mee
        }
    }
}
