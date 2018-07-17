using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ColonyConcierge.APIData.Data.Logistics.NotificationData;
using System.Linq;
using Xamarin.Forms;
using ColonyConcierge.Mobile.Customer;
using ColonyConcierge.Mobile.Customer.Localization.Resx;
using ColonyConcierge.APIData.Data;
using ZXing.Mobile;

namespace ColonyConcierge.Mobile.Customer
{
	public abstract class AppServicesBase : IAppServices
	{
		protected readonly string AnalyticsSetNameEvent = "set";
		protected readonly string AnalyticsSendNameEvent = "send";
		protected readonly string AnalyticsEcommerceNameEvent = "ecommerce_purchase";

		public virtual double LastLatitude
		{
			get;
			set;
		}
		public virtual double LastLongitude
		{
			get;
			set;
		}

		public abstract string AppVersion
		{
			get;
		}

		public abstract string UniqueDeviceId
		{
			get;
		}

		public abstract bool IsCanShow
		{
			get;
		}

		public abstract void Vibration(int milliseconds);
		public abstract void SetNetworkBar(bool isVisible);
		public abstract void SetShowStatus(bool isVisible);
		public abstract void GetNoncenBrainTree(BrainTreeCard brainTreeCard, Action<object> action);
		public abstract CreditCardType GetCreditCardType(string cardNumber);
		public abstract TimeZoneInfo GetTimeZoneById(string id);
		public abstract void TrackPage(string page);
		public abstract void TrackEvent(string eventCategory, string eventAction, string eventLabel);
		public abstract void TrackOrder(string id, string affiliation);
		public abstract void TrackOrder(string id, string affiliation, double revenue, double shipping, double tax);
		public abstract void AddServiceNotAvaible(string id, string fullname, string email, ExtendedAddress address);
		public abstract void CheckLocationPermission(Action<bool> locationRequestAction);
		public abstract void GetLocation(Action<bool> action, bool isRefresh = false);
		public abstract string GetRegistrationNotificationId();
		public abstract void RaiseAppOnLocationChanged(bool isLocation);
		public static readonly List<LogisticsNotification> LogisticsNotifications = new List<LogisticsNotification>();
		public List<LogisticsNotification> LogisticsNotificationsReadOnly
		{
			get
			{
				return LogisticsNotifications;
			}
		}

		//private static Dictionary<LogisticsNotification, IDisposable> UserDialogConfirms = new Dictionary<LogisticsNotification, IDisposable>();

		private bool ContainNotification(LogisticsNotification logisticsNotification)
		{
			return LogisticsNotifications.FirstOrDefault((arg) => arg.Kind == logisticsNotification.Kind
									   && (arg.IDs.Count() == logisticsNotification.IDs.Count())
			                                             && !arg.IDs.Except(logisticsNotification.IDs).Any()) != null;
		}

		public async Task ShowNotifitionAlert(string message)
		{
			var mainPage = Xamarin.Forms.Application.Current != null ? Xamarin.Forms.Application.Current.MainPage : null;
			if (!(mainPage is HomePage))
			{
				await Task.Run(() =>
				{
					while (!(mainPage is HomePage))
					{
						new System.Threading.ManualResetEvent(false).WaitOne(50);
							mainPage = Xamarin.Forms.Application.Current != null ? Xamarin.Forms.Application.Current.MainPage : null;
					}
				});
			}
			var lastPage = ((mainPage as HomePage).Detail as NavigationPage).CurrentPage;
			if (lastPage == null)
			{
				lastPage = mainPage;
			}

			await mainPage.DisplayAlert(AppResources.AppVersion, message, AppResources.OK);
		}

		public async Task ShowNotifitionAlert(LogisticsNotification logisticsNotification)
		{
			if (Shared.IsLoggedIn)
			{
				if (!ContainNotification(logisticsNotification))
				{
					LogisticsNotifications.Add(logisticsNotification);
					var mainPage = Xamarin.Forms.Application.Current != null ? Xamarin.Forms.Application.Current.MainPage : null;
					if (mainPage == null || mainPage.Navigation == null)
					{
						await Task.Run(() =>
						{
							while (mainPage == null || mainPage.Navigation == null)
							{
								new System.Threading.ManualResetEvent(false).WaitOne(50);
								mainPage = Xamarin.Forms.Application.Current != null ? Xamarin.Forms.Application.Current.MainPage : null;
							}
						});
					}

					var lastPape = mainPage;
					if (mainPage is MasterDetailPage)
					{
						lastPape = (mainPage as MasterDetailPage).Detail;
					}

					if (lastPape != null && logisticsNotification != null)
					{
						try
						{
							if (logisticsNotification.IDs != null 
							    && logisticsNotification.Kind != NotificationKind.OrderPlaced
							    && logisticsNotification.IDs.Count > 0)
							{
								string message = (Device.RuntimePlatform == Device.iOS ? "\n" : "") + logisticsNotification.Message;
								var view = await lastPape.DisplayAlert(AppResources.OrderStatus, message, AppResources.View, AppResources.OK);
								if (view)
								{
									var serviceRequestDetailsPage = new ServiceRequestDetailsPage(logisticsNotification.IDs.FirstOrDefault());
									await lastPape.Navigation.PushAsync(serviceRequestDetailsPage);
								}
							}
							LogisticsNotifications.Remove(logisticsNotification);
						}
						catch (Exception)
						{
							LogisticsNotifications.Remove(logisticsNotification);
							await ShowNotifitionAlert(logisticsNotification);
						}
					}
					else
					{
						LogisticsNotifications.Remove(logisticsNotification);
					}
				}
			}
		}

		public Task<ZXing.Result> ScanQRCode()
		{
			var scanner = new ZXing.Mobile.MobileBarcodeScanner();
			return scanner.Scan();
		}

		public void ScanQRCodeContinuously(Func<IMobileBarcodeScanner, ZXing.Result, bool> action)
		{
			var scanner = new ZXing.Mobile.MobileBarcodeScanner();
			scanner.ScanContinuously((result) =>
			{
				//scanner.Torch(true);
				var url = result.Text;
				if (action == null || action(scanner, result))
				{
					scanner.Cancel();
				}
			});
			//scanner.Torch(true);
		}
	}
}
