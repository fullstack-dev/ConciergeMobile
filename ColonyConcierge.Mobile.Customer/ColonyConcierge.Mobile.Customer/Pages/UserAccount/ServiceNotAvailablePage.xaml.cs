using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ColonyConcierge.APIData.Data;
using ColonyConcierge.Mobile.Customer.Localization.Resx;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ColonyConcierge.Mobile.Customer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ServiceNotAvailablePage : ContentPageBase
	{
		private bool mFistLoad = true;
		private int mId;
		IAppServices mAppServices;

		public ServiceNotAvailablePage(ExtendedAddress extendedAddress, string fullName = "", string email = "")
		{
			InitializeComponent();

			mAppServices = DependencyService.Get<IAppServices>();
			if (!string.IsNullOrEmpty(fullName))
			{
				EntryFullName.Text = fullName;
			}

			if (!string.IsNullOrEmpty(email))
			{
				EntryEmail.Text = email;
			}
			if (!string.IsNullOrEmpty(extendedAddress.BasicAddress.ZipCode))
			{
				LabelServicesAreNotCurrentlyAvailable.Text = AppResources.ServicesAreNotCurrentlyAvailableZipCode + " " + extendedAddress.BasicAddress.ZipCode + ".";
			}
			else
			{
				LabelServicesAreNotCurrentlyAvailable.Text = AppResources.ServicesAreNotCurrentlyAvailable;
			}

			GreenButtonNotifyMe.Clicked += (sender, e) =>
			{
				this.IsBusy = true;
				var fullname = EntryFullName.Text;
				var id = EntryFullName.Text;
				Task.Run(() =>
				{
					mAppServices.AddServiceNotAvaible(mId.ToString(), EntryFullName.Text, EntryEmail.Text, extendedAddress);
				}).ContinueWith((arg) =>
				{
					Utils.ShowSuccessMessage(AppResources.LeadsGenerationMessage, 4);
					Navigation.PopAsync();
					this.IsBusy = false;
				}, TaskScheduler.FromCurrentSynchronizationContext());
			};
		}

		public override void ReloadPage()
		{
			base.ReloadPage();

			LoadData();
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			if (mFistLoad)
			{
				LoadData();
			}
		}

		public async void LoadData()
		{
			Utils.IReloadPageCurrent = this;
			this.IsBusy = true;
			mFistLoad = false;
			ColonyConcierge.APIData.Data.User userModel = null;
			try
			{
				if (Shared.IsLoggedIn)
				{
					userModel = await Shared.APIs.IUsers.GetCurrentUser_Async();
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
			if (Utils.IReloadPageCurrent == this)
			{
				Utils.IReloadPageCurrent = null;
			}
			if (userModel != null)
			{
				EntryFullName.Text = userModel.FirstName + " " + userModel.LastName;
				EntryEmail.Text = userModel.EmailAddress;
				mId = userModel.ID;
			}
			this.IsBusy = false;
		}
	}
}
