using ColonyConcierge.APIData.Data;
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
    public partial class ContactInformationPage : ContentPageBase
	{
		PhoneNumber mobile;
		PhoneNumber work;
		PhoneNumber home;
		private bool mFirstLoad = true;

		List<PhoneNumber> phoneNumbers = new List<PhoneNumber>();

		public ContactInformationPage()
		{
			InitializeComponent();
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			if (mFirstLoad)
			{
				mFirstLoad = false;
				EditInformation();
			}
		}


		async private void EditInformation()
		{
			this.IsBusy = true;

			Utils.IReloadPageCurrent = this;
			var userModel = await Shared.APIs.IUsers.GetCurrentUser_Async();
			if (userModel != null)
			{
				this.firstNameEntry.Text = userModel.FirstName;
				this.middleNameEntry.Text = userModel.MiddleName;
				this.lastNameEntry.Text = userModel.LastName;
				this.emailEntry.Text = userModel.EmailAddress;
				try
				{
					phoneNumbers = await Task.Run(() => { return Shared.APIs.IUsers.GetPhoneNumbers(Shared.UserId); });
					if (phoneNumbers != null)
					{
						mobile = phoneNumbers.FirstOrDefault(x => x.Type == "Mobile");
						home = phoneNumbers.FirstOrDefault(x => x.Type == "Home");
						work = phoneNumbers.FirstOrDefault(x => x.Type == "Work");
						if (mobile != null)
						{
							System.Diagnostics.Debug.WriteLine("Mobile Number ID is {0}", mobile.ID);
							this.mobileNumberEntry.Text = mobile.Number;
						}
						if (home != null)
						{
							this.homeNumberEntry.Text = home.Number;
						}
						if (work != null)
						{
							this.workNumberEntry.Text = work.Number;
						}
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
			}

			if (!this.IsErrorPage)
			{
				this.GridContent.IsVisible = true;
			}
            this.IsBusy = false;

			if (Utils.IReloadPageCurrent == this)
			{
				Utils.IReloadPageCurrent = null;
			}
		}

		public override void ReloadPage()
		{
			base.ReloadPage();
            EditInformation();
		}

		async private void Save_Button_Clicked(object sender, EventArgs e)
		{
			this.IsBusy = true;
			(sender as VisualElement).IsEnabled = false;
			var success = false;

			try
			{
				var user = await Shared.APIs.IUsers.GetCurrentUser_Async();
				user.FirstName = this.firstNameEntry.Text;
				user.MiddleName = this.middleNameEntry.Text;
				user.LastName = this.lastNameEntry.Text;
				user.EmailAddress = this.emailEntry.Text;

				if (mobile != null)
				{
					mobile.Number = mobileNumberEntry.Text;
					mobile = Shared.APIs.IUsers.SetPhoneNumber(Shared.UserId, mobile.ID, mobile);
				}
				else if (!string.IsNullOrEmpty(mobileNumberEntry.Text))
				{
					mobile = new PhoneNumber();
					mobile.Priority = 1;
					mobile.Type = "Mobile";
					mobile.Number = homeNumberEntry.Text;
					success = Shared.APIs.IUsers.AddPhoneNumber(Shared.UserId, mobile) > 0;
				}

				if (home != null)
				{
					home.Number = homeNumberEntry.Text;
					home = Shared.APIs.IUsers.SetPhoneNumber(Shared.UserId, home.ID, home);
				}
				else if (!string.IsNullOrEmpty(homeNumberEntry.Text))
				{
					home = new PhoneNumber();
					home.Priority = 2;
					home.Type = "Home";
					home.Number = homeNumberEntry.Text;
					success = Shared.APIs.IUsers.AddPhoneNumber(Shared.UserId, home) > 0;
				}

				if (work != null)
				{
					work.Number = workNumberEntry.Text;
					work = Shared.APIs.IUsers.SetPhoneNumber(Shared.UserId, work.ID, work);
				}
				else if (!string.IsNullOrEmpty(workNumberEntry.Text))
				{
					work = new PhoneNumber();
					work.Priority = 3;
					work.Type = "Work";
					work.Number = workNumberEntry.Text;
					success = Shared.APIs.IUsers.AddPhoneNumber(Shared.UserId, work) > 0;
				}

				success = Shared.APIs.IUsers.SetUser(user);

				if (success == true)
				{
					var notificator = DependencyService.Get<IToastNotificator>();
					await notificator.Notify(ToastNotificationType.Success, AppResources.ContactInfoTitle, AppResources.UpdateInformation, TimeSpan.FromSeconds(5));
					await Navigation.PopAsync().ConfigureAwait(false);
				}
				else
				{
					var notificator = DependencyService.Get<IToastNotificator>();
					await notificator.Notify(ToastNotificationType.Error, AppResources.ContactInfoTitle, AppResources.UpdateError, TimeSpan.FromSeconds(5));
				}
			}
			catch (Exception)
			{
				if (success == true)
				{
					var notificator = DependencyService.Get<IToastNotificator>();
					await notificator.Notify(ToastNotificationType.Success, AppResources.ContactInfoTitle, AppResources.UpdateInformation, TimeSpan.FromSeconds(5));
					await Navigation.PopAsync().ConfigureAwait(false);
				}
				else
				{
					var notificator = DependencyService.Get<IToastNotificator>();
					await notificator.Notify(ToastNotificationType.Error, AppResources.ContactInfoTitle, AppResources.UpdateError, TimeSpan.FromSeconds(5));
				}
			}
			finally
			{
				this.IsBusy = false;
				(sender as VisualElement).IsEnabled = true;
			}
		}
	}
}
