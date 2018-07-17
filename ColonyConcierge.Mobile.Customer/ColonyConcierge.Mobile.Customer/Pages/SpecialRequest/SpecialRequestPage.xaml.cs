using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ColonyConcierge.APIData.Data;
using ColonyConcierge.Mobile.Customer.Localization.Resx;
using Plugin.Toasts;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ColonyConcierge.Mobile.Customer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SpecialRequestPage : ContentPageBase
	{
		private List<Service> Services = new List<Service>();
		private ExtendedAddress mServiceAddress;
		private AddressFacade mAddressFacade = new AddressFacade();
		ColonyConcierge.APIData.Data.User UserModel;
		List<ColonyConcierge.APIData.Data.CreditCardData> mPaymentAccountDatas = new List<APIData.Data.CreditCardData>();
		ScheduledSpecialRequest mScheduledSpecialRequest = new ScheduledSpecialRequest();

		public SpecialRequestPage()
		{
			InitializeComponent();

			DatePickerService.Date = DateTime.Now.Date;
			DatePickerService.MinimumDate = DateTime.Now.Date;
			DatePickerService.Format = "MM/dd/yyyy";

			mServiceAddress = mAddressFacade.GetUserAddress();
			if (mServiceAddress != null)
			{
				apartmentUnitEntry.Text = mServiceAddress.BasicAddress.Line2;
				addressEntry.Text = mServiceAddress.BasicAddress.ToAddress();
			}
			else
			{
				apartmentUnitEntry.Text = string.Empty;
				addressEntry.Text = string.Empty;
			}

			addressEntry.AddressChanged += (sender, e) =>
			{
				mServiceAddress = e;
				if (mServiceAddress != null)
				{
					addressEntry.Text = mServiceAddress.BasicAddress.ToAddress();
					apartmentUnitEntry.Text = mServiceAddress.BasicAddress.Line2;
				}
				else
				{
					addressEntry.Text = string.Empty;
					apartmentUnitEntry.Text = string.Empty;
				}
				LoadData();
			};
			apartmentUnitEntry.TextChanged += (sender, e) =>
			{
				if (mServiceAddress != null)
				{
					mServiceAddress.BasicAddress.Line2 = apartmentUnitEntry.Text;
				}
			};

			LabelProvideComment.IsVisible = string.IsNullOrEmpty(EntryProvideComment.Text);
			EntryProvideComment.TextChanged += (sender, e) =>
			{
				LabelProvideComment.IsVisible = string.IsNullOrEmpty(EntryProvideComment.Text);
				ButtonCheckout.IsEnabled = CheckCheckout();
			};

			ButtonCheckout.Clicked += (sender, e) =>
			{
				UserFacade userFacade = new UserFacade();
				userFacade.RequireLogin(this, () =>
				{
					this.IsBusy = true;
					bool isSucceed = false;
					mScheduledSpecialRequest = new ScheduledSpecialRequest();
					Task.Run(() =>
					{
						try
						{
							if (Services.FirstOrDefault() == null)
							{
								throw new Exception(AppResources.CanNotFoundService);
							}

							var serviceDate = new SimpleDate(DatePickerService.Date.Year, DatePickerService.Date.Month, DatePickerService.Date.Day);
							UserModel = Shared.APIs.IUsers.GetCurrentUser();
							mScheduledSpecialRequest.Status = "";
							mScheduledSpecialRequest.ServiceAddress = mServiceAddress;
							mScheduledSpecialRequest.ServiceID = Services.FirstOrDefault().ID;
							mScheduledSpecialRequest.ServiceDate = serviceDate;
							mScheduledSpecialRequest.SpecialInstructions = EntryProvideComment.Text;
							//mScheduledSpecialRequest.DetailedInstructions = EntryProvideComment.Text;
							//mPaymentAccountDatas = Shared.APIs.IAccounts.GetPaymentMethods(UserModel.AccountID);
							mPaymentAccountDatas = Shared.APIs.IAccounts.BtGetPaymentMethods(UserModel.AccountID);

							isSucceed = true;
						}
						catch (Exception ex)
						{
							Device.BeginInvokeOnMainThread(async () =>
							{
								var message = string.IsNullOrEmpty(ex.Message) ? AppResources.SomethingWentWrong : ex.Message;
								await Utils.ShowErrorMessage(message, 5);
							});
						}
					}).ContinueWith(async t =>
					{
						if (isSucceed)
						{
							if (mPaymentAccountDatas != null && mPaymentAccountDatas.Count > 0)
							{
								await GoSelectPaymentMethod();
							}
							else
							{
								var paymentMethodUpdatePage = new BrainTreePaymentMethodPage(true, (page, cardId) =>
								{
									Device.BeginInvokeOnMainThread(async () =>
									{
										var selectPaymentMethod = await GoSelectPaymentMethod();
										selectPaymentMethod.Navigation.RemovePage(page);
									});

									return true;
								});
								await Utils.PushAsync(Navigation, paymentMethodUpdatePage, true);
							}
						}
						this.IsBusy = false;
					}, TaskScheduler.FromCurrentSynchronizationContext());
				});
			};

			ButtonCheckout.IsEnabled = CheckCheckout();

			LoadData();
		}

		public override void ReloadPage()
		{
			base.ReloadPage();
			LoadData();
		}

		public void LoadData()
		{
			this.IsBusy = true;

			Task.Run(() =>
			{
				Utils.IReloadPageCurrent = this;
				try
				{
					var zipCode = mServiceAddress.BasicAddress.ZipCode;
					if (Shared.IsLoggedIn)
					{
						Services = Shared.APIs.IServices.GetAvailableServicesForUser(Shared.UserId, null, null, zipCode);
					}
					else
					{
						Services = Shared.APIs.IServices.GetAvailableServices(zipCode);
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
			}).ContinueWith(t =>
			{
				if (Services != null)
				{
					Services = Services
						.Where(s => s.ServiceType == APIData.Data.ServiceTypes.SpecialRequests)
						.Where(s => s.ServiceKind == ServiceKindCodes.SpecialRequest)
						  .ToList();

					ButtonCheckout.IsEnabled = CheckCheckout();
				}
				this.IsBusy = false;
			}, TaskScheduler.FromCurrentSynchronizationContext());
		}


		public async Task<SelectPaymentMethod> GoSelectPaymentMethod()
		{
			var shoppingSelectPaymentMethod = new SelectPaymentMethod(Services.FirstOrDefault(), mScheduledSpecialRequest);
			shoppingSelectPaymentMethod.Done += (sender2, id) =>
			{
				Page lastPage = sender2 as Page;
				var pages = lastPage.Navigation.NavigationStack.Reverse().Skip(1).ToList();
				foreach (var page in pages)
				{
					if (this != page)
					{
						lastPage.Navigation.RemovePage(page);
					}
					else
					{
						lastPage.Navigation.RemovePage(page);
						break;
					}
				}
				lastPage.Navigation.PopAsync(true).ConfigureAwait(false);
			};
			await Utils.PushAsync(Navigation, shoppingSelectPaymentMethod, true);
			return shoppingSelectPaymentMethod;
		}

		public bool CheckCheckout()
		{
			if (string.IsNullOrEmpty(EntryProvideComment.Text))
			{
				return false;
			}
			if (string.IsNullOrEmpty(addressEntry.Text))
			{
				return false;
			}
			return true;
		}
	}
}
