using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ColonyConcierge.APIData.Data;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ColonyConcierge.Mobile.Customer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MyOrderPage : ContentPageBase
	{
		AddressFacade mAddressFacade = new AddressFacade();
		private List<ScheduledService> availableServices = new List<ScheduledService>();
       

        public MyOrderPage()
		{
			InitializeComponent();

            bool mListViewScheduledItemTapping = false;
            ListViewMyOrder.ItemTapped += async (sender, e) =>
			{
                try
                {
                    if (!mListViewScheduledItemTapping)
                    {
                        mListViewScheduledItemTapping = true;
                        var scheduledService = (e.Item as OrderItemViewModel).ScheduledService;
                        ListViewMyOrder.SelectedItem = null;
                        ServiceRequestDetailsPage serviceKindList = new ServiceRequestDetailsPage(scheduledService, scheduledService.Name);
                        await Utils.PushAsync(Navigation, serviceKindList, true);
                    }                     
                }
                catch (Exception ex)
                {
                    await Utils.ShowErrorMessage(ex);
                }

                mListViewScheduledItemTapping = false;
            };

			LoadData();
		}

		public void LoadData()
		{
			this.IsBusy = true;
			Task.Run(() =>
			{
				try
				{
					Utils.IReloadPageCurrent = this;
                    availableServices = Shared.APIs.IUsers.GetScheduledServices(Shared.UserId, service_states: "Complete,Canceled,Rejected", start_date: (ColonyConcierge.APIData.Data.SimpleDate)DateTime.Now.AddMonths(-2));
					if (Utils.IReloadPageCurrent == this)
					{
						Utils.IReloadPageCurrent = null;
					}
				}
				catch (Exception ex)
				{
					if (!this.IsErrorPage && Utils.IReloadPageCurrent == this)
					{
						Device.BeginInvokeOnMainThread(() =>
						{
							Utils.ShowErrorMessage(ex);
						});
					}
				}
			}).ContinueWith((arg) =>
			{
				if (!this.IsErrorPage)
				{
					if (availableServices != null)
					{
						availableServices.Reverse();
						var orderItemViewModels = new List<OrderItemViewModel>();
						foreach (var availableService in availableServices)
						{
							OrderItemViewModel orderItemViewModel = new OrderItemViewModel();
							orderItemViewModel.ScheduledService = availableService;
							orderItemViewModel.ServiceDate = new DateTime(availableService.ServiceDate.Year, availableService.ServiceDate.Month, availableService.ServiceDate.Day).ToString("MMMM d, yyyy");
							orderItemViewModel.ServiceType = availableService.Name;
							orderItemViewModel.SpecialDescription = availableService.SpecialInstructions;
							orderItemViewModel.Status = availableService.Status;
							orderItemViewModels.Add(orderItemViewModel);
						}
						ListViewMyOrder.ItemsSource = orderItemViewModels;
						ImageEmpty.IsVisible = orderItemViewModels.Count == 0;
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

		protected override bool OnBackButtonPressed()
		{
			Application.Current.MainPage = new HomePage();
			return true;
		}

		private void NavigationAddressSuggestionListPage()
		{
			AddressSuggestionListPage AddressSuggestionListPage = new AddressSuggestionListPage(this, (obj) =>
			{
				//Shared.LocalAddress = obj;
				LoadData();
			});
			Utils.PushAsync(Navigation, AddressSuggestionListPage, true);
		}

	}
}
