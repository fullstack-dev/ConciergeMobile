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
using Xamarin.Forms.Xaml;

namespace ColonyConcierge.Mobile.Customer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SchoolLunchAddressPage : ContentPageBase
    {
		List<ColonyConcierge.APIData.Data.GroupedDeliveryDestination> GroupedDeliveryDestinations = new List<APIData.Data.GroupedDeliveryDestination>();
		Service mGroupedService;
        private bool mFirstLoad = true;

        public SchoolLunchAddressPage(Service groupedService)
        {
            InitializeComponent();

			NavigationPage.SetBackButtonTitle(this, AppResources.Back);
			mGroupedService = groupedService;

			bool ListViewServicesItemTapping = false;
			ListViewSchools.ItemTapped += async (sender, e) =>
			{
				if (!ListViewServicesItemTapping)
				{
					ListViewServicesItemTapping = true;
					var groupedDeliveryDestinationItemView = e.Item as GroupedDeliveryDestinationItemView;
					groupedDeliveryDestinationItemView.IsSelected = true;
					Shared.LocalAddress = groupedDeliveryDestinationItemView.Model.Address;
					ListViewSchools.SelectedItem = null;
					var restaurantListingPage = new RestaurantsTabPage(mGroupedService, groupedDeliveryDestinationItemView.Model);
					await Utils.PushAsync(Navigation, restaurantListingPage, true);
					groupedDeliveryDestinationItemView.IsSelected = false;
					ListViewServicesItemTapping = false;
				}
			};
        }

		protected override void OnAppearing()
		{
			base.OnAppearing();

            if (mFirstLoad)
            {
                mFirstLoad = false;
                LoadData();
            }
        }

        public void LoadData()
        {
			this.IsBusy = true;
			Task.Run(() =>
			{
				Utils.IReloadPageCurrent = this;
				try
				{
					var localAddress = Shared.LocalAddress;
					GroupedDeliveryDestinations = Shared.APIs.IServices.FindDestinationsByZip(localAddress.BasicAddress.ZipCode);
				}
				catch (Exception ex)
				{
					Device.BeginInvokeOnMainThread(async () =>
					{
						await Utils.ShowErrorMessage(ex);
					});
				}
				if (Utils.IReloadPageCurrent == this)
				{
					Utils.IReloadPageCurrent = null;
				}
			}).ContinueWith((arg) =>
			{
				if (!this.IsErrorPage)
				{
					if (GroupedDeliveryDestinations != null)
					{
						var groupedDeliveryDestinationItemViews = GroupedDeliveryDestinations.Select(t =>
						{
							return new GroupedDeliveryDestinationItemView(t);
						}).ToList();
						ListViewSchools.ItemsSource = groupedDeliveryDestinationItemViews;
					}
				}
				this.IsBusy = false;
			}, TaskScheduler.FromCurrentSynchronizationContext());
        }

		public override void ReloadPage()
		{
			base.ReloadPage();
			LoadData();
		}
	}
}
