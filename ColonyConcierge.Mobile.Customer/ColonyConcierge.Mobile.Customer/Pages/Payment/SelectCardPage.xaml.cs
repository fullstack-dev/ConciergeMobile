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
    public partial class SelectCardPage : ContentPageBase
	{
		ColonyConcierge.APIData.Data.User UserModel;
		CreditCardData CreditCardDataSelected;

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

		public event EventHandler<CreditCardDataItemViewModel> CardSelected;

		public SelectCardPage(CreditCardData creditCardDataSelected = null)
		{
			InitializeComponent();

			CreditCardDataSelected = creditCardDataSelected;

			lstView.ItemTapped += (sender, e) =>
			{
				if (CardSelected != null)
				{
					CardSelected(this, e.Item as CreditCardDataItemViewModel);
				}
				else
				{
					Navigation.PopAsync().ConfigureAwait(false);
				}
			};
		}

		public async override void ReloadPage()
		{
			base.ReloadPage();

			await PopulateItemsInList();
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

			Device.BeginInvokeOnMainThread(async () =>
			{
				await PopulateItemsInList();
			});
		}

		private void toolbarOnClick(object sender, EventArgs e)
		{
			var isFirstCard = PaymentMethodList == null || PaymentMethodList.Count == 0;
			Utils.PushAsync(Navigation, new BrainTreePaymentMethodPage(isFirstCard, (page, cardId) =>
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
						if (!this.IsErrorPage && Utils.IReloadPageCurrent == this)
						{
							Device.BeginInvokeOnMainThread(async () =>
							{
								await Utils.ShowErrorMessage(string.IsNullOrEmpty(ex.Message) ? AppResources.SomethingWentWrong : ex.Message);
							});
						}
					}
				}).ContinueWith(s =>
				{
                    this.IsBusy = false;
					var card = PaymentMethodList.FirstOrDefault((pre) =>
					{
						return pre.Model.ID == cardId;
					});
					if (card == null)
					{
						card = PaymentMethodList.FirstOrDefault((pre) =>
						{
							return pre.Model.IsPreferred;
						});
					}
					if (CardSelected != null && card !=  null)
					{
						CardSelected(this, card);
					}
					else
					{
						page.Navigation.RemovePage(this);
						page.Navigation.PopAsync().ConfigureAwait(false);
					}
				}, TaskScheduler.FromCurrentSynchronizationContext());
				return true;
			}));
		}

		async public Task PopulateItemsInList()
		{
			Utils.IReloadPageCurrent = this;
			this.IsBusy = true;
			try
			{
				this.PaymentMethodList = await Task.Run(() =>
				{
					try
					{
						UserModel = Shared.APIs.IUsers.GetCurrentUser();
						if (UserModel != null)
						{
							return Shared.APIs.IAccounts.BtGetPaymentMethods(UserModel.AccountID)
										 .Select(s => new CreditCardDataItemViewModel(s)
										 {
											 IsSelected = CreditCardDataSelected != null && s.ID == CreditCardDataSelected.ID,
										 })
										 .ToList();
						}
					}
					catch (Exception ex)
					{
						if (!this.IsErrorPage && Utils.IReloadPageCurrent == this)
						{
							Device.BeginInvokeOnMainThread(async () =>
							{
								await Utils.ShowErrorMessage(string.IsNullOrEmpty(ex.Message) ? AppResources.SomethingWentWrong : ex.Message);
							});
						}
					}
					return new List<CreditCardDataItemViewModel>();
				});
			}
			catch (Exception)
			{
				if (!this.IsErrorPage && Utils.IReloadPageCurrent == this)
				{
					Device.BeginInvokeOnMainThread(async () =>
					{
						await Utils.ShowErrorMessage(AppResources.SomethingWentWrong);
					});
				}
			}
			if (Utils.IReloadPageCurrent == this)
			{
				Utils.IReloadPageCurrent = null;
			}
			if (!this.IsErrorPage)
			{
				LabelNoPaymentMethods.IsVisible = (PaymentMethodList == null || PaymentMethodList.Count == 0);
				lstView.IsVisible = true;
				this.IsBusy = false;
			}
		}
	}
}
