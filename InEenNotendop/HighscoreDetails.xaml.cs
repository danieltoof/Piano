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
    /// Interaction logic for HighscoreDetails.xaml
    /// </summary>
    public partial class HighscoreDetails : Window
    {
        private int nummerId;
        private string moeilijkheidText;
        private string artiest;
        private int fullTime;
        private int bpm;
        private string filePath;
        private string convertedTime;
        private HighscoreList highscoreList;
        private int currentScore;

        public HighscoreDetails()
        {
            InitializeComponent();
        }

        public HighscoreDetails(int nummerId, string moeilijkheidText, string title, string artiest, int fullTime, int bpm, string filePath, string convertedTime, HighscoreList highscorepage, int currentScore)
        {
            this.nummerId = nummerId;
            this.moeilijkheidText = moeilijkheidText;
            Title = title;
            this.artiest = artiest;
            this.fullTime = fullTime;
            this.bpm = bpm;
            this.filePath = filePath;
            this.convertedTime = convertedTime;
            this.highscoreList = highscorepage;
            this.currentScore = currentScore;
        }
    }
}
