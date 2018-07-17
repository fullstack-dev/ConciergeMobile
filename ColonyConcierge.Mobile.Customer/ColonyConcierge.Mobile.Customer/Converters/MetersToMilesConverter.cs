using ColonyConcierge.APIData.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ColonyConcierge.Mobile.Customer
{
    public class MetersToMilesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            decimal miles = 0;
            try
            {
                var meters = (decimal)value;
                miles = meters / (decimal)1609.344;
            }
            catch
            {
                miles = 0;
            }
            return string.Format("{0:0.00}", miles);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
