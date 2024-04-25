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
        public int Lengte { get; set; }
        public int Bpm { get; set; }
        public int Moeilijkheid { get; set; }
        public int Id { get; set; }

        public Nummer(string title, string artiest, int lengte, int bpm, int moeilijkheid, int id)
        {
            Title = title;
            Artiest = artiest;
            Lengte = lengte;
            Bpm = bpm;
            Moeilijkheid = moeilijkheid;
            Id = id;
        }
    }
}
