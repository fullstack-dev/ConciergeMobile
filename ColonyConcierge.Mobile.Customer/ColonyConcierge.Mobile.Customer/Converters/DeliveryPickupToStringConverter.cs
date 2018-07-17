using ColonyConcierge.APIData.Data;
using ColonyConcierge.Client;
using ColonyConcierge.Mobile.Customer.Localization.Resx;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ColonyConcierge.Mobile.Customer
{
    public class DeliveryPickupToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is RestaurantVM)
            {
                RestaurantVM restaurantVM = value as RestaurantVM;
                if (!restaurantVM.Closed)
                {
                    if (restaurantVM.SearchResultData.IsDeliveryAvailable && restaurantVM.SearchResultData.IsPickupAvailable)
                    {
						return string.Empty;
                    }
                    if (restaurantVM.SearchResultData.IsDeliveryAvailable)
                    {
						return string.Empty;
                    }
                    if (restaurantVM.SearchResultData.IsPickupAvailable)
                    {
                        return AppResources.PickupOnly;
                    }
                }
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
