using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InEenNotendop.Data
{
    public class Nummer
    {
        public string Title { get; set; }
        public string Artiest { get; set; }
        public string FullTime { get; set; }
        public int Bpm { get; set; }
        public int Moeilijkheid { get; set; }
        public int Id { get; set; }
        public string Filepath { get; set; }
        public int Score { get; set; }

        public Nummer(string title, string artiest, string fulltime, int bpm, int moeilijkheid, int id, string filepath, int score)
        {
            Title = title;
            Artiest = artiest;
            FullTime = fulltime;
            Bpm = bpm;
            Moeilijkheid = moeilijkheid;
            Id = id;
            Filepath = filepath;
            Score = score;
        }
    }

}
