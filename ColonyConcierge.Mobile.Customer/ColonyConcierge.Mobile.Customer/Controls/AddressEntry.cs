using System;
using System.Collections.Generic;
using ColonyConcierge.APIData.Data;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace ColonyConcierge.Mobile.Customer
{
	public class AddressEntry : Entry
	{
		public bool IsNeedClearFocus
		{
			get;
			set;
		}

		public event EventHandler<GroupedDeliveryDestination> GroupedDeliveryDestinationChanged;
		public event EventHandler<ExtendedAddress> AddressChanged;

		public Page ParenntPage { get; set; }
		public bool IsShowCurrentLocation
		{
			get;
			set;
		} = true;

		public bool IsBusiness
		{
			get;
			set;
		} = false;

		public bool IsGroupedDeliveryDestination
		{
			get;
			set;
		} = false;

		public string FullName { get; set; }
		public string EmailAddress { get; set; }

		public AddressEntry()
		{
			this.Focused += async (sender, e) =>
			{
				if (e.IsFocused)
				{
					if (Device.RuntimePlatform == Device.iOS)
					{
						Device.BeginInvokeOnMainThread(() =>
						{
							this.IsNeedClearFocus = true;
							OnPropertyChanged(nameof(IsNeedClearFocus));
						});
					}
					else
					{
						//Device.BeginInvokeOnMainThread(() =>
						//{
						this.IsEnabled = false;
						this.Unfocus();
						//});
					}

					if (IsGroupedDeliveryDestination)
					{

						GroupedDeliveryAddressListPage addressSuggestionListPage = new GroupedDeliveryAddressListPage(ParenntPage, (obj) =>
						{
							if (GroupedDeliveryDestinationChanged != null)
							{
								GroupedDeliveryDestinationChanged(this, obj);
							}
						});
						await Utils.PushAsync(Navigation, addressSuggestionListPage, true);
					}
					else
					{
						AddressSuggestionListPage addressSuggestionListPage = new AddressSuggestionListPage(ParenntPage, (obj) =>
						{
							if (AddressChanged != null)
							{
								AddressChanged(this, obj);
							}
						}, IsBusiness);
						addressSuggestionListPage.EmailAddress = EmailAddress;
						addressSuggestionListPage.FullName = FullName;
						addressSuggestionListPage.IsShowCurrentLocation = IsShowCurrentLocation;
						await Utils.PushAsync(Navigation, addressSuggestionListPage, true);
					}
				}
			};

			this.Unfocused += (sender, e) =>
			{
				this.IsEnabled = true;
                this.IsVisible = true;
			};
		}
	}
}
