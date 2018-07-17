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
    public partial class PaymentPage : ContentPageBase
	{
		private bool mFirstLoad = true;
		ColonyConcierge.APIData.Data.User UserModel;

		public Command AddCommand
		{
			get
			{
				return new Command((obj) =>
				{
					toolbarOnClick(this, EventArgs.Empty);
				});
			}
		}

		private List<CreditCardDataItemViewModel> paymentMethodList;
		public List<CreditCardDataItemViewModel> PaymentMethodList
		{
			get { return paymentMethodList; }
			set
			{
				paymentMethodList = value;
				this.OnPropertyChanged(nameof(PaymentMethodList));
				// Add the PaymentMethodList as PaymentAccountData in List

			}
		}

		public PaymentPage()
		{
			InitializeComponent();
		}

		async public Task PopulateItemsInList()
		{
			this.IsBusy = true;
			Utils.IReloadPageCurrent = this;
			UserModel = Shared.APIs.IUsers.GetCurrentUser();
			try
			{
				if (UserModel != null)
				{
					this.PaymentMethodList = await Task.Run(() =>
					{
						try
						{
						//return api.IAccounts.GetPaymentMethods(userModel.AccountID);
						return Shared.APIs.IAccounts.BtGetPaymentMethods(UserModel.AccountID).Select(s => new CreditCardDataItemViewModel(s)).ToList();
						}
						catch (Exception ex)
						{
							Device.BeginInvokeOnMainThread(async () =>
							{
								var message = string.IsNullOrEmpty(ex.Message) ? AppResources.SomethingWentWrong : ex.Message;
								await Utils.ShowErrorMessage(message);
							});
							return new List<CreditCardDataItemViewModel>();
						}
					});
				}
				LabelNoPaymentMethods.IsVisible = (PaymentMethodList == null || PaymentMethodList.Count == 0);
			}
			catch (Exception e)
			{
				System.Diagnostics.Debug.WriteLine("Exceptions", e.Message);
			}
			if (Utils.IReloadPageCurrent == this)
			{
				Utils.IReloadPageCurrent = null;
			}
			lstView.IsVisible = true;
			this.IsBusy = false;
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			if (mFirstLoad)
			{
				mFirstLoad = false;
				Device.BeginInvokeOnMainThread(async() =>
				{
					await PopulateItemsInList();
				});
			}

			//var userModel = Shared.APIs.IUsers.GetCurrentUser();
			//var api = new APIs(new Connector());
			//api.LoginToken = Shared.LoginToken;
			//try
			//{
			//	this.PaymentMethodList = await Task.Run(() => 
			//	{
			//		//return api.IAccounts.GetPaymentMethods(userModel.AccountID);
			//		return api.IAccounts.BtGetPaymentMethods(userModel.AccountID).Select(s => new CreditCardDataItemViewModel(s)).ToList(); 
			//	});
			//}
			//catch (Exception e)
			//{
			//	System.Diagnostics.Debug.WriteLine("Exceptions", e.Message);
			//}
		}

		private void toolbarOnClick(object sender, EventArgs e)
		{
			Utils.PushAsync(Navigation, new BrainTreePaymentMethodPage(false, (page, cardId) =>
			{
				this.IsBusy = true;
				Task.Run(async () =>
				{
					try
					{
						await PopulateItemsInList();
					}
					catch (Exception ex)
					{
						Device.BeginInvokeOnMainThread(async () =>
						{
							var message = string.IsNullOrEmpty(ex.Message) ? AppResources.SomethingWentWrong : ex.Message;
							await Utils.ShowErrorMessage(message);
						});
					}

					this.IsBusy = false;
					Device.BeginInvokeOnMainThread(() =>
					{
						page.Navigation.PopAsync().ConfigureAwait(false);
					});
				});

				return true;
			}));
		}

		async private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			this.IsBusy = true;
			if (e.SelectedItem == null) return;
			var paymentMethod = ((CreditCardDataItemViewModel)e.SelectedItem).Model;
			lstView.SelectedItem = null;

			//var userModel = Shared.APIs.IUsers.GetCurrentUser();
			try
			{
				//var paymentAccount= await Task.Run(() => { return Shared.APIs.IAccounts.GetPaymentMethodForEditing(userModel.AccountID,paymentMethod.ID); });
				//paymentAccount.ID = paymentMethod.ID;
				//CreditCardData CreditCardData = (CreditCardData)paymentAccount;
				await Utils.PushAsync(Navigation, new BrainTreePaymentMethodViewPage(paymentMethod, this));
			}
			catch (Exception ex)
			{
				var notificator = DependencyService.Get<IToastNotificator>();
				await notificator.Notify(ToastNotificationType.Error, AppResources.Edit, ex.Message, TimeSpan.FromSeconds(2));
				System.Diagnostics.Debug.WriteLine("Exception occured When Add Payment Method : " + ex.Message);
			}
			finally
			{
				this.IsBusy = false;
			}
		}
	}
}
