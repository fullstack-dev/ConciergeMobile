using ColonyConcierge.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using ColonyConcierge.APIData.Data;
using Newtonsoft.Json;
using Plugin.Settings;

namespace ColonyConcierge.Mobile.Customer
{
    public class Shared
    {
		public static bool IsLoggedIn
		{
			get
			{
				return !string.IsNullOrEmpty(LoginToken);
			}
		}

        public static string LoginToken
		{
			get
			{

				return CrossSettings.Current.GetValueOrDefault("Token", string.Empty);
			}
			set
			{
				CrossSettings.Current.AddOrUpdateValue("Token", value);
			}
		}

		public static string NotificationToken
		{
			get
			{
				try
				{
					return CrossSettings.Current.GetValueOrDefault("NotificationToken", string.Empty);
				}
				catch (Exception)
				{
					return string.Empty;
				}
			}
			set
			{
				try
				{
					CrossSettings.Current.AddOrUpdateValue("NotificationToken", value);
				}
				catch (Exception) { }
			}
		}

		public static int UserId
		{
			get
			{
				return CrossSettings.Current.GetValueOrDefault<int>("UserId", -1);
			}
			set
			{
				CrossSettings.Current.AddOrUpdateValue<int>("UserId", value);
			}
		}

		public static void AddPreviouslyUsedAddress(ExtendedAddress value)
		{
			if (value != null)
			{
				var previouslyUsedAddresses = Shared.PreviouslyUsedAddresses;
				var firstOrDefault = previouslyUsedAddresses.FirstOrDefault(t => t.BasicAddress.ToAddressLine() == value.BasicAddress.ToAddressLine());
				if (firstOrDefault == null)
				{
					previouslyUsedAddresses.Insert(0, value);
				}
				else
				{
					previouslyUsedAddresses.Remove(firstOrDefault);
					previouslyUsedAddresses.Insert(0, value);
				}
				Shared.PreviouslyUsedAddresses = previouslyUsedAddresses.Take(10).ToList();
			}
		}

		public static List<ExtendedAddress> PreviouslyUsedAddresses
		{
			get
			{

				try
				{
					return JsonConvert.DeserializeObject<List<ExtendedAddress>>(CrossSettings.Current.GetValueOrDefault<string>("PreviouslyUsedAddresses", "[]"));
				}
				catch (Exception) { }
				return new List<ExtendedAddress>();
			}
			set
			{
				CrossSettings.Current.AddOrUpdateValue("PreviouslyUsedAddresses", JsonConvert.SerializeObject(value));
			}
		}

		public static ExtendedAddress LocalAddress
		{
			get
			{

				try
				{
					var address = CrossSettings.Current.GetValueOrDefault<string>("LocalAddress", null);
					return JsonConvert.DeserializeObject<ExtendedAddress>(address);
				}
				catch (Exception) { }
				return null;
			}
			set
			{
				AddPreviouslyUsedAddress(value);
				CrossSettings.Current.AddOrUpdateValue("LocalAddress", JsonConvert.SerializeObject(value));
			}
		}

		public static bool Firstlaunch
		{
			get
			{
				return CrossSettings.Current.GetValueOrDefault<bool>("firstlaunch", true);
			}
			set
			{
				CrossSettings.Current.AddOrUpdateValue("firstlaunch", value);
			}
		}

		public static APIs APIs
		{
			get
			{
				var connector = new Connector();
				var api = new APIs(connector);
				api.LoginToken = LoginToken;
				return api;
			}
		}

		public static void SendRegistrationToServer(string token, string platform, string uniqueDeviceId)
		{
			try
			{
				if (!string.IsNullOrEmpty(token) && Shared.IsLoggedIn && token != Shared.NotificationToken)
				{
					var isDeviceTokenSet = Shared.APIs.IUsers.AddDeviceToken(Shared.UserId, platform, token, uniqueDeviceId);
					if (isDeviceTokenSet)
					{
						Shared.NotificationToken = token;
					}
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("SendRegistrationToServer token: " + ex.Message);
			}
		}
    }
}
