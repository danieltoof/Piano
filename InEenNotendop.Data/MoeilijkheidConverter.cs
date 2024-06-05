using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace InEenNotendop.Data
{
    public class MoeilijkheidConverter
    {
        public string Convert(int Moeilijkheid)
        {
            // Directly convert the moeilijkheid value to readable text
            switch (Moeilijkheid)
            {
                case 1:
                    return "Easy";
                case 2:
                    return "Medium";
                case 3:
                    return "Hard";
                default:
                    return "Unknown";
            }
        }
    }
}
