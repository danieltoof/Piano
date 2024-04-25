using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace InEenNotendop.UI
{
    public class MoeilijkheidConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int moeilijkheid)
            {
                // Directly convert the moeilijkheid value to readable text
                switch (moeilijkheid)
                {
                    case 1:
                        return "easy";
                    case 2:
                        return "medium";
                    case 3:
                        return "hard";
                    default:
                        return "Unknown";
                }
            }

            return "Unknown";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
