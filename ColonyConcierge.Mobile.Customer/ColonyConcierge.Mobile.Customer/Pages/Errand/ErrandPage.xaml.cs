using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ColonyConcierge.APIData.Data;
using ColonyConcierge.Mobile.Customer.Localization.Resx;
using Plugin.Toasts;
using Xamarin.Forms;
using XLabs.Forms.Controls;
using Xamarin.Forms.Xaml;

namespace ColonyConcierge.Mobile.Customer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ErrandPage : ContentPageBase
	{
		private List<Service> Services = new List<Service>();
		private ExtendedAddress mServiceAddress;
		private ExtendedAddress mBusinessAddress;
		private AddressFacade mAddressFacade = new AddressFacade();
		ColonyConcierge.APIData.Data.User UserModel;
		List<ColonyConcierge.APIData.Data.CreditCardData> mPaymentAccountDatas = new List<APIData.Data.CreditCardData>();
		ScheduledErrand mScheduledErrand = new ScheduledErrand();
		List<ServiceKindCodes> ServiceKinds = new List<ServiceKindCodes>();
		List<ScheduledErrand.ErrandType> ErrandTypes = new List<ScheduledErrand.ErrandType>();
		string timeFormat = "hh:mm tt";

		public ErrandPage(List<Service> services)
		{
			InitializeComponent();

			if (services != null)
			{
				Services = services;
				LoadRadioButton();
			}

			DateTime dateTime = new DateTime(1, 1, 1, 8, 0, 0);
			PickerStartTime.Items.Add(AppResources.AnyTime);
			PickerEndTime.Items.Add(AppResources.AnyTime);
			PickerStartTime.SelectedIndex = 0;
			PickerEndTime.SelectedIndex = 0;
			while (dateTime.Hour < 19)
			{
				PickerStartTime.Items.Add(dateTime.ToString(timeFormat));
				PickerEndTime.Items.Add(dateTime.ToString(timeFormat));
				dateTime = dateTime.AddHours(1);
			}

			DatePickerService.Date = DateTime.Now.Date;
			DatePickerService.MinimumDate = DateTime.Now.Date;
			DatePickerService.Format = "MM/dd/yyyy";

			mServiceAddress = mAddressFacade.GetUserAddress();
			if (mServiceAddress != null)
			{
				EntryApartmentUnit.Text = mServiceAddress.BasicAddress.Line2;
				EntryAddress.Text = mServiceAddress.BasicAddress.ToAddress();
			}
			else
			{
				EntryApartmentUnit.Text = string.Empty;
				EntryAddress.Text = string.Empty;
			}

			EntryAddress.AddressChanged += (sender, e) =>
			{
				mServiceAddress = e;
				if (mServiceAddress != null)
				{
					EntryAddress.Text = mServiceAddress.BasicAddress.ToAddress();
					EntryApartmentUnit.Text = mServiceAddress.BasicAddress.Line2;
				}
				else
				{
					EntryAddress.Text = string.Empty;
					EntryApartmentUnit.Text = string.Empty;
				}
				LoadData();
			};
			EntryApartmentUnit.TextChanged += (sender, e) =>
			{
				if (mServiceAddress != null)
				{
					mServiceAddress.BasicAddress.Line2 = EntryApartmentUnit.Text;
				}
			};

			EntryAddressBusiness.AddressChanged += (sender, e) =>
			{
				mBusinessAddress = e;
				if (mBusinessAddress != null)
				{
					EntryAddressBusiness.Text = mBusinessAddress.BasicAddress.ToAddress();
					EntryApartmentUnitBusiness.Text = mBusinessAddress.BasicAddress.Line2;
				}
				else
				{
					EntryAddressBusiness.Text = string.Empty;
					EntryApartmentUnitBusiness.Text = string.Empty;
				}
				ButtonCheckout.IsEnabled = CheckCheckout();
			};
			EntryApartmentUnitBusiness.TextChanged += (sender, e) =>
			{
				if (mBusinessAddress != null)
				{
					mBusinessAddress.BasicAddress.Line2 = EntryApartmentUnitBusiness.Text;
				}
			};

			EntryNameBusiness.TextChanged += (sender, e) =>
			{
				ButtonCheckout.IsEnabled = CheckCheckout();
			};

			LabelProvideComment.IsVisible = string.IsNullOrEmpty(EntryProvideComment.Text);
			EntryProvideComment.TextChanged += (sender, e) =>
			{
				LabelProvideComment.IsVisible = string.IsNullOrEmpty(EntryProvideComment.Text);
				ButtonCheckout.IsEnabled = CheckCheckout();
			};

			PickerStartTime.SelectedIndexChanged += (sender, e) =>
			{
				ButtonCheckout.IsEnabled = CheckCheckout();
				var dayErrand = CheckDayErrand();
				ButtonCheckout.IsEnabled = dayErrand;
				LabelDateAreIncorrect.IsVisible = !dayErrand;
			};

			PickerEndTime.SelectedIndexChanged += (sender, e) =>
			{
				ButtonCheckout.IsEnabled = CheckCheckout();
				var dayErrand = CheckDayErrand();
				ButtonCheckout.IsEnabled = dayErrand;
				LabelDateAreIncorrect.IsVisible = !dayErrand;
			};

			CheckBoxStartTime.CheckedChanged += (sender, e) =>
			{
				ButtonCheckout.IsEnabled = CheckCheckout();
				var dayErrand = CheckDayErrand();
				ButtonCheckout.IsEnabled = dayErrand;
				LabelDateAreIncorrect.IsVisible = !dayErrand;
			};

			CheckBoxEndTime.CheckedChanged += (sender, e) =>
			{
				ButtonCheckout.IsEnabled = CheckCheckout();
				var dayErrand = CheckDayErrand();
				ButtonCheckout.IsEnabled = dayErrand;
				LabelDateAreIncorrect.IsVisible = !dayErrand;
			};

			ButtonCheckout.Clicked += (sender, e) =>
			{
				UserFacade userFacade = new UserFacade();
				userFacade.RequireLogin(this, () =>
				{
					this.IsBusy = true;
					bool isSucceed = false;
					mScheduledErrand = new ScheduledErrand();
					Task.Run(() =>
					{
						try
						{
							int indexErrand = -1;
							for (int i = 0; i < StackLayoutErrandToPerform.Children.Count; i++)
							{
								if (StackLayoutErrandToPerform.Children[i] is CustomRadioButton
									&& (StackLayoutErrandToPerform.Children[i] as CustomRadioButton).Checked)
								{
									indexErrand = i;
									break;
								}
							}

							if (indexErrand < 0 || Services.Count <= indexErrand || ErrandTypes.Count <= indexErrand)
							{
								throw new Exception(AppResources.CanNotFoundService);
							}

							var serviceDate = new SimpleDate(DatePickerService.Date.Year, DatePickerService.Date.Month, DatePickerService.Date.Day);
							UserModel = Shared.APIs.IUsers.GetCurrentUser();
							mScheduledErrand.Status = "";
							mScheduledErrand.ServiceAddress = mServiceAddress;
							mScheduledErrand.ServiceID = Services[indexErrand].ID;
							mScheduledErrand.ServiceDate = serviceDate;
							mScheduledErrand.SpecialInstructions = EntryProvideComment.Text;

							mScheduledErrand.Type = ErrandTypes[indexErrand];
							mScheduledErrand.ErrandLocation = mBusinessAddress.BasicAddress;
							mScheduledErrand.ErrandLocationName = EntryNameBusiness.Text;
							mScheduledErrand.Dropoff = CheckBoxStartTime.Checked;
							mScheduledErrand.Pickup = CheckBoxEndTime.Checked;

							if (PickerStartTime.SelectedIndex > 0)
							{
								var date = DatePickerService.Date;
								var time = DateTime.ParseExact(PickerStartTime.Items[PickerStartTime.SelectedIndex], timeFormat, null);
								mScheduledErrand.DropoffTime = date.Add(time.TimeOfDay).ToUniversalTime();
							}

							if (PickerEndTime.SelectedIndex > 0)
							{
								var date = DatePickerService.Date;
								var time = DateTime.ParseExact(PickerEndTime.Items[PickerEndTime.SelectedIndex], timeFormat, null);
								mScheduledErrand.PickupTime = date.Add(time.TimeOfDay).ToUniversalTime();
							}

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

		public void LoadRadioButton()
		{
			StackLayoutErrandToPerform.Children.Clear();
			ServiceKinds = Services.Select(t => t.ServiceKind).ToList();
			foreach (var serviceKind in ServiceKinds)
			{
				RadioButtonCustom customRadioButton = new RadioButtonCustom();
				if (serviceKind == ServiceKindCodes.Errand_CarServices)
				{
					customRadioButton.Text = AppResources.CarServices;
					ErrandTypes.Add(ScheduledErrand.ErrandType.CarDelivery);
				}
				else if (serviceKind == ServiceKindCodes.Errand_DryCleaning)
				{
					customRadioButton.Text = AppResources.DryCleaning;
					ErrandTypes.Add(ScheduledErrand.ErrandType.DryCleaning);
				}
				else if (serviceKind == ServiceKindCodes.Errand_PetServices)
				{
					customRadioButton.Text = AppResources.PetServices;
					ErrandTypes.Add(ScheduledErrand.ErrandType.PetServices);
				}
				else if (serviceKind == ServiceKindCodes.Errand_Other)
				{
					customRadioButton.Text = AppResources.OtherErrand;
					ErrandTypes.Add(ScheduledErrand.ErrandType.Other);
				}
				else 
				{
					customRadioButton.Text = serviceKind.ToString().Replace("Errand_", "");
					ErrandTypes.Add(ScheduledErrand.ErrandType.Other);
				}
				customRadioButton.TextColor = Color.Black;
				customRadioButton.CheckedChanged += (sender, e) =>
				{
					if (customRadioButton.Checked)
					{
						foreach (CustomRadioButton item in StackLayoutErrandToPerform.Children)
						{
							if (item != customRadioButton)
							{
								item.Checked = false;
							}
						}
						ButtonCheckout.IsEnabled = CheckCheckout();
					}
				};
				StackLayoutErrandToPerform.Children.Add(customRadioButton);
			}
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
				Services = Services
							.Where(s => s.ServiceType == APIData.Data.ServiceTypes.Errands)
						    .ToList();
				LoadRadioButton();

				ButtonCheckout.IsEnabled = CheckCheckout();
				this.IsBusy = false;
			}, TaskScheduler.FromCurrentSynchronizationContext());
		}


		public async Task<SelectPaymentMethod> GoSelectPaymentMethod()
		{
			var shoppingSelectPaymentMethod = new SelectPaymentMethod(Services.FirstOrDefault(), mScheduledErrand);
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

		public bool CheckDayErrand()
		{
			if (PickerStartTime.SelectedIndex > 0 && PickerEndTime.SelectedIndex > 0
				&& CheckBoxStartTime.Checked && CheckBoxEndTime.Checked)
			{
				var startTime = DateTime.ParseExact(PickerStartTime.Items[PickerStartTime.SelectedIndex], timeFormat, null);
				var endTime = DateTime.ParseExact(PickerEndTime.Items[PickerEndTime.SelectedIndex], timeFormat, null);
				if (startTime > endTime)
				{
					return false;
				}
			}
			return true;
		}

		public bool CheckCheckout()
		{
			bool errandChecked = false;
			foreach (CustomRadioButton item in StackLayoutErrandToPerform.Children)
			{
				errandChecked = errandChecked || item.Checked;
			}
			if (!errandChecked)
			{
				return false;
			}
			if (mBusinessAddress == null)
			{
				return false;
			}
			if (string.IsNullOrEmpty(EntryNameBusiness.Text))
			{
				return false;
			}
			if (string.IsNullOrEmpty(EntryAddress.Text))
			{
				return false;
			}

			return true;
		}
	}
}
