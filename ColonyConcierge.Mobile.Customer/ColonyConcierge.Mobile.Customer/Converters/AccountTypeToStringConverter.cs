using ColonyConcierge.APIData.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using ColonyConcierge.Mobile.Customer.Localization.Resx;

namespace ColonyConcierge.Mobile.Customer
{
    class AccountTypeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IEnumerable<SubscriptionPlan>)
            {
                var plans = value as IEnumerable<SubscriptionPlan>;
				IList<string> plansToReturn = plans.Select(x =>
				{
					string text = x.Name + " ("
					               + (x.AnnualPrice.HasValue ? ("$" + Math.Round(x.AnnualPrice.Value) + " / year") : AppResources.Variable)
					               + ")\n" + x.Description;

					return text;
				}).ToList();

                return plansToReturn;
            }
            return System.Convert.ToString(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
