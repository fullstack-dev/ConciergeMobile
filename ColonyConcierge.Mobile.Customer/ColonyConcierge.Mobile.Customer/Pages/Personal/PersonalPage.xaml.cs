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
    public partial class PersonalPage : ContentPageBase
	{
		private List<Service> Services = new List<Service>();
		private ExtendedAddress mServiceAddress;
		private AddressFacade mAddressFacade = new AddressFacade();
		ColonyConcierge.APIData.Data.User UserModel;
		List<ColonyConcierge.APIData.Data.CreditCardData> mPaymentAccountDatas = new List<APIData.Data.CreditCardData>();
		ScheduledPersonalService mScheduledSpecialRequest = new ScheduledPersonalService();
		List<CustomRadioButton> CustomRadioButtonsWaiting = new List<CustomRadioButton>();
		List<CustomRadioButton> CustomRadioButtonsLaundry = new List<CustomRadioButton>();
		List<CheckBox> CheckBoxsHouseSitting = new List<CheckBox>();
		ServiceKindCodes mServiceKindCodes;
		ScheduledPersonalService.PersonalServiceType mPersonalServiceType;

		List<string> HouseSittings = new List<string>
			{
				"WATER PLANTS",
				"FEED PETS",
				"CHECK MAIL",
				"NEWSPAPER",
				"TURN ON LIGHTS",
				"OTHER",
			};
		List<string> Laundries = new List<string>
			{
				"Tide w/ Bleach",
				"All Free",
				"Provide My Own",
			};
		List<string> Waitings = new List<string>
			{
				"CABLE/SATELLITE",
				"PHONE",
				"REPAIRMAN",
				"OTHER",
			};

		public PersonalPage(ServiceKindCodes serviceKindCodes)
		{
			InitializeComponent();

			mServiceKindCodes = serviceKindCodes;
			switch (serviceKindCodes)
			{
				case ServiceKindCodes.PersonalService_Waiting:
					StackLayoutWaiting.IsVisible = true;
					mPersonalServiceType = ScheduledPersonalService.PersonalServiceType.Waiting;
					break;
				case ServiceKindCodes.PersonalService_SnowBird:
					mPersonalServiceType = ScheduledPersonalService.PersonalServiceType.SnowBird;
					break;
				case ServiceKindCodes.PersonalService_Laundry:
					StackLayoutLaundryService.IsVisible = true;
					mPersonalServiceType = ScheduledPersonalService.PersonalServiceType.Laundry;
					break;
				case ServiceKindCodes.PersonalService_Handyman:
					mPersonalServiceType = ScheduledPersonalService.PersonalServiceType.HandyMan;
					break;
				case ServiceKindCodes.PersonalService_HouseSitting:
					StackLayoutHouseSitting.IsVisible = true;
					mPersonalServiceType = ScheduledPersonalService.PersonalServiceType.HouseSitting;
					break;
			}

			Title = AppResources.PersonalService + "/" + mPersonalServiceType;

			CustomRadioButtonsWaiting = new List<CustomRadioButton>();
			foreach (var waiting in Waitings)
			{
				RadioButtonCustom customRadioButton = new RadioButtonCustom();
				customRadioButton.FontName = AppearanceBase.Instance.FontFileNameDefault;
				customRadioButton.Text = waiting;
				customRadioButton.TextColor = Color.Black;
				customRadioButton.CheckedChanged += (sender, e) =>
				{
					if (customRadioButton.Checked)
					{
						foreach (var item in CustomRadioButtonsWaiting)
						{
							if (item != customRadioButton)
							{
								item.Checked = false;
							}
						}
						ButtonCheckout.IsEnabled = CheckCheckout();
					}
				};
				CustomRadioButtonsWaiting.Add(customRadioButton);
				StackLayoutWaiting.Children.Add(customRadioButton);
			}
			//if (CustomRadioButtonsWaiting.FirstOrDefault() != null)
			//{
			//	CustomRadioButtonsWaiting.FirstOrDefault().Checked = true;
			//}

			CustomRadioButtonsLaundry = new List<CustomRadioButton>();
			foreach (var laundry in Laundries)
			{
				CustomRadioButton customRadioButton = new RadioButtonCustom();
				customRadioButton.FontName = AppearanceBase.Instance.FontFamilyDefault;
				customRadioButton.Text = laundry;
				customRadioButton.TextColor = Color.Black;
				customRadioButton.CheckedChanged += (sender, e) =>
				{
					if (customRadioButton.Checked)
					{
						foreach (var item in CustomRadioButtonsLaundry)
						{
							if (item != customRadioButton)
							{
								item.Checked = false;
							}
						}
						ButtonCheckout.IsEnabled = CheckCheckout();
					}
				};
				CustomRadioButtonsLaundry.Add(customRadioButton);
				StackLayoutLaundryService.Children.Add(customRadioButton);
			}

			CheckBoxsHouseSitting = new List<CheckBox>();
			foreach (var houseSitting in HouseSittings)
			{
				CheckBoxCustom checkBox = new CheckBoxCustom();
				checkBox.FontName = AppearanceBase.Instance.FontFamilyDefault;
				checkBox.DefaultText = houseSitting;
				checkBox.TextColor = Color.Black;
				checkBox.CheckedChanged += (sender, e) =>
				{
					ButtonCheckout.IsEnabled = CheckCheckout();
				};
				CheckBoxsHouseSitting.Add(checkBox);
				StackLayoutHouseSitting.Children.Add(checkBox);
			}

			DatePickerService.Date = DateTime.Now.Date.AddDays(1);
			DatePickerService.MinimumDate = DateTime.Now.Date.AddDays(1);
			DatePickerService.Format = "MM/dd/yyyy";
			DatePickerService.DateSelected += (sender, e) =>
			{
				ButtonCheckout.IsEnabled = CheckCheckout();
			};

			GridDatePickerEndDate.IsVisible = mPersonalServiceType == ScheduledPersonalService.PersonalServiceType.HouseSitting;
			DatePickerEndDate.Date = DateTime.Now.Date.AddDays(3);
			DatePickerEndDate.MinimumDate = DateTime.Now.Date.AddDays(1);
			DatePickerEndDate.Format = "MM/dd/yyyy";
			DatePickerEndDate.DateSelected += (sender, e) =>
			{
				ButtonCheckout.IsEnabled = CheckCheckout();
			};

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
					mScheduledSpecialRequest = new ScheduledPersonalService();
					Task.Run(() =>
					{
						Utils.IReloadPageCurrent = this;
						try
						{
							if (Services.FirstOrDefault() == null)
							{
								throw new Exception(AppResources.CanNotFoundService);
							}

							var serviceDate = new SimpleDate(DatePickerService.Date.Year, DatePickerService.Date.Month, DatePickerService.Date.Day);
							var serviceEndDate = new SimpleDate(DatePickerEndDate.Date.Year, DatePickerEndDate.Date.Month, DatePickerEndDate.Date.Day);

							UserModel = Shared.APIs.IUsers.GetCurrentUser();
							mScheduledSpecialRequest.Status = "";
							mScheduledSpecialRequest.ServiceAddress = mServiceAddress;
							mScheduledSpecialRequest.ServiceID = Services.FirstOrDefault().ID;
							mScheduledSpecialRequest.ServiceDate = serviceDate;
							mScheduledSpecialRequest.Type = mPersonalServiceType;
							mScheduledSpecialRequest.EndDate = serviceEndDate;
							mScheduledSpecialRequest.SpecialInstructions = EntryProvideComment.Text;
							//mPaymentAccountDatas = Shared.APIs.IAccounts.GetPaymentMethods(UserModel.AccountID);
							mPaymentAccountDatas = Shared.APIs.IAccounts.BtGetPaymentMethods(UserModel.AccountID);

							switch (mPersonalServiceType)
							{
								case APIData.Data.ScheduledPersonalService.PersonalServiceType.Waiting:
									foreach (var customRadioButton in CustomRadioButtonsWaiting)
									{
										if (customRadioButton.Checked)
										{
											mScheduledSpecialRequest.WaitingFor = customRadioButton.Text;
										}
									}
									break;
								case APIData.Data.ScheduledPersonalService.PersonalServiceType.HouseSitting:
									mScheduledSpecialRequest.WaterPlants = CheckBoxsHouseSitting[0].Checked;
									mScheduledSpecialRequest.FeedPets = CheckBoxsHouseSitting[1].Checked;
									mScheduledSpecialRequest.CheckMail = CheckBoxsHouseSitting[2].Checked;
									mScheduledSpecialRequest.NewsPaper = CheckBoxsHouseSitting[3].Checked;
									mScheduledSpecialRequest.TurnOnLights = CheckBoxsHouseSitting[4].Checked;
									mScheduledSpecialRequest.OtherHouseSitting = CheckBoxsHouseSitting[5].Checked;
									break;
								case APIData.Data.ScheduledPersonalService.PersonalServiceType.HandyMan:
									break;
								case APIData.Data.ScheduledPersonalService.PersonalServiceType.Laundry:
									foreach (var customRadioButton in CustomRadioButtonsLaundry)
									{
										if (customRadioButton.Checked)
										{
											mScheduledSpecialRequest.DetergentType = customRadioButton.Text;
										}
									}
									break;
								case APIData.Data.ScheduledPersonalService.PersonalServiceType.SnowBird:
									break;
							}


							isSucceed = true;
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
				Services = Services
					.Where(s => s.ServiceType == APIData.Data.ServiceTypes.PersonalServices)
					.Where(s => s.ServiceKind == mServiceKindCodes)
	 			 	.ToList();

				ButtonCheckout.IsEnabled = CheckCheckout();
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
			if (mPersonalServiceType == ScheduledPersonalService.PersonalServiceType.HouseSitting
			    && CheckBoxsHouseSitting.Count(t => t.Checked) ==0)
			{
				return false;
			}
			if (mPersonalServiceType == ScheduledPersonalService.PersonalServiceType.Waiting
			    && CustomRadioButtonsWaiting.Count(t => t.Checked) == 0)
			{
				return false;
			}
			if (mPersonalServiceType == ScheduledPersonalService.PersonalServiceType.Laundry
			    && CustomRadioButtonsLaundry.Count(t => t.Checked) == 0)
			{
				return false;
			}
			if (string.IsNullOrEmpty(EntryProvideComment.Text))
			{
				return false;
			}
			if (string.IsNullOrEmpty(addressEntry.Text))
			{
				return false;
			}
			if (GridDatePickerEndDate.IsVisible && DatePickerEndDate.Date < DatePickerService.Date)
			{
				return false;
			}
			return true;
		}
	}
}
