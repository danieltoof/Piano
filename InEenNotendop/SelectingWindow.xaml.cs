using InEenNotendop.Data;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
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
        string FilePath;
        public SelectingWindow(int nummerId, string moeilijkheidText, string Title, string Artiest, string Lengte, int Bpm, string FilePath, object sender)
        {
            InitializeComponent();
            songsWindow = (SongsWindow)sender;
            Owner = songsWindow;
            this.FilePath = FilePath;
            DataContext = new NummerDetailsViewModel(nummerId, moeilijkheidText, Title, Artiest, Lengte, Bpm);
            FillDataGrid(nummerId);
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
            public string Lengte { get; }
            public int Bpm { get; }

            public NummerDetailsViewModel(int nummerId, string moeilijkheidText, string title, string artiest, string lengte, int bpm)
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
            PlayWindow playWindow = new PlayWindow(FilePath, this);
            //SongsWindow songsWindow = SongsWindow();
            Owner.Close();
            playWindow.Show();
            Close();
            //SongsWindow.Close();
            //playWindow.StartPlay(@"..\..\..\..\midi-test\midis\Coldplay - Viva La Vida.mid"); //TODO: geef variabele mee
        }

        private void FillDataGrid(int nummerId)
        {
            string user = System.Net.Dns.GetHostName() + "\\" + Environment.UserName;
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = "(localdb)\\MSSQLLocalDB";
            builder.IntegratedSecurity = true;
            builder.UserID = user;
            builder.Password = "";
            builder.ApplicationIntent = ApplicationIntent.ReadWrite;

            string CmdString = string.Empty;
            using (SqlConnection con = new SqlConnection(builder.ConnectionString))
            {
                CmdString = $"USE PianoHeroDB \n SELECT score, SpelerID FROM HighScore WHERE NummerID = '{nummerId}'";
                SqlCommand cmd = new SqlCommand(CmdString, con);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                HighScoresGrid.ItemsSource = dt.DefaultView;
            }
        }
    }
}
