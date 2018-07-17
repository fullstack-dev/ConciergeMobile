
﻿using ColonyConcierge.APIData.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using ColonyConcierge.Client;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Plugin.Toasts;
using ColonyConcierge.Mobile.Customer.Localization.Resx;
using Xamarin.Forms.Xaml;

namespace ColonyConcierge.Mobile.Customer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AccountAddressPage : ContentPageBase
	{
		private AddressFacade mAddressFacade = new AddressFacade();
		private bool mFirstLoad = true;

		public AccountAddressPage()
		{
			InitializeComponent();
		}

		public override void ReloadPage()
		{
			base.ReloadPage();
            PopulateItemsInList();
		}

		private void PopulateItemsInList(bool isProgress = true)
		{
			this.IsBusy = isProgress;
			Utils.IReloadPageCurrent = this;
			try
			{
				var userAddress = mAddressFacade.GetUserAddress(false);
				if (userAddress != null)
				{
					this.AddressList = new List<ExtendedAddress> { userAddress };
				}
				//this.AddressList = await Task.Run(() => { return Shared.APIs.IUsers.GetServiceAddresses(Shared.UserId); });
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
			if (Utils.IReloadPageCurrent == this)
			{
				Utils.IReloadPageCurrent = null;
			}
			this.IsBusy = false;
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

			if (mFirstLoad)
			{
				mFirstLoad = false;
				Task.Run(() =>
				{
					PopulateItemsInList();
				});
			}
			else if (!this.IsBusy)
			{
				Task.Run(() =>
				{
					PopulateItemsInList(false);
				});
			}
		}

		private List<ExtendedAddress> addressList;

		public List<ExtendedAddress> AddressList
		{
			get { return addressList; }
			set
			{
				addressList = value;
				this.OnPropertyChanged(nameof(AddressList));
				// Add the AddressList as Address in List
			}
		}

		private async void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			RegistrationEntry mUserInfo = new RegistrationEntry();
			if (e.SelectedItem == null) return;
			var Address = (ExtendedAddress)e.SelectedItem;
			mUserInfo.ServiceAddress = Address;
			(sender as ListView).SelectedItem = null;
			await Utils.PushAsync(Navigation, new AccountAddressTypePage(true)
			{
				BindingContext = mUserInfo,
				Title = AppResources.ServiceAddress
			});
		}
	}
}
