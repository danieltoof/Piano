using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InEenNotendop.Business
{
    public class Nummer
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Artiest { get; set; }
        public int Lengte { get; set; }
        public int Bpm { get; set; }
        public int Moeilijkheid { get; set; }

        
    }
}
