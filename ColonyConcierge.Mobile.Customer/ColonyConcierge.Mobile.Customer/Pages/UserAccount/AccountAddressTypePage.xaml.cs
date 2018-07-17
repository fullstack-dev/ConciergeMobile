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
    public partial class AccountAddressTypePage : ContentPageBase
	{
		private string ZipCodeSelecting = string.Empty;
		private bool ISUpdate;
		private AddressFacade mAddressFacade = new AddressFacade();
		//private bool mFirstLoad = true;
		public bool IsBackAnimation
		{
			get;
			set;
		} = false;

		public AccountAddressTypePage(bool IsUpdate)
		{
			InitializeComponent();
			ISUpdate = IsUpdate;
			NavigationPage.SetBackButtonTitle(this, AppResources.Back);

			if (IsUpdate)
			{
				StackLayoutTitle.IsVisible = false;
				LabelTitle.TextColor = AppearanceBase.Instance.PrimaryColor;
				GridLineTitle.BackgroundColor = AppearanceBase.Instance.PrimaryColor;
				this.NextButton.IsVisible = false;
				//this.BackButton.IsVisible = false;
			}
			else
			{
				this.SaveButton.IsVisible = false;
				//this.NextButton.IsVisible = false;
				//this.BackButton.IsVisible = false;
			}
			// Initialize places
			GmsPlace.Init(ConfigurationManager.AppSettings["PlacesApiKey"]);

			this.MessageVisible = false;
			addressEntry.AddressChanged += (sender, e) =>
			{
				if (e != null)
				{
					RegistrationEntry mUserInfo = (RegistrationEntry)this.BindingContext;
					if (mUserInfo.ServiceAddress != null)
					{
						mUserInfo.ServiceAddress.BasicAddress = e.BasicAddress;
						mUserInfo.ServiceAddress.Longitude = e.Longitude;
						mUserInfo.ServiceAddress.Latitude = e.Latitude;
					}
					else
					{
						mUserInfo.ServiceAddress = e;
					}
					this.SelectedZipCode = e.BasicAddress.ZipCode;
					this.SearchText = e.BasicAddress.ToAddress();
					this.apartmentEntry.Text = e.BasicAddress.Line2;
				}
			};

			this.BindingContextChanged += AddressSuggestionListPage_BindingContextChanged;
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			//if (!ISUpdate)
			//{
			//	if (this.Width > 0 && this.Height > 0)
			//	{
			//		if (IsBackAnimation)
			//		{
			//			IsBackAnimation = false;
			//			this.TranslationX = -this.Width;
			//			Animation animation = new Animation((s) =>
			//			{
			//				this.TranslationX = -this.Width * s;
			//			}, 1, 0);
			//			animation.Commit(this, "OnAppearing", 16, 500, Easing.Linear);
			//		}
			//		else
			//		{
			//			if (mFirstLoad)
			//			{
			//				mFirstLoad = false;
			//				this.TranslationX = this.Width;
			//				Animation animation = new Animation((s) =>
			//				{
			//					this.TranslationX = this.Width * s;
			//				}, 1, 0);
			//				animation.Commit(this, "OnDisappearing", 16, 500, Easing.Linear);
			//			}

			//		}
			//	}
			//}
		}

		private void AddressSuggestionListPage_BindingContextChanged(object sender, EventArgs e)
		{
			if (this.BindingContext is RegistrationEntry)
			{
				RegistrationEntry userInfo = (RegistrationEntry)this.BindingContext;
				this.SelectedZipCode = userInfo.ServiceAddress.BasicAddress.ZipCode;
				Task.Run(() =>
				{
					new System.Threading.ManualResetEvent(false).WaitOne(200);
				}).ContinueWith(t =>
				{
					this.SearchText = userInfo.ServiceAddress.BasicAddress.ToAddress();
					this.apartmentEntry.Text = userInfo.ServiceAddress.BasicAddress.Line2;
					addressEntry.FullName = userInfo.FirstName + " " + userInfo.LastName;
					addressEntry.EmailAddress = userInfo.EmailAddress;
				}, TaskScheduler.FromCurrentSynchronizationContext());
			}
		}

		private bool mMessageVisible;
		public bool MessageVisible
		{
			get
			{
				return mMessageVisible;
			}
			set
			{
				mMessageVisible = value;
				NotifyPropertyChanged(nameof(MessageVisible));
			}
		}

		public string ZipValid
		{
			get
			{
				if (this.BindingContext != null)
				{
					RegistrationEntry userInfo = (RegistrationEntry)this.BindingContext;

					if (userInfo == null || !mAddressFacade.ValidateAddress(userInfo.ServiceAddress))
					{
						// Invalid or incomplete address
						return AppResources.InvalidAddress;
					}
				}

				return string.IsNullOrEmpty(selectedZip) ?
							 string.Format(AppResources.ZipInvalid, ZipCodeSelecting)
							 : AppResources.ZipValid;
			}
		}

		public bool IsValid
		{
			get
			{
				if (this.BindingContext != null)
				{
					RegistrationEntry userInfo = (RegistrationEntry)this.BindingContext;

					if (userInfo == null || !mAddressFacade.ValidateAddress(userInfo.ServiceAddress))
					{
						// Invalid or incomplete address
						return false;
					}
				}

				return !string.IsNullOrEmpty(selectedZip);
			}
		}

		private string searchText;
		public string SearchText
		{
			get { return searchText; }
			set
			{
				NotifyPropertyChanging(nameof(SearchText));
				searchText = value;
				NotifyPropertyChanged(nameof(SearchText));
			}
		}

		private string selectedZip;
		public string SelectedZipCode
		{
			get
			{
				return selectedZip;
			}
			set
			{
				selectedZip = value;
				NotifyPropertyChanged(nameof(SelectedZipCode));
				NotifyPropertyChanged(nameof(ZipValid));
				NotifyPropertyChanged(nameof(IsValid));
			}
		}

		private ObservableCollection<Prediction> places;

		public ObservableCollection<Prediction> Places
		{
			get { return places; }
			set
			{
				NotifyPropertyChanging(nameof(Places));
				places = value;
				NotifyPropertyChanged(nameof(Places));
			}
		}


		private Prediction selectedPlace;

		public Prediction SelectedPlace
		{
			get { return selectedPlace; }
			set
			{
				selectedPlace = value;
			}
		}

		private void NotifyPropertyChanging(string propertyName)
		{
			OnPropertyChanging(propertyName);
		}

		private void NotifyPropertyChanged(string propertyName)
		{
			OnPropertyChanged(propertyName);
		}

		private async void FindPostalCode()
		{
			if (selectedPlace.Description == AppResources.UseCurrentLocation)
			{

			}
			else
			{
				this.IsBusy = true;

				try
				{
					var details = await GmsPlace.Instance.GetDetails(selectedPlace.PlaceId);
					this.SelectedZipCode = string.Empty;

					// Check if the place has a postal code
					if (details.Status == GmsDetailsResultStatus.Ok)
					{
						await UpdateLocation(details.Item);
					}
				}
				catch (Exception) { }

				this.IsBusy = false;
			}
		}

		public Task UpdateLocation(GmsDetailsResultItem msDetailsResult)
		{
			RegistrationEntry mUserInfo = (RegistrationEntry)this.BindingContext;
			Device.BeginInvokeOnMainThread(() =>
			{
				this.MessageVisible = false;
			});
			try
			{
				if (mUserInfo.ServiceAddress == null)
				{
					mUserInfo.ServiceAddress = new ExtendedAddress { BasicAddress = new Address() };
				}

				// Check to see if service is provided in this area
				mAddressFacade.FillExtendedAddress(mUserInfo.ServiceAddress, msDetailsResult);
				ZipCodeSelecting = mUserInfo.ServiceAddress.BasicAddress.ZipCode;

				SerivesFacade serivesService = new SerivesFacade();
				var zipCode = serivesService.CheckAvailableServices(mUserInfo.ServiceAddress);
				//if (!string.IsNullOrEmpty(zipCode))
				//{
				//	Shared.LocalAddress = mUserInfo.ServiceAddress;
				//}
				this.SelectedZipCode = zipCode;
			}
			catch (Exception) { }
			finally
			{
				Device.BeginInvokeOnMainThread(() =>
				{
					this.MessageVisible = true;
					if (IsValid)
					{
						this.SearchText = mUserInfo.ServiceAddress.BasicAddress.ToAddress();
					}
				});
			}
			return Task.FromResult(1);
		}

		private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			if (e.SelectedItem == null) return;
			var prediction = (Prediction)e.SelectedItem;

			this.HandleItemSelected(prediction);
		}

		private void HandleItemSelected(Prediction prediction)
		{
			RegistrationEntry mUserInfo = (RegistrationEntry)this.BindingContext;
			this.SearchText = prediction.Description;
			this.SelectedZipCode = string.Empty;
			this.SelectedPlace = prediction;
			this.FindPostalCode();

			this.Reset();
		}

		private void Reset()
		{
			this.addressEntry.Unfocus();
		}


		//private void OnCompleted(object sender, EventArgs e)
		//{
		//	this.Reset();
		//	this.apartmentEntry.Focus();
		//}

		//private void OnEntryUnFocus(object sender, FocusEventArgs e)
		//{
		//	this.Reset();
		//}

		async private void ButtonNext_Clicked(object sender, EventArgs e)
		{
			RegistrationEntry mUserInfo = (RegistrationEntry)this.BindingContext;
			mUserInfo.ServiceAddress.IsPreferred = true;

			if (ISUpdate)
			{
				this.IsBusy = true;
				try
				{
					var user = await Shared.APIs.IUsers.GetCurrentUser_Async();
					if (mUserInfo.ServiceAddress.ID > 0)
					{
						var address = await Task.Run(() =>
						{
							return Shared.APIs.IAccounts.SetServiceAddress(user.AccountID, mUserInfo.ServiceAddress.ID, mUserInfo.ServiceAddress);
						});
						if (address != null)
						{
							var notificator = DependencyService.Get<IToastNotificator>();
							await notificator.Notify(ToastNotificationType.Success, AppResources.ServiceAddressTitle, AppResources.ServiceAddressSuccess, TimeSpan.FromSeconds(2));

							await Navigation.PopAsync().ConfigureAwait(false);
						}
						else
						{
							var notificator = DependencyService.Get<IToastNotificator>();
							await notificator.Notify(ToastNotificationType.Error, AppResources.ServiceAddressTitle, AppResources.ServiceAddressError, TimeSpan.FromSeconds(2));
						}
					}
					else
					{
						var isAddress = await Task.Run(() =>
						{
							return Shared.APIs.IAccounts.AddServiceAddress(user.AccountID, mUserInfo.ServiceAddress);
						});
						if (isAddress > 0)
						{
							var notificator = DependencyService.Get<IToastNotificator>();
							await notificator.Notify(ToastNotificationType.Success, AppResources.ServiceAddressTitle, AppResources.ServiceAddressSuccess, TimeSpan.FromSeconds(2));

							await Navigation.PopAsync().ConfigureAwait(false);
						}
						else
						{
							var notificator = DependencyService.Get<IToastNotificator>();
							await notificator.Notify(ToastNotificationType.Error, AppResources.ServiceAddressTitle, AppResources.ServiceAddressError, TimeSpan.FromSeconds(2));
						}
					}
				}
				catch (Exception ex)
				{
					var notificator = DependencyService.Get<IToastNotificator>();
					await notificator.Notify(ToastNotificationType.Error, AppResources.ServiceAddressTitle, ex.Message, TimeSpan.FromSeconds(2));
				}
				finally
				{
					this.IsBusy = false;
				}
			}
			else
			{
				await Utils.PushAsync(Navigation, new AccountTypePage()
				{
					BindingContext = this.BindingContext,
				}, true);
				IsBackAnimation = true;
			}
		}

		async private void ButtonBack_Clicked(object sender, EventArgs e)
		{
			await Navigation.PopAsync().ConfigureAwait(false);
		}
	}
}
