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
    public partial class BrainTreePaymentMethodViewPage : ContentPageBase
	{
		//private IAppServices mAppServices;
		private PaymentPage mPaymentPage;

		public BrainTreePaymentMethodViewPage(CreditCardData creditCardData, PaymentPage paymentPage)
		{
			InitializeComponent();

			NavigationPage.SetBackButtonTitle(this, AppResources.Back);

			mPaymentPage = paymentPage;
			//mAppServices = DependencyService.Get<IAppServices>();

			this.Title = creditCardData.creditType;

			this.ToolbarItems.Add(new ToolbarItem()
			{
				Text = AppResources.Delete,
				Command = new Command(async () =>
				{
					bool delete = await DisplayAlert(AppResources.Delete, AppResources.DeleteCardMessage, AppResources.Yes, AppResources.No);
					if (delete)
					{
						var isSucceed = false;
						this.IsBusy = true;
						await Task.Run(() =>
						{
							try
							{
								var userModel = Shared.APIs.IUsers.GetCurrentUser();
								isSucceed = Shared.APIs.IAccounts.BtDeletePaymentMethod(userModel.AccountID, creditCardData.ID);
							}
							catch (Exception ex)
							{
								Device.BeginInvokeOnMainThread(async () =>
								{
									var message = string.IsNullOrEmpty(ex.Message) ? AppResources.SomethingWentWrong : ex.Message;
									await Utils.ShowErrorMessage(message);
								});
							}
						}).ContinueWith(async t =>
						{
							if (isSucceed)
							{
								if (mPaymentPage != null)
								{
									await mPaymentPage.PopulateItemsInList();
								}
								this.IsBusy = false;
								await Navigation.PopAsync().ConfigureAwait(false);
							}
							this.IsBusy = false;
						}, TaskScheduler.FromCurrentSynchronizationContext());

					}
				})
			});

			this.ToolbarItems.Add(new ToolbarItem()
			{
				Text = AppResources.Edit,
				Command = new Command(async() =>
				{
					await Utils.PushAsync(Navigation, new UpdateBrainTreePaymentMethodPage(creditCardData, (arg) =>
					{
						this.IsBusy = true;
						Task.Run(() =>
						{
							try
							{
								var userModel = Shared.APIs.IUsers.GetCurrentUser();
								var newCreditCardData = Shared.APIs.IAccounts.BtGetPaymentMethods(userModel.AccountID).FirstOrDefault(s => s.ID == creditCardData.ID);
								if (newCreditCardData != null)
								{
									creditCardData = newCreditCardData;
								}
							}
							catch (Exception)
							{
								Device.BeginInvokeOnMainThread(async () =>
								{
									await Utils.ShowErrorMessage(AppResources.SomethingWentWrong, 2);
								});
							}
							Device.BeginInvokeOnMainThread(async () =>
							{
								LabelPreferred.Text = creditCardData.IsPreferred ? AppResources.Yes : AppResources.No;
								LabelExpiryDate.Text = (creditCardData.ExpirationMonth > 9 ? creditCardData.ExpirationMonth + "" : "0" + creditCardData.ExpirationMonth)
														+ "/" + creditCardData.ExpirationYear;
								LabelCreditCardNumber.Text = creditCardData.CreditCardNumber;
								LabelCardNickName.Text = creditCardData.AccountNickname;
								if (mPaymentPage != null)
								{
									await mPaymentPage.PopulateItemsInList();
								}
								this.IsBusy = false;
								await arg.Navigation.PopAsync().ConfigureAwait(false);
							});
						});
						return true;
					}));
				})
			});

			this.BindingContext = creditCardData;

			LabelPreferred.Text = creditCardData.IsPreferred ? AppResources.Yes : AppResources.No;
			LabelExpiryDate.Text = (creditCardData.ExpirationMonth > 9 ? creditCardData.ExpirationMonth + "" : "0" + creditCardData.ExpirationMonth)
									+ "/" + creditCardData.ExpirationYear;
			if (creditCardData == null || string.IsNullOrEmpty(creditCardData.CreditCardNumber))
			{
				LabelCreditCardNumber.Text = string.Empty;
			}
			else
			{
				var index = creditCardData.CreditCardNumber.IndexOf("*", StringComparison.OrdinalIgnoreCase);
				LabelCreditCardNumber.Text = creditCardData.CreditCardNumber.Substring(index + 1);
			}
		
			LabelCardNickName.Text = creditCardData.AccountNickname;
		}
	}
}
