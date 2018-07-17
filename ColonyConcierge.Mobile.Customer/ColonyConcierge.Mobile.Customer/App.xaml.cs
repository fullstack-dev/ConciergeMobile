using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using Xamarin.Forms;
using PCLAppConfig;
using System.IO;
using ColonyConcierge.Client;
using System.Globalization;
using ColonyConcierge.Mobile.Customer.Localization.Resx;
using ColonyConcierge.APIData.Data.Logistics.NotificationData;
using Xamarin.Forms.Xaml;

namespace ColonyConcierge.Mobile.Customer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class App : Application
    {
		public event EventHandler Started;

		protected App()
		{
			Assembly assembly = typeof(App).GetTypeInfo().Assembly;
			Stream configStream = assembly.GetManifestResourceStream("ColonyConcierge.Mobile.Customer.App.config");
			try
			{
				using (configStream)
				{
					ConfigurationManager.Initialise(configStream);
				}
			}
			catch (Exception e)
			{
				System.Diagnostics.Debug.WriteLine(e.ToString());
			}

			InitializeComponent();

			this.PropertyChanged += (sender, e) =>
			{
				if (e.PropertyName == nameof(this.MainPage))
				{
					var appServices = DependencyService.Get<IAppServices>();
					appServices.SetNetworkBar(false);
				}
			};
		}

		public App(int destinationId) : this()
		{
			Shared.LocalAddress = null;

			var restaurantsTabPage = new RestaurantsTabPage(destinationId);
			var homePage = new HomePage(restaurantsTabPage);
			Application.Current.MainPage = homePage;

			MainPage.SetValue(NavigationPage.BarTextColorProperty, Color.White);
			MainPage.SetValue(NavigationPage.BackButtonTitleProperty, AppResources.Back);
		}

		public App(LogisticsNotification logisticsNotification = null) : this()
        {
			if (Shared.IsLoggedIn && logisticsNotification != null && logisticsNotification.IDs != null && logisticsNotification.IDs.Count > 0)
			{
				Shared.LocalAddress = null;
				AddressSuggestionListPage addressSuggestionListPage = new AddressSuggestionListPage(null, (obj) =>
				{
					if (obj != null)
					{
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            Application.Current.MainPage = new HomePage();
                        });
						
					}
				})
				{
					IsShowQRCode = true
				};
				var appServices = DependencyService.Get<IAppServices>();

				var serviceRequestDetailsPage = new ServiceRequestDetailsPage(logisticsNotification.IDs.FirstOrDefault());
				var customNavigationPage = new CustomNavigationPage(serviceRequestDetailsPage);
				Application.Current.MainPage = customNavigationPage;
				serviceRequestDetailsPage.ShowBack(customNavigationPage, addressSuggestionListPage);
				serviceRequestDetailsPage.Back += () =>
				{
					if (Shared.IsLoggedIn)
					{
						// Take to Home screen
						//MainPage = new HomePage();
						AddressSuggestionListPage newAddressSuggestionListPage = new AddressSuggestionListPage(null, (obj) =>
						{
							if (obj != null)
							{
								Application.Current.MainPage = new HomePage();
							}
						})
						{
							IsShowQRCode = true
						};
						appServices.TrackPage(newAddressSuggestionListPage.GetType().Name);
						Application.Current.MainPage = new NavigationPage(newAddressSuggestionListPage);
					}
					else
					{
						Shared.LocalAddress = null;
						var mainPage = new ColonyConcierge.Mobile.Customer.MainPage(true);
						Application.Current.MainPage = mainPage;
					}
				};
			}
			else
			{
				if (Shared.IsLoggedIn)
				{
					Shared.LocalAddress = null;
					// Take to Home screen
					//MainPage = new HomePage();
					AddressSuggestionListPage addressSuggestionListPage = new AddressSuggestionListPage(null, (obj) =>
					{
						if (obj != null)
						{
							Application.Current.MainPage = new HomePage();
						}
					})
					{
						IsShowQRCode = true
					};
					var appServices = DependencyService.Get<IAppServices>();
					appServices.TrackPage(addressSuggestionListPage.GetType().Name);
					Application.Current.MainPage = new NavigationPage(addressSuggestionListPage);
				}
				else
				{
					Shared.LocalAddress = null;
					var mainPage = new ColonyConcierge.Mobile.Customer.MainPage(true);
					Application.Current.MainPage = mainPage;
				}
			}

			//Background color
			//MainPage.SetValue(NavigationPage.BarBackgroundColorProperty, Color.Black);
			//Title color
			MainPage.SetValue(NavigationPage.BarTextColorProperty, Color.White);
			MainPage.SetValue(NavigationPage.BackButtonTitleProperty, AppResources.Back);

    //        else
    //        {
				//// Time for some introduction
				//MainPage = new ColonyConcierge.Mobile.Customer.MainPage();   
    //            Shared.Firstlaunch = false;
    //        }

        }

        protected override void OnStart()
        {
			// Handle when your app starts
			if (Started != null)
			{
				Started(this, EventArgs.Empty);
			}
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }

	public class CustomLogger : FFImageLoading.Helpers.IMiniLogger
	{
		public void Debug(string message)
		{
			System.Diagnostics.Debug.WriteLine(message);
		}

		public void Error(string errorMessage)
		{
			System.Diagnostics.Debug.WriteLine(errorMessage);
		}

		public void Error(string errorMessage, Exception ex)
		{
			Error(errorMessage + System.Environment.NewLine + ex.ToString());
		}
	}
}
