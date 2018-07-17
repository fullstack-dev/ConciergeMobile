using ColonyConcierge.APIData.Data;
using ColonyConcierge.Client;
using ColonyConcierge.Mobile.Customer.Localization.Resx;
using PCLAppConfig;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TK.CustomMap.Api;
using TK.CustomMap.Api.Google;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Plugin.Toasts;
using Xamarin.Forms.Xaml;

namespace ColonyConcierge.Mobile.Customer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GroupedDeliveryAddressListPage : ContentPageBase
	{
		private IAppServices mAppServices;
		private List<ColonyConcierge.APIData.Data.GroupedDeliveryDestination> GroupedDeliveryDestinations = new List<APIData.Data.GroupedDeliveryDestination>();
		private Service mGroupedService;
		private bool mFirstLoad = true;
		private Page ParentPage;
		private Action<GroupedDeliveryDestination> Saved;

		public GroupedDeliveryAddressListPage(Page parentPage, Action<GroupedDeliveryDestination> saved = null)
		{
			InitializeComponent();

			ParentPage = parentPage;
			Saved = saved;

			mAppServices = DependencyService.Get<IAppServices>();

			double lastWidth = 0;
			double lastHeight = 0;
			this.SizeChanged += (sender, e) =>
			{
				if (this.Width > 1 && this.Height > 1)
				{
					if (lastWidth < this.Width || lastHeight < this.Height)
					{
						lastWidth = this.Width;
						lastHeight = this.Height;

						ImageBackgroundFront.WidthRequest = this.Width;
						ImageBackgroundFront.HeightRequest = lastHeight;

						ImageBackgroundBehind.WidthRequest = this.Width;
						ImageBackgroundBehind.HeightRequest = lastHeight;
					}
				}
			};

			ImageBackgroundBehind.SizeChanged += (sender, e) =>
			{
				if (this.ImageBackgroundBehind.Width > 1 && this.ImageBackgroundBehind.Height > 1)
				{
					ImageBackgroundFront.WidthRequest = this.ImageBackgroundBehind.Width;
					ImageBackgroundFront.HeightRequest = this.ImageBackgroundBehind.Height;
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
							return new GroupedDeliveryDestinationItemView(t)
							{
								IsSelected = t.Address != null && t.Address.BasicAddress != null &&
									t.Address.BasicAddress.ToAddressLine() == Shared.LocalAddress.BasicAddress.ToAddressLine()
							};
						}).ToList();
						StackLayoutGroupedDeliveryAddress.Children.Clear();
						foreach (var groupedDeliveryDestinationItemView in groupedDeliveryDestinationItemViews)
						{
							var groupedDeliveryAddressItemView = new GroupedDeliveryAddressItemView();
							groupedDeliveryAddressItemView.BindingContext = groupedDeliveryDestinationItemView;
							groupedDeliveryAddressItemView.Clicked += (sender, e) =>
							{
								Shared.LocalAddress = groupedDeliveryDestinationItemView.Model.Address;
								Saved(groupedDeliveryDestinationItemView.Model);
								var pages = Navigation.NavigationStack.Reverse().Skip(1).ToList();
								if (ParentPage != null)
								{
									foreach (var page in pages)
									{
										if (page != ParentPage)
										{
											Navigation.RemovePage(page);
										}
										else
										{
											break;
										}
									}
								}
								if (pages.Count > 0)
								{
									Navigation.PopAsync(true).ConfigureAwait(false);
								}
							};
							StackLayoutGroupedDeliveryAddress.Children.Add(groupedDeliveryAddressItemView);
						}

					}
					scrollView.IsVisible = true;
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
