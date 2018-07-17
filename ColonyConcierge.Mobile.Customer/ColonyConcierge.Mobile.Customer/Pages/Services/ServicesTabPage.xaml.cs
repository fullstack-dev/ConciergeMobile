using ColonyConcierge.APIData.Data;
using ColonyConcierge.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using ColonyConcierge.Mobile.Customer.Localization.Resx;
using Plugin.Toasts;
using System.Text.RegularExpressions;
using System.Diagnostics;
using Xamarin.Forms.Xaml;

namespace ColonyConcierge.Mobile.Customer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ServicesTabPage : TabbedPage, IServicesTabPage, IReloadPage
    {
		AddressFacade mAddressFacade = new AddressFacade();
		IAppServices mAppServices;
        private List<ScheduledService> availableServices = new List<ScheduledService>();
		private List<Service> mAvailableServicesForUser = new List<Service>();
		private bool mIsNeedLocation = false;
		ExtendedAddress mServiceAddress;

        ServicesPage ServicesTab;
        ScheduledPage ScheduledTab;


        public List<Service> AvailableServicesForUser
		{
			get
			{
				return mAvailableServicesForUser;
			}
			set
			{
				mAvailableServicesForUser = value;
			}
		}
        private string mZipCode;
        private bool mFirstLoad = true;

        private bool mIsLoadingToday = false;
        public bool IsLoadingToday
        {
            set
            {
                this.OnPropertyChanging(nameof(IsLoadingToday));
                mIsLoadingToday = value;
                this.OnPropertyChanged(nameof(IsLoadingToday));
            }
            get
            {
                return mIsLoadingToday;
            }
        }

        public bool IsLoadingScheduled
        {
            set
            {
                ScheduledTab.LoadingScheduled = value;
                ServicesTab.LoadingScheduled = value;
            }
        }

		public bool LoadInAppearing
		{
			get;
			set;
		} = true;

		public bool IsErrorPage
		{
			get;
			set;
		}

		public ServicesTabPage()
        {
            InitializeComponent();

            this.ServicesTab = new ServicesPage();
            this.ScheduledTab = new ScheduledPage();

            this.ServicesTab.Appearing += ServicesTab_Appearing;

            this.Children.Add(this.ServicesTab);
            this.Children.Add(this.ScheduledTab);

			//double width = 1;
			//double height = 1;
			//this.SizeChanged += (sender, e) =>
			//{
			//	if (this.Width > width || this.Height > height)
			//	{
			//		width = this.Width;
			//		height = this.Height;

			//		ServicesTab.HeightRequest = height;
			//		ServicesTab.Content.HeightRequest = height;
			//	}
			//};

			NavigationPage.SetBackButtonTitle(this, AppResources.Back);

			mAppServices = DependencyService.Get<IAppServices>();


			if (Device.RuntimePlatform == Device.iOS)
			{
				ServicesTab.Icon = "servicesIcon.png";
				ScheduledTab.Icon = "calendarIcon.png";
			}

			this.ToolbarItems.Add(new ToolbarItem()
			{
				Icon = "location1.png",
				Text = AppResources.Location,
				Command = new Command(() =>
				{
					NavigationAddressSuggestionListPage();
				})
			});

			//this.CurrentPageChanged += (sender, e) =>
			//{
			//	if (this.CurrentPage == ScheduledTab)
			//	{
			//		LoadScheduled(true);
			//	}
			//};

			bool ListViewServicesItemTapping = false;
			ServicesTab.ItemTapped += async (sender, e) =>
			{
				if (!ListViewServicesItemTapping)
				{
					ListViewServicesItemTapping = true;
					var serviceTypeItem = e.Item as ServiceTypeItemViewModel;
					serviceTypeItem.IsSelected = true;
                    ServicesTab.SelectedItem = null;
					if (serviceTypeItem.TypeName == ServiceTypes.Restaurant)
					{
						//if (serviceTypeItem.DisplayType == "School Lunch")
						if (serviceTypeItem.ServiceKind == ServiceKindCodes.Restaurant_GroupedDelivery)
						{
							var schoolLunchAddressPage = new SchoolLunchAddressPage(serviceTypeItem.Model);
							await Utils.PushAsync(Navigation, schoolLunchAddressPage, true);
						}
						else
						{
							var restaurantListingPage = new RestaurantsTabPage(serviceTypeItem.Model);
							await Utils.PushAsync(Navigation, restaurantListingPage, true);
						}
					}
					else if (serviceTypeItem.TypeName == ServiceTypes.Shopping)
					{
						var shoppingPage = new ShoppingPage();
						shoppingPage.Title = serviceTypeItem.DisplayType;
						await Utils.PushAsync(Navigation, shoppingPage, true);
					}
					else if (serviceTypeItem.TypeName == ServiceTypes.SpecialRequests)
					{
						var specialRequestPage = new SpecialRequestPage();
						specialRequestPage.Title = serviceTypeItem.DisplayType;
						await Utils.PushAsync(Navigation, specialRequestPage, true);
					}
					else if (serviceTypeItem.TypeName == ServiceTypes.Errands)
					{
						var services = AvailableServicesForUser.Where((t) => t.ServiceType.Equals(serviceTypeItem.TypeName)).ToList();
						var errandPage = new ErrandPage(services);
						errandPage.Title = serviceTypeItem.DisplayType;
						await Utils.PushAsync(Navigation, errandPage, true);
					}
					else
					{
						var services = AvailableServicesForUser.Where((t) => t.ServiceType.Equals(serviceTypeItem.TypeName)).ToList();
						ServiceKindList serviceKindList = new ServiceKindList(services);
						serviceKindList.Title = serviceTypeItem.DisplayType;
						await Utils.PushAsync(Navigation, serviceKindList, true);
					}
					serviceTypeItem.IsSelected = false;
					ListViewServicesItemTapping = false;
				}
			};

			bool ListViewScheduledItemTapping = false;
			ScheduledTab.ItemTapped += async (sender, e) =>
			{
				if (!ListViewScheduledItemTapping)
				{
					ListViewScheduledItemTapping = true;
					var scheduledService = (e.Item as ScheduledServiceItemViewModel).Model;
                    ScheduledTab.SelectedItem = null;
					ServiceRequestDetailsPage serviceKindList = new ServiceRequestDetailsPage(scheduledService, scheduledService.Name);
					await Utils.PushAsync(Navigation, serviceKindList, true);
					ListViewScheduledItemTapping = false;
				}
			};

            ScheduledTab.RefreshCommand = new Command(() =>
			{
                ScheduledTab.IsRefreshing = false;
				LoadScheduled(true);
			});

			//throw new Exception("Test Firebase Crash report");
        }

        private void ServicesTab_Appearing(object sender, EventArgs e)
        {
            if (mFirstLoad)
            {
                mFirstLoad = false;
                if (LoadInAppearing)
                {
                    LoadData();
                }
            }
            else
            {
                ExtendedAddress serviceAddress = Shared.LocalAddress;
                if (serviceAddress != null &&
                    ((mZipCode != serviceAddress.BasicAddress.ZipCode) ||
                     ((mServiceAddress == null) || (mServiceAddress.BasicAddress.ToAddressLine() != serviceAddress.BasicAddress.ToAddressLine()))))
                {
                    if (LoadInAppearing)
                    {
                        LoadData();
                    }
                }
            }
        }

        public void SelectScheduleTab(bool isRefresh = false)
		{
			this.CurrentPage = ScheduledTab;
			if (isRefresh)
			{
				LoadScheduled(true);
			}
		}

		public void SelectServicesTab(bool isRefresh = false)
		{
			this.CurrentPage = ServicesTab;
			if (isRefresh)
			{
				LoadData();
			}
		}

		private void NavigationAddressSuggestionListPage()
		{
			AddressSuggestionListPage AddressSuggestionListPage = new AddressSuggestionListPage(this, (obj) =>
			{
				//Shared.LocalAddress = obj;
				LoadData();
			})
			{
				IsShowQRCode = true
			};
			Utils.PushAsync(Navigation, AddressSuggestionListPage, true);
		}

        public async void LoadData()
        {
            this.IsLoadingToday = true;
			this.IsLoadingScheduled = true;
			this.mIsNeedLocation = false;

            var serviceTypeItems = new List<ServiceTypeItemViewModel>();
			AvailableServicesForUser = new List<Service>();

			Utils.IReloadPageCurrent = this;
			try
			{
                mServiceAddress = mAddressFacade.GetUserAddress();

				if (mServiceAddress == null || (mServiceAddress.Latitude.Equals(0) && mServiceAddress.Longitude.Equals(0)))
				{
					if (mAppServices.LastLatitude.Equals(0) && mAppServices.LastLongitude.Equals(0))
					{
						mIsNeedLocation = true;
					}
					else if (mAppServices != null)
					{
						var latitude = mAppServices.LastLatitude;
						var longitude = mAppServices.LastLongitude;

						if (!latitude.Equals(0) || !longitude.Equals(0))
						{
							var currentAddressDetails = mAddressFacade.GetGmsDetails(latitude, longitude).Result;
							if (mServiceAddress == null)
							{
                                mServiceAddress = new ExtendedAddress() { BasicAddress = new Address() };
							}
							mAddressFacade.FillExtendedAddress(mServiceAddress, currentAddressDetails);
						}
					}
				}

                if (!this.IsErrorPage)
				{
					if (mIsNeedLocation)
					{
                        if (ContentPageBase.CheckCurrentPage(this))
						{
							Device.BeginInvokeOnMainThread(() =>
							{
                                NavigationAddressSuggestionListPage();
							});
						}
					}
					else
					{
                        var zipCode = mServiceAddress.BasicAddress.ZipCode;
                        SerivesFacade serivesService = new SerivesFacade();
                        mZipCode = await serivesService.CheckAvailableServicesAsync(mServiceAddress);
                        if (string.IsNullOrEmpty(mZipCode))
						{
                            if (!this.IsErrorPage && ContentPageBase.CheckCurrentPage(this))
							{
                                Device.BeginInvokeOnMainThread(() =>
								{
									NavigationAddressSuggestionListPage();
								});
							}
						}
						else
						{
                            mZipCode = mServiceAddress.BasicAddress.ZipCode;
							if (Shared.IsLoggedIn)
							{
                                AvailableServicesForUser = await Shared.APIs.IServices.GetAvailableServicesForUser_Async(Shared.UserId, null, null, mZipCode);
							}
							else
							{
                                AvailableServicesForUser = await Shared.APIs.IServices.GetAvailableServices_Async(mZipCode);
							}

							if (AvailableServicesForUser != null)
							{
                                serviceTypeItems = AvailableServicesForUser.GroupBy(t => t.ServiceType).Select(t =>
								{
									decimal? price = null;
									string priceDescription = string.Empty;
									var service = t.FirstOrDefault();
									if (t.Key == ServiceTypes.Restaurant)
									{
                                        var restaurantDelivery = t.FirstOrDefault(s => s.ServiceKind == ServiceKindCodes.Restaurant_Delivery);
										if (restaurantDelivery != null)
										{
											service = restaurantDelivery;
										}
									}
									var fee = service.Fees.FirstOrDefault();

									if (fee != null)
									{
										price = Math.Round(fee.Amount, 2);
										priceDescription = fee.Description;
									}

									return new ServiceTypeItemViewModel(service)
									{
										TypeName = t.FirstOrDefault().ServiceType,
										ServiceKind = t.FirstOrDefault().ServiceKind,
										DisplayType = Regex.Replace(t.FirstOrDefault().ServiceType.ToString(), "(\\B[A-Z])", " $1"),
										PriceDescription = priceDescription,
										Price = price,
										TypeDescription = AppResources.ServiceTypeDescription,
									};
								}).ToList();

                                //var schoolLunch = AvailableServicesForUser.FirstOrDefault(t => t.DisplayName == "School Lunch");
                                var groupedDelivery = AvailableServicesForUser.Where(t => t.ServiceType == ServiceTypes.Restaurant &&
														t.ServiceKind == ServiceKindCodes.Restaurant_GroupedDelivery);
								if (groupedDelivery != null)
								{
                                    foreach (var servicedDelivery in groupedDelivery)
									{
										decimal? price = null;
										string priceDescription = string.Empty;

										var fee = servicedDelivery.Fees.FirstOrDefault();
										if (fee != null)
										{
											price = Math.Round(fee.Amount, 2);
											priceDescription = fee.Description;
										}

										serviceTypeItems.Add(new ServiceTypeItemViewModel(servicedDelivery)
										{
											TypeName = servicedDelivery.ServiceType,
											ServiceKind = servicedDelivery.ServiceKind,
											DisplayType = servicedDelivery.DisplayName,
											PriceDescription = priceDescription,
											Price = price,
											TypeDescription = AppResources.ServiceTypeDescription,
										});
									}
								}

                                serviceTypeItems = serviceTypeItems.OrderBy(t => t.TypeName.ToString()).ToList();
							}

							if (Shared.IsLoggedIn)
							{
                                availableServices = await Shared.APIs.IUsers.GetScheduledServices_Async(Shared.UserId, service_states: "!Complete,Canceled,Rejected", start_date: (ColonyConcierge.APIData.Data.SimpleDate)DateTime.Now.AddMonths(-2));
								availableServices.Reverse();
							}
						}
					}
				}

            }
			catch (Exception ex)
			{
                if (!this.IsErrorPage && Utils.IReloadPageCurrent == this)
				{
					Device.BeginInvokeOnMainThread(async () =>
					{
						await Utils.ShowErrorMessage(ex);
					});
				}
			}
			if (Utils.IReloadPageCurrent == this)
			{
				Utils.IReloadPageCurrent = null;
			}

            if (!this.IsErrorPage)
            {
                ServicesTab.ItemsSource = serviceTypeItems;
                this.IsLoadingToday = false;
                if (availableServices == null)
                {
                    availableServices = new List<ScheduledService>();
                }
                ScheduledTab.ItemsSource = availableServices.Select(s => new ScheduledServiceItemViewModel(s));
                
                this.IsLoadingScheduled = false;
                if (Application.Current.MainPage is HomePage)
                {
                    ((Application.Current.MainPage as HomePage).Master as HomeMasterPage).LoadUser();
                }
            }
        }

		public void LoadScheduled(bool isProgress = false)
		{
			this.IsLoadingScheduled = isProgress;
			availableServices = new List<ScheduledService>();

			Task.Run(() =>
			{
				Utils.IReloadPageCurrent = this;
				try
				{
					if (Shared.IsLoggedIn)
					{
						availableServices = Shared.APIs.IUsers.GetScheduledServices(Shared.UserId, service_states: "!Complete,Canceled,Rejected", start_date: (ColonyConcierge.APIData.Data.SimpleDate)DateTime.Now.AddMonths(-2));
						if (availableServices != null)
						{
							availableServices.Reverse();
						}
					}
				}
				catch (Exception ex)
				{
					if (this.IsErrorPage)
					{
						Device.BeginInvokeOnMainThread(async () =>
						{
							await Utils.ShowErrorMessage(ex);
						});
					}
				}
				if (Utils.IReloadPageCurrent == this)
				{
					Utils.IReloadPageCurrent = null;
				}
				Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
				{
					if (availableServices == null)
					{
						availableServices = new List<ScheduledService>();
					}
					ScheduledTab.ItemsSource = availableServices.Select(s => new ScheduledServiceItemViewModel(s));
					this.IsLoadingScheduled = false;
				});
			});
		}

		public void ReloadPage()
		{
			LoadData();
		}

		public void ShowLoadErrorPage()
		{
			if (!IsErrorPage)
			{
				IsErrorPage = true;
				Device.BeginInvokeOnMainThread(() =>
				{
					;
					var scheduledTabContent = this.ScheduledTab.Content;
					var servicesTabContent = this.ServicesTab.Content;

					var errorView = new ErrorView();
					this.ScheduledTab.Content = errorView;
					errorView.TryAgain += (sender, e) =>
					{
						this.ScheduledTab.Content = scheduledTabContent;
						this.ServicesTab.Content = servicesTabContent;
                        this.IsErrorPage = false;
						ReloadPage();
					};

				    errorView = new ErrorView();
    				this.ServicesTab.Content = errorView;
					errorView.TryAgain += (sender, e) =>
					{
						this.ScheduledTab.Content = scheduledTabContent;
						this.ServicesTab.Content = servicesTabContent;
						this.IsErrorPage = false;
						ReloadPage();
					};
				});
			}
		}
	}
}
