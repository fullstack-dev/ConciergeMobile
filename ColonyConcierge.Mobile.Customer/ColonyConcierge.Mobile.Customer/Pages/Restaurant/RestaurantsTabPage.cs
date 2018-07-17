using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ColonyConcierge.APIData.Data;
using ColonyConcierge.Client;
using ColonyConcierge.Mobile.Customer.Localization.Resx;
using FFImageLoading.Forms;
using Plugin.Toasts;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ColonyConcierge.Mobile.Customer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RestaurantsTabPage : ContentPageBase
    {
        int mDestinationId = -1;
        private List<RestaurantVM> restaurants = new List<RestaurantVM>();
        //private List<RestaurantCategoryDislayItemViewModel> restaurantCategoryItems = new List<RestaurantCategoryDislayItemViewModel>();
        private IAppServices mAppServices;
        AddressFacade mAddressFacade = new AddressFacade();

        GroupedDeliveryDestination mGroupedDeliveryDestination = null;
        Service mGroupedService;
        ExtendedAddress mServiceAddress;

        private bool mIsNeedLocation = false, mFirstLoad = true;
        private string mZipCode = string.Empty;

        private bool mIsLoadingCategories = false;

        public bool IsLoadingCategories
        {
            set
            {
                this.OnPropertyChanging(nameof(IsLoadingCategories));
                mIsLoadingCategories = value;
                this.OnPropertyChanged(nameof(IsLoadingCategories));
            }
            get
            {
                return mIsLoadingCategories;
            }
        }

        public RestaurantsTabPage()
        {
            InitializeComponent();

            NavigationPage.SetBackButtonTitle(this, AppResources.Back);

            mAppServices = DependencyService.Get<IAppServices>();

            this.ToolbarItems.Add(new ToolbarItem()
            {
                Icon = "location1.png",
                Text = AppResources.Location,
                Command = new Command(() =>
                {
                    NavigationAddressSuggestionListPage();
                })
            });

            bool ListViewRestaurantItemTapping = false;
            ListViewRestaurant.ItemTapped += async (sender, e) =>
            {
                if (!ListViewRestaurantItemTapping)
                {
                    if (e.Item is RestaurantVM)
                    {
                        ListViewRestaurantItemTapping = true;
                        RestaurantVM restaurantVM = e.Item as RestaurantVM;
                        restaurantVM.IsSelected = true;
                        ListViewRestaurant.SelectedItem = null;
                        RestaurantDetailPage restaurantDetailPage = new RestaurantDetailPage(restaurantVM, mGroupedService, mGroupedDeliveryDestination);
                        restaurantDetailPage.Title = restaurantVM.SearchResultData.RestaurantDisplayName;
                        await Utils.PushAsync(Navigation, restaurantDetailPage, true);
                        restaurantVM.IsSelected = false;
                        ListViewRestaurantItemTapping = false;
                    }
                }
            };

            ImageClose.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command((t) =>
                {
                    restaurantEntry.Text = string.Empty;
                })

            });
        }

        public RestaurantsTabPage(int destinationId) : this()
        {
            mDestinationId = destinationId;
        }

        public RestaurantsTabPage(Service groupedService, GroupedDeliveryDestination groupedDeliveryDestination = null) : this()
        {
            mGroupedDeliveryDestination = groupedDeliveryDestination;
            mGroupedService = groupedService;
        }

        public void LoadDestination()
        {
            this.IsBusy = true;
            Task.Run(() =>
            {
                Utils.IReloadPageCurrent = this;
                try
                {
                    mGroupedDeliveryDestination = Shared.APIs.IServices.GetDestinationByID(mDestinationId);
                    if (mGroupedDeliveryDestination != null
                        && mGroupedDeliveryDestination.Address != null)
                    {
                        List<ColonyConcierge.APIData.Data.Service> availableServicesForUser = new List<Service>();
                        if (Shared.IsLoggedIn)
                        {
                            availableServicesForUser = Shared.APIs.IServices.GetAvailableServicesForUser(Shared.UserId, null, null, mGroupedDeliveryDestination.Address.BasicAddress.ZipCode);
                        }
                        else
                        {
                            availableServicesForUser = Shared.APIs.IServices.GetAvailableServices(mGroupedDeliveryDestination.Address.BasicAddress.ZipCode);
                        }
                        if (availableServicesForUser != null)
                        {
                            mGroupedService = availableServicesForUser.FirstOrDefault((arg) => arg.ServiceType == ServiceTypes.Restaurant
                                                                                      && arg.ServiceKind == ServiceKindCodes.Restaurant_GroupedDelivery);
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (!this.IsErrorPage &&
                        (Utils.IReloadPageCurrent == this || CheckCurrentPage()))
                    {
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            await Utils.ShowErrorMessage(string.IsNullOrEmpty(ex.Message) ? AppResources.SomethingWentWrong : ex.Message);
                        });
                    }
                }
                if (Utils.IReloadPageCurrent == this)
                {
                    Utils.IReloadPageCurrent = null;
                }
            }).ContinueWith((arg) =>
            {
                if (mGroupedDeliveryDestination != null && mGroupedService != null)
                {
                    Shared.LocalAddress = mGroupedDeliveryDestination.Address;
                    LoadData();
                }
                else
                {
                    this.IsBusy = false;
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void NavigationAddressSuggestionListPage()
        {
            if (mGroupedDeliveryDestination == null)
            {
                AddressSuggestionListPage AddressSuggestionListPage = new AddressSuggestionListPage(this, (obj) =>
                {
                    //Shared.LocalAddress = obj;
                    LoadData();
                });
                Utils.PushAsync(Navigation, AddressSuggestionListPage, true);
            }
            else
            {
                GroupedDeliveryAddressListPage addressSuggestionListPage = new GroupedDeliveryAddressListPage(this, (obj) =>
                {
                    mGroupedDeliveryDestination = obj;
                });
                Utils.PushAsync(Navigation, addressSuggestionListPage, true);
            }
        }

        public override void ReloadPage()
        {
            base.ReloadPage();
            if (mDestinationId > 0)
            {
                LoadDestination();
            }
            else
            {
                LoadData();
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            //if (mIsNeedLocation)
            //{
            //    LoadData();
            //}

            if (mFirstLoad)
            {
                mFirstLoad = false;
                if (mDestinationId > 0)
                {
                    LoadDestination();
                }
                else
                {
                    LoadData();
                }
            }
            else
            {
                ExtendedAddress serviceAddress = Shared.LocalAddress;
                if (serviceAddress != null &&
                    (mZipCode != serviceAddress.BasicAddress.ZipCode) ||
                    (mServiceAddress != null && mServiceAddress.BasicAddress.ToAddressLine() != serviceAddress.BasicAddress.ToAddressLine()))
                {
                    if (mDestinationId > 0)
                    {
                        LoadDestination();
                    }
                    else
                    {
                        LoadData();
                    }
                }
            }
        }

        public void LoadData()
        {
            this.mIsNeedLocation = false;
            this.IsBusy = true;
            var apis = Shared.APIs;
            restaurants = new List<RestaurantVM>();
            //restaurantCategoryItems = new List<RestaurantCategoryDislayItemViewModel>();
            Task.Run(() =>
            {
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

                    if (mIsNeedLocation)
                    {
                        if (!this.IsErrorPage &&
                            (Utils.IReloadPageCurrent == this || CheckCurrentPage()))
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
                        mZipCode = serivesService.CheckAvailableServices(mServiceAddress);
                        if (!this.IsErrorPage && Utils.IReloadPageCurrent == this)
                        {
                            if (string.IsNullOrEmpty(mZipCode))
                            {
                                if (CheckCurrentPage())
                                {
                                    Device.BeginInvokeOnMainThread(() =>
                                    {
                                        NavigationAddressSuggestionListPage();
                                        //string message = string.Format(AppResources.ZipInvalid, zipCode);
                                        //var notificator = DependencyService.Get<IToastNotificator>();
                                        //await notificator.Notify(ToastNotificationType.Error, AppResources.Error, message, TimeSpan.FromSeconds(5));
                                    });
                                }
                            }
                            else
                            {
                                RestaurantFacade restaurantFacade = new RestaurantFacade();
                                restaurants = restaurantFacade.GetRestaurantByAddress(mServiceAddress.Latitude, mServiceAddress.Longitude, true, mZipCode, -1, mGroupedDeliveryDestination);
                                if (restaurants == null)
                                {
                                    restaurants = new List<RestaurantVM>();
                                }
                                restaurants = restaurants.Where(s =>
                                {
                                    return mGroupedDeliveryDestination == null || s.SearchResultData.IsGroupedDeliveryAvailable;
                                }).ToList();
                                restaurants.Sort((s1, s2) =>
                                {
                                    var isNexts1 = s1.IsNextDeliveryTime || s1.IsNextPickupTime;
                                    var isNexts2 = s2.IsNextDeliveryTime || s2.IsNextPickupTime;

                                    if (isNexts1 != isNexts2)
                                    {
                                        return isNexts1.CompareTo(isNexts2);
                                    }
                                    else if (!isNexts1 && !isNexts2)
                                    {
                                        if (s1.SearchResultData.IsDeliveryAvailable != s2.SearchResultData.IsDeliveryAvailable)
                                        {
                                            return s2.SearchResultData.IsDeliveryAvailable.CompareTo(s1.SearchResultData.IsDeliveryAvailable);
                                        }
                                        return s1.SearchResultData.DistanceInMeters.CompareTo(s2.SearchResultData.DistanceInMeters);
                                    }
                                    else
                                    {
                                        if (s1.SearchResultData.IsDeliveryAvailable != s2.SearchResultData.IsDeliveryAvailable)
                                        {
                                            return s2.SearchResultData.IsDeliveryAvailable.CompareTo(s1.SearchResultData.IsDeliveryAvailable);
                                        }

                                        //if (s1.SearchResultData.IsDeliveryAvailable == s2.SearchResultData.IsDeliveryAvailable)
                                        //{
                                        //if (s1.SearchResultData.IsDeliveryAvailable)
                                        //{
                                        //if (s1.IsNextDeliveryTime != s2.IsNextDeliveryTime)
                                        //{
                                        //	return s2.IsNextDeliveryTime.CompareTo(s1.IsNextDeliveryTime);
                                        //}
                                        //}
                                        //else
                                        //{
                                        //	if (s1.IsNextPickupTime != s2.IsNextPickupTime)
                                        //	{
                                        //		return s2.IsNextPickupTime.CompareTo(s1.IsNextPickupTime);
                                        //	}
                                        //}
                                        return s1.SearchResultData.DistanceInMeters.CompareTo(s2.SearchResultData.DistanceInMeters);
                                        //}
                                    }

                                    //return s2.SearchResultData.IsDeliveryAvailable.CompareTo(s1.SearchResultData.IsDeliveryAvailable);
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (!this.IsErrorPage &&
                        (Utils.IReloadPageCurrent == this || CheckCurrentPage()))
                    {
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            await Utils.ShowErrorMessage(string.IsNullOrEmpty(ex.Message) ? AppResources.SomethingWentWrong : ex.Message);
                        });
                    }
                }
                if (Utils.IReloadPageCurrent == this)
                {
                    Utils.IReloadPageCurrent = null;
                }

            }).ContinueWith((t) =>
            {
                this.IsBusy = false;
                //ListViewRestaurantCategories.ItemsSource = restaurantCategoryItems;
                ListViewRestaurant.ItemsSource = restaurants;
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        //private void SearchBar_TextChanged(object sender, TextChangedEventArgs e){
        //    if (string.IsNullOrEmpty(e.NewTextValue))
        //    {
        //        ListViewRestaurant.ItemsSource = restaurants;
        //    }

        //    else
        //    {
        //        ListViewRestaurant.ItemsSource = restaurants.Where(x => x.RestaurantTitle.ToLower().Contains(e.NewTextValue.ToLower()) || x.CategoriesText.ToLower().Contains(e.NewTextValue.ToLower()));
        //    }
        //}

        private void restaurantEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            //if (this._textChangeItemSelected)
            //{
            //    this._textChangeItemSelected = false;
            //    return;
            //}

            //var searchingText = this.SearchText;
            var searchingText = restaurantEntry.Text;

            if (string.IsNullOrEmpty(searchingText))
            {
                ListViewRestaurant.ItemsSource = restaurants;
            }
            else
            {
                ListViewRestaurant.ItemsSource = restaurants.Where(x => x.RestaurantTitle.ToLower().Contains(e.NewTextValue.ToLower()) || x.CategoriesText.ToLower().Contains(e.NewTextValue.ToLower()));
            }
            //this.IsBusy = false;
            //if (searchingText != this.SearchText)
            //{
            //    restaurantEntry_TextChanged(restaurantEntry, null);
            //}

        }
    }
}
