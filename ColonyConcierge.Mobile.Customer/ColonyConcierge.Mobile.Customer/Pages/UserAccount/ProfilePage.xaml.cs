
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
using System.Collections.ObjectModel;
using Xamarin.Forms.Xaml;

namespace ColonyConcierge.Mobile.Customer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProfilePage : ContentPageBase
	{
		private bool mIsNeedShowErrorPage = false;

		public ProfilePage()
		{
			InitializeComponent();

			NavigationPage.SetBackButtonTitle(this, AppResources.Back);

			this.Profiles = new ObservableCollection<ProfilePageItemViewModel>();
			this.Profiles.Add(new ProfilePageItemViewModel { Name = AppResources.ServiceAddress , IconSource = "home.png"});
			this.Profiles.Add(new ProfilePageItemViewModel { Name = AppResources.Payment , IconSource = "creditCard.png"});
			this.Profiles.Add(new ProfilePageItemViewModel { Name = AppResources.ShoppingPreferances , IconSource = "shopping.png"});
			this.Profiles.Add(new ProfilePageItemViewModel { Name = AppResources.UpdateSubscription , IconSource = "updation.png"});
			//this.Profiles.Add(new ProfilePageItemViewModel { Name = AppResources.SettingTitle , IconSource = "settings.png"});
			this.Profiles.Add(new ProfilePageItemViewModel { Name = AppResources.AccountCancel , IconSource = "cancel.png"});

			EditButton.GestureRecognizers.Add(new TapGestureRecognizer()
			{
				Command = new Command(async () =>
				{
					EditButton.TextColor = AppearanceBase.Instance.OrangeColor;
					await Utils.PushAsync(Navigation, new ContactInformationPage());
					EditButton.TextColor = AppearanceBase.Instance.PrimaryColor;
				})
			});

			ActivityIndicatorProfile.IsVisible = true;
		}

		public override void ShowLoadErrorPage()
		{
			if (mIsNeedShowErrorPage)
			{
				base.ShowLoadErrorPage();
			}
		}

		public void LoadData(bool isRefresh = false)
		{
			User userModel = null;
			List<PhoneNumber> phoneNumbers = new List<PhoneNumber>();
			mIsNeedShowErrorPage = !isRefresh;
			if (Shared.IsLoggedIn)
			{
				Task.Run(() =>
				{
					Utils.IReloadPageCurrent = this;
					try
					{
						userModel = Shared.APIs.IUsers.GetCurrentUser();
						phoneNumbers = Shared.APIs.IUsers.GetPhoneNumbers(Shared.UserId);
					}
					catch (Exception ex)
					{
						if (this.IsErrorPage)
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
				}).ContinueWith((obj) =>
				{
					if (!this.IsErrorPage)
					{
						if (userModel != null)
						{
							this.Name.Text = userModel.FirstName + " " + userModel.LastName;
							this.Email.Text = userModel.EmailAddress;
						}
						if (phoneNumbers != null)
						{
							var mobile = phoneNumbers.Find(x => x.Type == "Mobile");
							this.Phone.Text = mobile.Number;
							StackLayoutProfile.Opacity = 1;
							ActivityIndicatorProfile.IsVisible = false;
						}
					}
				}, TaskScheduler.FromCurrentSynchronizationContext());
			}
			else
			{
				StackLayoutProfile.Opacity = 1;
				ActivityIndicatorProfile.IsVisible = false;
			}
		}

		public override void ReloadPage()
		{
			base.ReloadPage();
			StackLayoutProfile.Opacity = 0;
			ActivityIndicatorProfile.IsVisible = true;
			LoadData();
		}

		private bool mFirstLoad = true;
		protected override void OnAppearing()
		{
			base.OnAppearing();

			if (mFirstLoad)
			{
				mFirstLoad = false;
				LoadData();
			}
			else
			{
                LoadData(true);
			}
		}

		private ObservableCollection<ProfilePageItemViewModel> profiles;

		public ObservableCollection<ProfilePageItemViewModel> Profiles
		{
			get { return profiles; }
			set
			{
				profiles = value;

				this.OnPropertyChanged(nameof(Profiles));
			}
		}

		protected override bool OnBackButtonPressed()
		{
			Application.Current.MainPage = new HomePage();
			return true;
		}

		async private void tappedListViewItem(object sender, SelectedItemChangedEventArgs e)
		{
			var item = e.SelectedItem as ProfilePageItemViewModel;
			lstView.SelectedItem = null;
			if (item == null) return;
			if (item.Name == AppResources.ServiceAddress)
			{
				this.IsBusy = true;
				await Utils.PushAsync(Navigation, new AccountAddressPage());
				this.IsBusy = false;
			}
			else if (item.Name == AppResources.Payment)
			{
				this.IsBusy = true;
				await Utils.PushAsync(Navigation, new PaymentPage());
				this.IsBusy = false;
			}
			else if (item.Name == AppResources.ShoppingPreferances)
			{
				this.IsBusy = true;
				await Utils.PushAsync(Navigation, new MyShoppingPrefPage());
				this.IsBusy = false;
			}
			else if (item.Name == AppResources.UpdateSubscription)
			{
				this.IsBusy = true;
				await Utils.PushAsync(Navigation, new MySubscriptionPage());
				this.IsBusy = false;
			}
			else if (item.Name == AppResources.SettingTitle)
			{
				this.IsBusy = true;
				await Utils.PushAsync(Navigation, new SettingsPage());
				this.IsBusy = false;
			}
			else if(item.Name == AppResources.AccountCancel)
			{
				var ans = await DisplayAlert(AppResources.AccountCancel, AppResources.AccountCancelMessage, AppResources.Yes, AppResources.No);
				if (ans)
				{
					//Success condition
					//bool cancelAccount = await Task.Run(() => { return Shared.APIs.IAccounts.RequestAccountCancelation(userModel.AccountID);});
					//
						//Success condition
						//App.Current.MainPage = new AuthPage();
					//}
					//else
					//{

					//}
				}
				else
				{
					//false conditon
				}
			}
		}
	}
}
