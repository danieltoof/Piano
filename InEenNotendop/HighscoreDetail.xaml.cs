using InEenNotendop.Data;
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
using InEenNotendop.Business;

namespace InEenNotendop.UI
{
    /// <summary>
    /// Interaction logic for HighscoreDeatail.xaml
    /// </summary>
    public partial class HighscoreDetail : Window
    {
        private int _id;
        private string _difficulty;
        private string _title;
        private string _artist;
        private string _lenght;
        private string _name;

        private PianoHeroService _pianoHeroService = new PianoHeroService(new SqlDataAccess());
        public HighscoreDetail(int nummerId, string difficultyText, string title, string artist, string convertedTime, HighscoreList highscoreList, string name)
        {
            InitializeComponent();

            this._id = nummerId;
            this._difficulty = difficultyText;
            this._title = title;
            this._artist = artist;
            this._lenght = convertedTime;
            this._name = name;

            GetAllScores(_id);

        }

        private void GetAllScores(int id)
        {
            AllScores.ItemsSource = _pianoHeroService.GetAllScores(id);
        }
    }
}