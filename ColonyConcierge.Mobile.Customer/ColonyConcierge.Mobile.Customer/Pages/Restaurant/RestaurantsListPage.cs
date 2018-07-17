using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ColonyConcierge.APIData.Data;
using ColonyConcierge.Client;
using ColonyConcierge.Mobile.Customer.Localization.Resx;
using FFImageLoading.Cache;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ColonyConcierge.Mobile.Customer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RestaurantsListPage : ContentPageBase
	{
		int mDestinationId = -1;

		public RestaurantsListPage(int destinationId)
		{
            InitializeComponent();

			mDestinationId = destinationId;
			LoadDestination();
		}

		public void LoadDestination()
		{
			GroupedDeliveryDestination deliveryDestination = null;
			Task.Run(() =>
			{
				deliveryDestination = Shared.APIs.IServices.GetDestinationByID(mDestinationId);
			}).ContinueWith((arg) =>
			{
				if (deliveryDestination != null)
				{
					Shared.LocalAddress = deliveryDestination.Address;
				}
			}, TaskScheduler.FromCurrentSynchronizationContext());
		}


		public RestaurantsListPage(Service groupedService, List<RestaurantVM> restaurantsCategory = null)
		{
			InitializeComponent();

			NavigationPage.SetBackButtonTitle(this, AppResources.Back);

			ListViewRestaurant.ItemTapped += (sender, e) =>
			{
                RestaurantVM restaurantVM = e.Item as RestaurantVM;
                RestaurantDetailPage restaurantDetailPage = new RestaurantDetailPage(restaurantVM, groupedService);
                restaurantDetailPage.Title = restaurantVM.SearchResultData.RestaurantDisplayName;
                Utils.PushAsync(Navigation, restaurantDetailPage, true);
			};

			LoadData(restaurantsCategory);
		}

		public void LoadData(List<RestaurantVM> restaurantsCategory)
		{
			ListViewRestaurant.ItemsSource = restaurantsCategory;
		}

		protected override bool OnBackButtonPressed()
		{
			return base.OnBackButtonPressed();
		}
	}
}
