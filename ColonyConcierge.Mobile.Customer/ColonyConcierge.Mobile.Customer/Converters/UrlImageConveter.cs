using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ColonyConcierge.Mobile.Customer
{
	public class UrlImageConveter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var image = System.Convert.ToString(value);
			if (string.IsNullOrEmpty(image))
			{
				return "";
			}

			return Task.Run(() =>
			{
				Uri uri = new Uri(image);
				var imageSource = new UriImageSource()
				{
					CachingEnabled = false,
					//CacheValidity = TimeSpan.FromMinutes(1),
				};
				imageSource.Uri = uri;
				return imageSource;
			}).Result;
			//return PCLAppConfig.ConfigurationManager.AppSettings["ImageBaseUrl"] + System.Convert.ToString(value);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}

