using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ColonyConcierge.Mobile.Customer.Localization.Resx;
using Plugin.Toasts;
using Xamarin.Forms;
using System.Linq;
using ColonyConcierge.APIData.Data;
using Xamarin.Forms.Xaml;

namespace ColonyConcierge.Mobile.Customer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SelectPaymentMethod : ContentPageBase
	{
		List<ColonyConcierge.APIData.Data.CreditCardData> PaymentAccountDatas = new List<APIData.Data.CreditCardData>();
		ColonyConcierge.APIData.Data.User UserModel;
		CreditCardData CardSelected = null;
		public event EventHandler<int> Done;
		private IAppServices mAppServices;
		private int mDefaultCardId = -1;

		public SelectPaymentMethod(Service service, ScheduledService scheduledService)
		{
			InitializeComponent();

			NavigationPage.SetBackButtonTitle(this, AppResources.Back);

			mAppServices = DependencyService.Get<IAppServices>();


			//PickerPayment.SelectedIndexChanged += (sender, e) =>
			//{
			//	if (PickerPayment.SelectedIndex >= 0 && PickerPayment.Items[PickerPayment.SelectedIndex] == AppResources.AddNewCreditCard)
			//	{
			//		var paymentMethodUpdatePage = new BrainTreePaymentMethodPage(true, (page) =>
			//		{
			//			Device.BeginInvokeOnMainThread(async () =>
			//			{
			//				await page.Navigation.PopAsync().ConfigureAwait(false);
			//				LoadCard();
			//			});
			//			return true;
			//		});
			//		Utils.PushAsync(Navigation, paymentMethodUpdatePage, true);
			//		PickerPayment.SelectedIndex = PickerPaymentSelected;
			//	}
			//	else
			//	{
			//		PickerPaymentSelected = PickerPayment.SelectedIndex;
			//	}
			//};

			StackLayoutChangeCard.GestureRecognizers.Add(new TapGestureRecognizer
			{
				Command = new Command(async () =>
				{
					SelectCardPage selectCardPage = new SelectCardPage(CardSelected);
					selectCardPage.CardSelected += (sender2, e2) =>
					{
						CardSelected = e2.Model;
						UpdateCard();
						var pages = Navigation.NavigationStack.Reverse().Skip(1).ToList();
						foreach (var page in pages)
						{
							if (page != this)
							{
								Navigation.RemovePage(page);
							}
							else
							{
								break;
							}
						}
						if (pages.Count > 0)
						{
							Navigation.PopAsync(true).ConfigureAwait(false);
						}
					};
					await Utils.PushAsync(Navigation, selectCardPage, true);
				}),
			});

			//EntryCard.CardChanged += (sender, e) =>
			//{
			//	if (e != null)
			//	{
			//		CachedImageCard.Source = e.creditTypeImg;
			//		CachedImageCard.IsVisible = true;
			//	}
			//	else
			//	{
			//		CachedImageCard.IsVisible = false;
			//	}
			//};

			ButtonCheckout.Clicked += (sender, e) =>
			{
				if (CardSelected != null)
				{
					this.IsBusy = true;
					int id = 0;
					this.ButtonCheckout.Text = AppResources.PlacingOrder;
					Task.Run(() =>
					{
						try
						{
							if (scheduledService is ScheduledShopping)
							{
								var scheduledShopping = scheduledService as ScheduledShopping;
								if (scheduledShopping.ShoppingList.Browseable)
								{
									Shared.APIs.IUsers.AddShoppingList(Shared.UserId, scheduledShopping.ShoppingList);
								}
							}

							var paymentAccountData = CardSelected;
							//CreditCardData creditCardData = Shared.APIs.IAccounts.BtGetPaymentMethods(UserModel.AccountID).FirstOrDefault(t => t.ID == paymentAccountData.ID);
							//if (!creditCardData.IsPreferred)
							//{
							//	creditCardData.ID = paymentAccountData.ID;
							//	creditCardData.IsPreferred = true;
							//	bool result = Shared.APIs.IAccounts.BtUpdatePaymentMethod(UserModel.AccountID, creditCardData.ID, creditCardData);
							//	if (!result)
							//	{
							//		throw new Exception(AppResources.CanNotUseThisCard);
							//	}
							//}
							scheduledService.PaymentMethodID = paymentAccountData.ID;

							id = Shared.APIs.IUsers.AddScheduledService(Shared.UserId, scheduledService);
						}
						catch (Exception ex)
						{
							Device.BeginInvokeOnMainThread(async () =>
							{
								var message = string.IsNullOrEmpty(ex.Message) ? AppResources.SomethingWentWrong : ex.Message;
								await Utils.ShowErrorMessage(message, 5);
							});
						}
					}).ContinueWith(t =>
					{
						if (id > 0)
						{
							if (service != null)
							{
								mAppServices.TrackOrder(id.ToString(),
														service.Name);
							}
							var notificator = DependencyService.Get<IToastNotificator>();
							notificator.Notify(ToastNotificationType.Success, AppResources.PlaceOrder, AppResources.YourOrderSuccessfully, TimeSpan.FromSeconds(2));

							var pages = Navigation.NavigationStack.Reverse().Skip(1).ToList();
							IServicesTabPage servicesTabPage = null;
							foreach (var page in pages)
							{
								if (page is IServicesTabPage)
								{
									servicesTabPage = page as IServicesTabPage;
									break;
								}
								else
								{
									Navigation.RemovePage(page);
								}
							}
							if (servicesTabPage != null)
							{
								servicesTabPage.SelectScheduleTab(true);
							}
							Navigation.PopAsync(true).ConfigureAwait(false);

							//if (Done != null)
							//{
							//	this.Done(this, id);
							//}
						}
						this.ButtonCheckout.Text = AppResources.PlaceOrder;
						IsBusy = false;
					}, TaskScheduler.FromCurrentSynchronizationContext());
				}
				else 
				{
					Device.BeginInvokeOnMainThread(async () =>
					{
						var notificator = DependencyService.Get<IToastNotificator>();
						await notificator.Notify(ToastNotificationType.Error, AppResources.Error, AppResources.PleaseSelectCard, TimeSpan.FromSeconds(2));
					});
				}
			};

			LoadCard();
		}

		public override void ReloadPage()
		{
			base.ReloadPage();

			LoadCard(mDefaultCardId);
		}

		private void LoadCard(int cardId = -1)
		{
			UserFacade userFacade = new UserFacade();
			userFacade.RequireLogin(this, () =>
			{
				this.IsBusy = true;
				Task.Run(() =>
				{
					Utils.IReloadPageCurrent = this;
					try
					{
						UserModel = Shared.APIs.IUsers.GetCurrentUser();
						if (UserModel != null)
						{
							PaymentAccountDatas = Shared.APIs.IAccounts.BtGetPaymentMethods(UserModel.AccountID);
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
					if (Utils.IReloadPageCurrent == this)
					{
						Utils.IReloadPageCurrent = null;
					}
				}).ContinueWith((t) =>
				{
					if (PaymentAccountDatas != null)                  
					{
						var preferredIndex = PaymentAccountDatas.FindIndex(s => s.ID == cardId);
						if (preferredIndex < 0)
						{
							preferredIndex = PaymentAccountDatas.FindIndex(s => s.IsPreferred);
						}
						if (PaymentAccountDatas.Count > 0)
						{
							ButtonCheckout.IsEnabled = true;
							preferredIndex = preferredIndex > 0 ? preferredIndex : 0;
							CardSelected = PaymentAccountDatas[preferredIndex];
							UpdateCard();
						}
						else
						{
							ButtonCheckout.IsEnabled = false;
							var paymentMethodUpdatePage = new BrainTreePaymentMethodPage(true, (page, cardResult) =>
							{
								Device.BeginInvokeOnMainThread(async () =>
								{
									await page.Navigation.PopAsync().ConfigureAwait(false);
									mDefaultCardId = cardResult;
									LoadCard(mDefaultCardId);
								});

								return true;
							});
							Utils.PushAsync(Navigation, paymentMethodUpdatePage, true);
						}
					}

					this.IsBusy = false;
				}, TaskScheduler.FromCurrentSynchronizationContext());
			});
		}

		public void UpdateCard()
		{
			if (CardSelected != null)
			{
				CachedImageCard.Source = CardSelected.creditTypeImg;
				var number = string.Empty;
				var index = CardSelected.CreditCardNumber.IndexOf("*", StringComparison.OrdinalIgnoreCase);
				if (index < CardSelected.CreditCardNumber.Length - 1)
				{
					number = CardSelected.CreditCardNumber.Substring(index + 1);
				}
				LabelCardNumber.Text = number;
				LabelCardName.Text = CardSelected.AccountNickname;
				CachedImageCard.IsVisible = true;
				ButtonCheckout.IsEnabled = true;
			}
			else
			{
				LabelCardNumber.Text = string.Empty;
				LabelCardName.Text = string.Empty;
				CachedImageCard.IsVisible = false;
			}
		}
	}
}
