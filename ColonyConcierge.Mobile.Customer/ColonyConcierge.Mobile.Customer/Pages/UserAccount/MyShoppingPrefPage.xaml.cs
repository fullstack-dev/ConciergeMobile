
﻿using ColonyConcierge.APIData.Data;
using ColonyConcierge.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ColonyConcierge.Mobile.Customer.Localization.Resx;
using Plugin.Toasts;
using System.Collections.ObjectModel;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ColonyConcierge.Mobile.Customer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MyShoppingPrefPage : ContentPageBase
	{
		public event EventHandler<ShoppingPreference> ShoppingPreferenceChanged;
		User userModel;
		private bool mFirstLoad = true;

		public MyShoppingPrefPage()
		{
			InitializeComponent();
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			if (mFirstLoad)
			{
				mFirstLoad = false;
				GetShoppingPrefModel();
			}
		}

		public override void ReloadPage()
		{
			base.ReloadPage();
            GetShoppingPrefModel();
		}

		public void GetShoppingPrefModel()
		{
			UserFacade userFacade = new UserFacade();
			userFacade.RequireLogin(this, async () =>
			{
				this.IsBusy = true;
				Utils.IReloadPageCurrent = this;
				try
				{
					userModel = await Shared.APIs.IUsers.GetCurrentUser_Async();
					if (userModel != null)
					{
						this.BindingContext = await Task.Run(() => { return Shared.APIs.IAccounts.GetShoppingPreference(userModel.AccountID); });
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
				this.IsBusy = false;
			});
		}

		async private void Button_Clicked(object sender, EventArgs e)
		{
			this.IsBusy = true;
			ShoppingPreference shoppingPref = (ShoppingPreference)this.BindingContext;
			if (shoppingPref.DefaultCanSubstituteLarger || shoppingPref.DefaultCanSubstituteSmaller)
			{
				shoppingPref.DefaultSizeSubstitution = true;
			}
			else
			{
				shoppingPref.DefaultSizeSubstitution = false;
			}
			try
			{
				await Task.Run(() => { return Shared.APIs.IAccounts.SetShoppingPreference(userModel.AccountID, shoppingPref); });

				if (ShoppingPreferenceChanged != null)
				{
					ShoppingPreferenceChanged(this, shoppingPref);
				}

				var notificator = DependencyService.Get<IToastNotificator>();
				await notificator.Notify(ToastNotificationType.Success, AppResources.ShoppingTitle, AppResources.MYShoppingPrefMessage, TimeSpan.FromSeconds(2));

				await Navigation.PopAsync().ConfigureAwait(false);
			}
			catch (Exception ex)
			{
				var notificator = DependencyService.Get<IToastNotificator>();
				await notificator.Notify(ToastNotificationType.Error, AppResources.ShoppingTitle, ex.Message, TimeSpan.FromSeconds(2));
				System.Diagnostics.Debug.WriteLine("Exception occured When Set Shopping  Preferencess : " + ex.Message);
			}
			finally
			{
				this.IsBusy = false;
			}
		}

	}
}
