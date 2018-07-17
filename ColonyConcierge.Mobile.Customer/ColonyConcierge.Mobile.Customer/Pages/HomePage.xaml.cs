using ColonyConcierge.Mobile.Customer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using ColonyConcierge.APIData.Data;
using ColonyConcierge.Mobile.Customer.Localization.Resx;
using Xamarin.Forms.Xaml;

namespace ColonyConcierge.Mobile.Customer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : MasterDetailPage
    {
		private bool IsChanging = false;
		IAppServices mAppServices;

        public HomePage(Page page = null)
        {
            InitializeComponent();

			if (Device.RuntimePlatform == Device.iOS)
			{
				if (page == null)
				{
					var servicesNonTabPage = new ServicesNonTabPage();
					this.Detail = new NavigationPage(servicesNonTabPage)
					{
						BarTextColor = Color.White
					};
				}
				else
				{
					var navigationPage = new NavigationPage(page)
					{
						BarTextColor = Color.White
					};
					var servicesNonTabPage = new ServicesNonTabPage();
					servicesNonTabPage.LoadInAppearing = false;
					page.Navigation.InsertPageBefore(servicesNonTabPage, page);
					navigationPage.Popped += (sender, e) =>
					{
						if (e.Page == page)
						{
							servicesNonTabPage.LoadInAppearing = true;
							servicesNonTabPage.LoadData();
						}
					};
                    this.Detail = navigationPage;
				}
			}
			else
			{
				if (page == null)
				{
					var servicesTabPage = new ServicesTabPage();
					this.Detail = new NavigationPage(servicesTabPage)
					{
						BarTextColor = Color.White
					};
				}
				else
				{
					var navigationPage = new NavigationPage(page)
					{
						BarTextColor = Color.White
					};
					var servicesTabPage = new ServicesTabPage();
					servicesTabPage.LoadInAppearing = false;
					page.Navigation.InsertPageBefore(servicesTabPage, page);
					navigationPage.Popped += (sender, e) =>
					{
						if (e.Page == page)
						{
							servicesTabPage.LoadInAppearing = true;
							servicesTabPage.LoadData();
						}
					};
					this.Detail = navigationPage;
				}
			}
			mAppServices = DependencyService.Get<IAppServices>();
			mAppServices.TrackPage(this.GetType().Name);

			this.PropertyChanged += (sender, e) =>
			{
				if (e.PropertyName == nameof(this.Detail))
				{
					mAppServices.SetNetworkBar(false);
				}
			};

			if (Shared.IsLoggedIn 
			    	&& Shared.UserId == -1)
			{
				var userModel = Shared.APIs.IUsers.GetCurrentUser();
				Shared.UserId = userModel.ID;
			}

			masterPage.ListView.ItemTapped += ListView_ItemTapped;
			masterPage.TermsConditions.GestureRecognizers.Add(new TapGestureRecognizer()
			{
				Command = new Command((sender) =>
				{
					masterPage.ListView.SelectedItem = null;
					WebViewPage legalPage = new WebViewPage(PCLAppConfig.ConfigurationManager.AppSettings["TermsUrl"]);
					Detail = new NavigationPage(legalPage);
					IsPresented = false;
				})
			});
			masterPage.PrivacyPolicy.GestureRecognizers.Add(new TapGestureRecognizer()
			{
				Command = new Command((sender) =>
				{
					masterPage.ListView.SelectedItem = null;
					WebViewPage legalPage = new WebViewPage(PCLAppConfig.ConfigurationManager.AppSettings["PrivacyPolicyUrl"]);
					Detail = new NavigationPage(legalPage);
					IsPresented = false;
				})
			});

			this.IsPresentedChanged += (sender, e) =>
			{
				if (IsPresented)
				{
					var itemsSource = masterPage.ListView.ItemsSource;
					masterPage.ListView.ItemsSource = null;
					masterPage.ListView.ItemsSource = itemsSource;
					//Task.Run(() =>
					//{
					//	new System.Threading.ManualResetEvent(false).WaitOne(200); // to init menu
					//}).ContinueWith(t =>
					//{
					//	IsChanging = true;
					//	var ItemsSource = masterPage.ListView.ItemsSource;
					//	masterPage.ListView.ItemsSource = null;
					//	masterPage.ListView.ItemsSource = ItemsSource;
					//	IsChanging = false;
					//}, TaskScheduler.FromCurrentSynchronizationContext());
				}
			};
        }

		void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
		{
			var item = e.Item as MasterPageItemViewModel;
			if (item != null && !IsChanging)
			{
				masterPage.ListView.SelectedItem = null;
				foreach (var masterPageItemViewModel in masterPage.ListView.ItemsSource.Cast<MasterPageItemViewModel>().ToList())
				{
					masterPageItemViewModel.IsSelected = masterPageItemViewModel == item;
				}
				if (item.TargetType != null)
				{
					mAppServices = DependencyService.Get<IAppServices>();
					mAppServices.TrackPage(item.TargetType.Name);

					//IsPresented = false; 
					if (item.TargetType == typeof(AuthPage))
					{
						try
						{
							Shared.LoginToken = null;
							if (item.Parameter is ExtendedAddress)
							{
								if (Device.RuntimePlatform == Device.iOS)
								{
									var signinPage = new SigninPage()
									{
										ServiceAddress = item.Parameter as ExtendedAddress
									};
									var customNavigationPage = new CustomNavigationPage(signinPage);
									Application.Current.MainPage = customNavigationPage;
									signinPage.ShowBack(customNavigationPage);
									signinPage.Back += () =>
									{
										Application.Current.MainPage = new HomePage();
									};
									signinPage.Done += (obj) =>
									{
										Application.Current.MainPage = new HomePage();
									};
								}
								else 
								{
									var authPage = new AuthPage(item.Parameter as ExtendedAddress);
									authPage.ShowBack();
									authPage.Back += () =>
									{
										Application.Current.MainPage = new HomePage();
									};
									authPage.Done += (obj) =>
									{
										Application.Current.MainPage = new HomePage();
									};
									Application.Current.MainPage = authPage;
								}
							}
							else 
							{
								if (Device.RuntimePlatform == Device.iOS)
								{
									SigninPage signinPage = new SigninPage();
									var customNavigationPage = new CustomNavigationPage(signinPage);
									Application.Current.MainPage = customNavigationPage;
									signinPage.ShowBack(customNavigationPage);
									signinPage.Back += () =>
									{
										Application.Current.MainPage = new HomePage();
									};
								}
								else 
								{
									var authPage = new AuthPage();
									authPage.ShowBack();
									authPage.Back += () =>
									{
										Application.Current.MainPage = new HomePage();
									};
									Application.Current.MainPage = authPage;
								}
								Utils.ShowSuccessMessage(AppResources.LogoutMessage, AppResources.Logout);
							}
						}
						catch (Exception)
						{
							System.Diagnostics.Debug.WriteLine("Log Out Error !!!.");
						}
					}
					if (item.TargetType == typeof(WebViewPage))
					{
						var parameter = item.Parameter.ToString();
						var servciesUrl = PCLAppConfig.ConfigurationManager.AppSettings["ServciesUrl"];
						if (servciesUrl.Equals(item.Parameter))
						{
							AddressFacade addressFacade = new AddressFacade();
							if (addressFacade.GetUserAddress() != null)
							{
								parameter = PCLAppConfig.ConfigurationManager.AppSettings["ServciesUrl"] + addressFacade.GetUserAddress().BasicAddress.ZipCode;
							}
						}
						WebViewPage legalPage = new WebViewPage(parameter);
						Detail = new NavigationPage(legalPage);
					}
					else
					{
						if (item.Parameter == null)
						{
							var page = (Page)Activator.CreateInstance(item.TargetType);
							Detail = new NavigationPage(page);
						}
						else 
						{
							var page = (Page)Activator.CreateInstance(item.TargetType, item.Parameter);
							Detail = new NavigationPage(page);
						}
					}
					Task.Run(() =>
					{
						new System.Threading.ManualResetEvent(false).WaitOne(200);
					}).ContinueWith(t =>
					{
						IsPresented = false;
						var itemsSource = masterPage.ListView.ItemsSource;
						masterPage.ListView.ItemsSource = null;
						masterPage.ListView.ItemsSource = itemsSource;
					}, TaskScheduler.FromCurrentSynchronizationContext());
				}
			}
		}
    }
}
