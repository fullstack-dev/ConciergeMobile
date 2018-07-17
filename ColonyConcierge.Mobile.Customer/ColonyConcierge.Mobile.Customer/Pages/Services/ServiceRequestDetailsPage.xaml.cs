using ColonyConcierge.APIData.Data;
using ColonyConcierge.Mobile.Customer.Localization.Resx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using System.Collections.ObjectModel;
using Plugin.Toasts;
using XLabs.Forms.Controls;
using Xamarin.Forms.Xaml;

namespace ColonyConcierge.Mobile.Customer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ServiceRequestDetailsPage : ContentPageBase
    {
		private bool IsEdit = false;
		private int mScheduledServiceId;

		public Action Back
		{
			get;
			set;
		}

		private List<ColonyConcierge.APIData.Data.ShoppingStore> mShoppingStores = new List<APIData.Data.ShoppingStore>();
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
		List<CustomRadioButton> CustomRadioButtonsWaiting = new List<CustomRadioButton>();
		List<CustomRadioButton> CustomRadioButtonsLaundry = new List<CustomRadioButton>();
		List<CheckBox> CheckBoxsHouseSitting = new List<CheckBox>();
		string timeFormat = "hh:mm tt";

        private ScheduledService mScheduledService;
        public ScheduledService ScheduledService
        {
            set
            {
                this.OnPropertyChanging(nameof(ScheduledService));
                mScheduledService = value;
                this.OnPropertyChanged(nameof(ScheduledService));
            }
            get
            {
                return mScheduledService;
            }
        }

		ObservableCollection<ShoppingListItemViewModel> ShoppingListItemViewModels = new ObservableCollection<ShoppingListItemViewModel>();

		public ServiceRequestDetailsPage(int scheduledServiceId)
		{
            InitializeComponent();

			LoadScheduledService(scheduledServiceId);
		}

        public ServiceRequestDetailsPage(ScheduledService scheduledService, string title)
        {
            try
            {
                InitializeComponent();

                this.Title = title;
                LoadScheduledService(scheduledService);
            }
            catch (Exception ex)
            {
                Utils.ShowErrorMessage(ex);
            }
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

			if (Navigation != null && Navigation.NavigationStack != null)
			{
				var pages = Navigation.NavigationStack.Reverse().Skip(1).ToList();
				foreach (var page in pages)
				{
					if (page is ServiceRequestDetailsPage)
					{
						Navigation.RemovePage(page);
					}
					else
					{
						break;
					}
				}
			}
		}

		public void ShowBack(CustomNavigationPage customNavigationPage, Page backPage = null)
		{
			if (backPage == null)
			{
				backPage = new Page();
			}
			this.Navigation.InsertPageBefore(backPage, this);
			customNavigationPage.PopView = (page) =>
			{
				if (page == this)
				{
					Back.Invoke();
					return true;
				}
				return false;
			};
		}
		public void LoadScheduledService(int scheduledServiceId)
		{
			mScheduledServiceId = scheduledServiceId;
			this.IsBusy = true;
			this.Title = string.Empty;
			GridContent.IsVisible = false;
			ScheduledService scheduledService = null;
			Task.Run(() =>
			{
				Utils.IReloadPageCurrent = this;
				try
				{
					scheduledService = Shared.APIs.IUsers.GetScheduledService(Shared.UserId, scheduledServiceId);
				}
				catch (Exception ex)
				{
					if (!this.IsErrorPage && Utils.IReloadPageCurrent == this)
					{
						Device.BeginInvokeOnMainThread(() =>
						{
							Utils.ShowErrorMessage(new CustomException(ex.Message, ex));
						});
					}
				}
				if (Utils.IReloadPageCurrent == this)
				{
					Utils.IReloadPageCurrent = null;
				}
			}).ContinueWith((obj) =>
			{
				this.IsBusy = false;
				if (scheduledService != null)
				{
					GridContent.IsVisible = true;
					this.Title = scheduledService.Name;
					LoadScheduledService(scheduledService);
				}
			}, TaskScheduler.FromCurrentSynchronizationContext());
		}

		public void LoadScheduledService(ScheduledService scheduledService)
		{
			if (scheduledService != null)
			{
				ScheduledService = scheduledService;
				mScheduledServiceId = scheduledService.ID;
				bool isEnabled = scheduledService.Status.ToLower() == "entered";

				ButtonUpdateShopping.Clicked += (sender, e) =>
				{
					this.IsBusy = true;
					Task.Run(() =>
					{
						try
						{
							Shared.APIs.IUsers.UpdateScheduledService(Shared.UserId, scheduledService.ID, scheduledService);
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
						var pages = Navigation.NavigationStack.Reverse().ToList();
						IServicesTabPage servicesTabPage = null;
						foreach (var page in pages)
						{
							if (page is IServicesTabPage)
							{
								servicesTabPage = page as IServicesTabPage;
								break;
							}
						}
						if (servicesTabPage != null)
						{
							servicesTabPage.SelectScheduleTab(true);
						}
						Navigation.PopAsync().ConfigureAwait(false);
						this.IsBusy = false;
					}, TaskScheduler.FromCurrentSynchronizationContext());
				};


				if (ScheduledService is ScheduledShopping)
				{
					var scheduledShopping = ScheduledService as ScheduledShopping;
					this.IsBusy = true;

					this.GridShopping.IsVisible = true;

					DatePickerServiceShopping.MinimumDate = DateTime.Now.Date;
					DatePickerServiceShopping.Format = "MM/dd/yyyy";
					DatePickerServiceShopping.Date = new DateTime(scheduledShopping.ServiceDate.Year,
										  scheduledShopping.ServiceDate.Month, scheduledShopping.ServiceDate.Day);
					DatePickerServiceShopping.DateSelected += (sender, e) =>
					{
						var serviceDate = new SimpleDate(e.NewDate.Year, e.NewDate.Month, e.NewDate.Day);
						scheduledShopping.ServiceDate = serviceDate;
					};

					ShoppingListItemViewModels.Clear();
					if (scheduledShopping.ShoppingList.Items != null)
					{
						foreach (var shoppingListItem in scheduledShopping.ShoppingList.Items)
						{
							ShoppingListItemViewModels.Add(new ShoppingListItemViewModel(shoppingListItem, (obj) =>
							{
								ShoppingListItemViewModels.Remove(obj);
							}));
						}
					}

					LabelAddItemsShopping.GestureRecognizers.Add(new TapGestureRecognizer()
					{
						Command = new Command(async () =>
						{
							if (!this.GridUpdateShopping.IsEnabled || this.IsBusy)
							{
								return;
							}
							LabelAddItemsShopping.TextColor = AppearanceBase.Instance.OrangeColor;

							var addItemShoppingPage = new AddOrEditItemShoppingPage();
							addItemShoppingPage.Done += (sender, e) =>
							{
								var shoppingListItemViewModel = ShoppingListItemViewModels.FirstOrDefault((arg) =>
								{
									return arg.Model.Product.Brand == e.Product.Brand
											  && arg.Model.Product.Name == e.Product.Name;
								});
								if (shoppingListItemViewModel == null)
								{
									shoppingListItemViewModel = new ShoppingListItemViewModel(e, (obj) =>
									{
										ShoppingListItemViewModels.Remove(obj);
									});
									ShoppingListItemViewModels.Insert(0, shoppingListItemViewModel);

									var shoppingItemView = CreateShoppingItemView(shoppingListItemViewModel, scheduledShopping);
									StackLayoutShoppingItems.Children.Insert(0, shoppingItemView);
								}
								else
								{
									shoppingListItemViewModel.Model.Quantity += e.Quantity;
									shoppingListItemViewModel.UpdateModel();
								}
								scheduledShopping.ShoppingList.Items = ShoppingListItemViewModels.Select(t => t.Model).ToList();
							};
							await Utils.PushAsync(Navigation, addItemShoppingPage, true);
							LabelAddItemsShopping.TextColor = AppearanceBase.Instance.PrimaryColor;
						})
					});

					StackLayoutShoppingItems.Children.Clear();
					foreach (var item in ShoppingListItemViewModels)
					{
						var shoppingItemView = CreateShoppingItemView(item, scheduledShopping);
						StackLayoutShoppingItems.Children.Add(shoppingItemView);
					}

					SwitchServiceWeeklyShopping.IsToggled = scheduledShopping.IsWeekly;
					SwitchServiceWeeklyShopping.Toggled += (sender, e) =>
					{
						scheduledShopping.IsWeekly = e.Value;
					};

					PickerStorePreferancesShopping.Items.Clear();
					PickerStorePreferancesShopping.Items.Add(AppResources.NoPreference);
					PickerStorePreferancesShopping.SelectedIndexChanged += (sender, e) =>
					{
						if (PickerStorePreferancesShopping.SelectedIndex > 0)
						{
							scheduledShopping.ShoppingStoreID = mShoppingStores[PickerStorePreferancesShopping.SelectedIndex - 1].ID;
						}
						else
						{
							scheduledShopping.ShoppingStoreID = null;
						}
					};

					LabelProvideCommentShopping.Text = scheduledShopping.SpecialInstructions;
					EntryProvideCommentShopping.Text = scheduledShopping.SpecialInstructions;
					EntryProvideCommentShopping.TextChanged += (sender, e) =>
					{
						scheduledShopping.SpecialInstructions = EntryProvideCommentShopping.Text;
					};

					this.IsBusy = true;
					GridContent.IsVisible = false;
					Task.Run(() =>
					{
						Utils.IReloadPageCurrent = this;
						try
						{
							var zipCode = scheduledShopping.ServiceAddress.BasicAddress.ZipCode;
							mShoppingStores = Shared.APIs.IShoppingLists.GetStores(scheduledShopping.ServiceAddress.BasicAddress.ZipCode);
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
						if (mShoppingStores != null)
						{
							var storeNames = mShoppingStores.Select(s => s.DisplayName).ToList();
							foreach (var storeName in storeNames)
							{
								PickerStorePreferancesShopping.Items.Add(storeName);
							}

							PickerStorePreferancesShopping.SelectedIndex = mShoppingStores.FindIndex(s => s.ID == scheduledShopping.ShoppingStoreID) + 1;
						}
						GridContent.IsVisible = true;
						this.IsBusy = false;
					}, TaskScheduler.FromCurrentSynchronizationContext());
				}
				else if (ScheduledService is ScheduledPersonalService)
				{
					var scheduledPersonalService = ScheduledService as ScheduledPersonalService;
					GridPersonal.IsVisible = true;

					addressLabelPersonal.Text = scheduledPersonalService.ServiceAddress.BasicAddress.ToAddressLine();

					if (scheduledPersonalService.Type == ScheduledPersonalService.PersonalServiceType.Waiting)
					{
						CustomRadioButtonsWaiting = new List<CustomRadioButton>();
						var preCheck = true;
						StackLayoutWaiting.Children.Clear();
						StackLayoutWaiting.IsVisible = true;
						foreach (var waiting in Waitings)
						{
							RadioButtonCustom customRadioButton = new RadioButtonCustom();
							customRadioButton.Text = waiting;
							customRadioButton.TextColor = Color.Black;
							customRadioButton.CheckedChanged += (sender, e) =>
							{
								if (!preCheck)
								{
									if (IsEdit || Device.RuntimePlatform != Device.iOS)
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
											scheduledPersonalService.WaitingFor = customRadioButton.Text;
										}
									}
									else
									{
										preCheck = true;
										customRadioButton.Checked = !customRadioButton.Checked;
										preCheck = false;
									}
								}
							};
							CustomRadioButtonsWaiting.Add(customRadioButton);
							StackLayoutWaiting.Children.Add(customRadioButton);
							if (!string.IsNullOrEmpty(scheduledPersonalService.WaitingFor))
							{
								customRadioButton.Checked = (scheduledPersonalService.WaitingFor.ToLower() == waiting.ToLower());
							}
						}
						preCheck = false;
					}


					if (scheduledPersonalService.Type == ScheduledPersonalService.PersonalServiceType.Laundry)
					{
						StackLayoutLaundryService.IsVisible = true;
						StackLayoutLaundryService.Children.Clear();
						CustomRadioButtonsLaundry = new List<CustomRadioButton>();
						var preCheck = true;
						foreach (var laundry in Laundries)
						{
							RadioButtonCustom customRadioButton = new RadioButtonCustom();
							customRadioButton.Text = laundry;
							customRadioButton.TextColor = Color.Black;
							customRadioButton.CheckedChanged += (sender, e) =>
							{
								if (!preCheck)
								{
									if (IsEdit || Device.RuntimePlatform != Device.iOS)
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
											scheduledPersonalService.DetergentType = customRadioButton.Text;
										}
									}
									else
									{
										preCheck = true;
										customRadioButton.Checked = !customRadioButton.Checked;
										preCheck = false;
									}
								}
							};
							CustomRadioButtonsLaundry.Add(customRadioButton);
							StackLayoutLaundryService.Children.Add(customRadioButton);
							if (!string.IsNullOrEmpty(scheduledPersonalService.DetergentType))
							{
								customRadioButton.Checked = (scheduledPersonalService.DetergentType.ToLower() == laundry.ToLower());
							}
						}
						//if (CustomRadioButtonsLaundry.FirstOrDefault() != null)
						//{
						//	CustomRadioButtonsLaundry.FirstOrDefault().Checked = true;
						//}
						preCheck = false;
					}

					if (scheduledPersonalService.Type == ScheduledPersonalService.PersonalServiceType.HouseSitting)
					{
						StackLayoutHouseSitting.IsVisible = true;
						StackLayoutHouseSitting.Children.Clear();
						CheckBoxsHouseSitting = new List<CheckBox>();
						var preCheck = true;
						foreach (var houseSitting in HouseSittings)
						{
							CheckBoxCustom checkBox = new CheckBoxCustom();
							checkBox.DefaultText = houseSitting;
							checkBox.TextColor = Color.Black;
							checkBox.CheckedChanged += (sender, e) =>
							{
								if (!preCheck)
								{
									if (IsEdit || Device.RuntimePlatform != Device.iOS)
									{
										scheduledPersonalService.WaterPlants = CheckBoxsHouseSitting[0].Checked;
										scheduledPersonalService.FeedPets = CheckBoxsHouseSitting[1].Checked;
										scheduledPersonalService.CheckMail = CheckBoxsHouseSitting[2].Checked;
										scheduledPersonalService.NewsPaper = CheckBoxsHouseSitting[3].Checked;
										scheduledPersonalService.TurnOnLights = CheckBoxsHouseSitting[4].Checked;
										scheduledPersonalService.OtherHouseSitting = CheckBoxsHouseSitting[5].Checked;
										GridUpdateShopping.IsVisible = CheckPersonal(scheduledPersonalService);
									}
									else
									{
										preCheck = true;
										checkBox.Checked = !checkBox.Checked;
										preCheck = false;
									}
								}
							};
							CheckBoxsHouseSitting.Add(checkBox);
							StackLayoutHouseSitting.Children.Add(checkBox);
						}
						CheckBoxsHouseSitting[0].Checked = scheduledPersonalService.WaterPlants;
						CheckBoxsHouseSitting[1].Checked = scheduledPersonalService.FeedPets;
						CheckBoxsHouseSitting[2].Checked = scheduledPersonalService.CheckMail;
						CheckBoxsHouseSitting[3].Checked = scheduledPersonalService.NewsPaper;
						CheckBoxsHouseSitting[4].Checked = scheduledPersonalService.TurnOnLights;
						CheckBoxsHouseSitting[5].Checked = scheduledPersonalService.OtherHouseSitting;
						preCheck = false;
					}

					DatePickerServicePersonal.Date = new DateTime(scheduledPersonalService.ServiceDate.Year,
																  scheduledPersonalService.ServiceDate.Month, scheduledPersonalService.ServiceDate.Day);
					DatePickerServicePersonal.MinimumDate = DateTime.Now.Date.AddDays(1);
					DatePickerServicePersonal.Format = "MM/dd/yyyy";
					DatePickerServicePersonal.DateSelected += (sender, e) =>
					{
						scheduledPersonalService.ServiceDate = new SimpleDate(e.NewDate.Year,
													  e.NewDate.Month, e.NewDate.Day);
						GridUpdateShopping.IsVisible = CheckPersonal(scheduledPersonalService);
					};

					if (scheduledPersonalService.EndDate.Year == 0)
					{
						scheduledPersonalService.EndDate = scheduledPersonalService.ServiceDate;
					}

					DatePickerEndDatePersonal.Date = new DateTime(scheduledPersonalService.EndDate.Year,
												  scheduledPersonalService.EndDate.Month, scheduledPersonalService.EndDate.Day);

					DatePickerEndDatePersonal.MinimumDate = DateTime.Now.Date.AddDays(1);
					DatePickerEndDatePersonal.Format = "MM/dd/yyyy";
					DatePickerEndDatePersonal.DateSelected += (sender, e) =>
					{
						scheduledPersonalService.EndDate = new SimpleDate(e.NewDate.Year,
																		  e.NewDate.Month, e.NewDate.Day);
						GridUpdateShopping.IsVisible = CheckPersonal(scheduledPersonalService);
					};
					StackLayoutDatePickerEndDatePersonal.IsVisible = scheduledPersonalService.Type == ScheduledPersonalService.PersonalServiceType.HouseSitting;

					LabelProvideCommentPersonal.Text = scheduledPersonalService.SpecialInstructions;
					EntryProvideCommentPersonal.Text = scheduledPersonalService.SpecialInstructions;
					EntryProvideCommentPersonal.TextChanged += (sender, e) =>
					{
						scheduledPersonalService.SpecialInstructions = EntryProvideCommentPersonal.Text;
					};
				}
				else if (ScheduledService is ScheduledSpecialRequest)
				{
					ScheduledSpecialRequest scheduledSpecialRequest = ScheduledService as ScheduledSpecialRequest;
					StackLayoutSpecial.IsVisible = true;
					DatePickerServiceSpecial.Date = new DateTime(scheduledSpecialRequest.ServiceDate.Year,
																  scheduledSpecialRequest.ServiceDate.Month, scheduledSpecialRequest.ServiceDate.Day);
					DatePickerServiceSpecial.MinimumDate = DateTime.Now.Date.AddDays(1);
					DatePickerServiceSpecial.Format = "MM/dd/yyyy";
					DatePickerServiceSpecial.DateSelected += (sender, e) =>
					{
					};
					LabelProvideCommentSpecial.Text = scheduledSpecialRequest.SpecialInstructions;
					EntryProvideCommentSpecial.Text = scheduledSpecialRequest.SpecialInstructions;
					addressLabelSpecial.Text = scheduledSpecialRequest.ServiceAddress.BasicAddress.ToAddressLine();
					EntryProvideCommentSpecial.TextChanged += (sender, e) =>
					{
						scheduledSpecialRequest.SpecialInstructions = EntryProvideCommentSpecial.Text;
					};
				}
				else if (ScheduledService is ScheduledErrand)
				{
					ScheduledErrand scheduledErrand = ScheduledService as ScheduledErrand;
					GridErrand.IsVisible = true;

					//LabelErrandType.Text = scheduledErrand.Type.ToString().Trim();

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

					if (scheduledErrand.DropoffTime != null)
					{
						var dropoffTime = TimeZoneInfo.ConvertTime(DateTime.Parse(scheduledErrand.DropoffTime.Time), TimeZoneInfo.Local);
						var dropoffTimeString = dropoffTime.ToString(timeFormat);
						PickerStartTime.SelectedIndex = Math.Max(0, PickerStartTime.Items.ToList().FindIndex(t => t == dropoffTimeString));
					}

					if (scheduledErrand.PickupTime != null)
					{
						var pickupTime = TimeZoneInfo.ConvertTime(DateTime.Parse(scheduledErrand.PickupTime.Time), TimeZoneInfo.Local);
						var pickupTimeString = pickupTime.ToString(timeFormat);
						PickerEndTime.SelectedIndex = Math.Max(0, PickerEndTime.Items.ToList().FindIndex(t => t == pickupTimeString));
					}

					PickerStartTime.SelectedIndexChanged += (sender, e) =>
					{
						if (PickerStartTime.SelectedIndex > 0)
						{
							var date = DatePickerServiceErrand.Date;
							var time = DateTime.ParseExact(PickerStartTime.Items[PickerStartTime.SelectedIndex], timeFormat, null);
							scheduledErrand.DropoffTime = date.Add(time.TimeOfDay).ToUniversalTime();
							GridUpdateShopping.IsVisible = CheckErrand(scheduledErrand);
							var dayErrand = CheckDayErrand(scheduledErrand);
							ButtonUpdateShopping.IsEnabled = dayErrand;
							LabelDateAreIncorrect.IsVisible = !dayErrand;
						}
					};

					PickerEndTime.SelectedIndexChanged += (sender, e) =>
					{
						if (PickerEndTime.SelectedIndex > 0)
						{
							var date = DatePickerServiceErrand.Date;
							var time = DateTime.ParseExact(PickerEndTime.Items[PickerEndTime.SelectedIndex], timeFormat, null);
							scheduledErrand.PickupTime = date.Add(time.TimeOfDay).ToUniversalTime();
							GridUpdateShopping.IsVisible = CheckErrand(scheduledErrand);
							var dayErrand = CheckDayErrand(scheduledErrand);
							ButtonUpdateShopping.IsEnabled = dayErrand;
							LabelDateAreIncorrect.IsVisible = !dayErrand;
						}
					};

					CheckBoxStartTime.Checked = scheduledErrand.Dropoff;
					CheckBoxEndTime.Checked = scheduledErrand.Pickup;
					var reCheck = false;
					CheckBoxStartTime.CheckedChanged += (sender, e) =>
									{
										if (!reCheck)
										{
											if (IsEdit || Device.RuntimePlatform != Device.iOS)
											{
												scheduledErrand.Dropoff = CheckBoxStartTime.Checked;
												GridUpdateShopping.IsVisible = CheckErrand(scheduledErrand);
												var dayErrand = CheckDayErrand(scheduledErrand);
												ButtonUpdateShopping.IsEnabled = dayErrand;
												LabelDateAreIncorrect.IsVisible = !dayErrand;
											}
											else
											{
												reCheck = true;
												CheckBoxStartTime.Checked = !CheckBoxStartTime.Checked;
												reCheck = false;
											}
										}
									};

					CheckBoxEndTime.CheckedChanged += (sender, e) =>
					{
						if (!reCheck)
						{
							if (IsEdit || Device.RuntimePlatform != Device.iOS)
							{
								scheduledErrand.Pickup = CheckBoxEndTime.Checked;
								GridUpdateShopping.IsVisible = CheckErrand(scheduledErrand);
								var dayErrand = CheckDayErrand(scheduledErrand);
								ButtonUpdateShopping.IsEnabled = dayErrand;
								LabelDateAreIncorrect.IsVisible = !dayErrand;
							}
							else
							{
								reCheck = true;
								CheckBoxStartTime.Checked = !CheckBoxStartTime.Checked;
								reCheck = false;
							}
						}
					};

					EntryNameBusiness.Text = scheduledErrand.ErrandLocationName;
					EntryNameBusiness.TextChanged += (sender, e) =>
					{
						scheduledErrand.ErrandLocationName = EntryNameBusiness.Text;
						GridUpdateShopping.IsVisible = CheckErrand(scheduledErrand);
					};

					DatePickerServiceErrand.Date = DateTime.Now.Date;
					DatePickerServiceErrand.MinimumDate = DateTime.Now.Date;
					DatePickerServiceErrand.Format = "MM/dd/yyyy";
					DatePickerServiceErrand.Date = new DateTime(scheduledErrand.ServiceDate.Year,
																  scheduledErrand.ServiceDate.Month, scheduledErrand.ServiceDate.Day);

					addressLabelErrand.Text = scheduledErrand.ServiceAddress.BasicAddress.ToAddressLine();
					LabelAddressBusiness.Text = scheduledErrand.ErrandLocation.ToAddressLine();
					LabelProvideCommentErrand.Text = scheduledErrand.SpecialInstructions;
					EntryProvideCommentErrand.Text = scheduledErrand.SpecialInstructions;
					EntryProvideCommentErrand.TextChanged += (sender, e) =>
					{
						scheduledErrand.SpecialInstructions = EntryProvideCommentErrand.Text;
					};
				}
				else if (ScheduledService is ScheduledRestaurantService)
				{
					//isEnabled = false;
					GridEditOrder.IsVisible = false;

					ScheduledRestaurantService scheduledRestaurantService = ScheduledService as ScheduledRestaurantService;
					if (!string.IsNullOrEmpty(scheduledRestaurantService.SpecialInstructions))
					{
						LabelRestaurantSpecialInstructions.Text = "Order notes: " + scheduledRestaurantService.SpecialInstructions;
					}
					else
					{
						LabelRestaurantSpecialInstructions.IsVisible = false;
					}
					LabelRestaurantServiceAddress.Text = scheduledRestaurantService.Delivery ? AppResources.DeliveryAddress : AppResources.PickupAddress;
					addressLabelRestaurant.Text = scheduledRestaurantService.Delivery ?
						scheduledRestaurantService.ServiceAddress.BasicAddress.ToAddressLine() : string.Empty;

					StackLayoutRestaurantStatus.IsVisible = false;

					RestaurantLocation restaurantLocation = null;
					Restaurant restaurant = null;
					this.IsBusy = true;
					GridContent.IsVisible = false;
					Task.Run(() =>
					{
						Utils.IReloadPageCurrent = this;
						try
						{
							restaurantLocation = Shared.APIs.IRestaurant.GetLocation(scheduledRestaurantService.RestaurantLocationID);
							if (restaurantLocation != null)
							{
								restaurant = Shared.APIs.IRestaurant.GetRestaurant(restaurantLocation.RestaurantID);
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
					}).ContinueWith((arg) =>
					{
						if (restaurant != null)
						{
							LabelRestaurantName.Text = restaurant.DisplayName;
							LabelRestaurantAddress.Text = restaurantLocation.Address.ToAddressLine();
							if (!scheduledRestaurantService.Delivery)
							{
								addressLabelRestaurant.Text = restaurantLocation.Address.ToAddressLine();
							}
						}
						this.IsBusy = false;
						GridContent.IsVisible = true;
					}, TaskScheduler.FromCurrentSynchronizationContext());

					StackLayoutRestaurant.IsVisible = true;
					LabelRestaurantScheduledService.IsVisible = false;
					this.Title = scheduledRestaurantService.Delivery ? AppResources.RestaurantDelivery : AppResources.RestaurantPickup;
					var serviceStartTime = TimeZoneInfo.ConvertTime(DateTime.Parse(scheduledRestaurantService.ServiceStartTime.Time), TimeZoneInfo.Local);
					var serviceEndTime = TimeZoneInfo.ConvertTime(DateTime.Parse(scheduledRestaurantService.ServiceEndTime.Time), TimeZoneInfo.Local);

					LabelServiceRestaurantDateTitle.Text = AppResources.Date;
					LabelServiceRestaurantTimeTitle.Text = AppResources.Time;
					(LabelServiceRestaurantTimeTitle.Parent as Layout).ForceLayout();


					LabelServiceRestaurantTime.Text = serviceStartTime.ToString("h:mm tt") + " - "
										+ serviceEndTime.ToString("h:mm tt");

					LabelServiceRestaurantDate.Text = new DateTime(serviceStartTime.Year,
																   serviceStartTime.Month, serviceStartTime.Day).ToString("MM/dd/yyyy");
					StackLayoutCarts.Children.Clear();
					foreach (var item in scheduledRestaurantService.Items)
					{
						var cartItemView = new CartItemView();
						cartItemView.BindingContext = item;
						cartItemView.Margin = new Thickness(0, 0, 0, 12);
						StackLayoutCarts.Children.Add(cartItemView);
					}
				}

				if (Device.Idiom == TargetIdiom.Tablet)
				{
					LabelAddItemsShopping.FontSize = Device.GetNamedSize(NamedSize.Large, LabelAddItemsShopping);
					LabelEditOrder.FontSize = Device.GetNamedSize(NamedSize.Large, LabelEditOrder);
					LabelCancelOrder.FontSize = Device.GetNamedSize(NamedSize.Large, LabelCancelOrder);
				}
				else
				{
					LabelAddItemsShopping.FontSize = Device.GetNamedSize(NamedSize.Medium, LabelAddItemsShopping);
					LabelEditOrder.FontSize = Device.GetNamedSize(NamedSize.Medium, LabelEditOrder);
					LabelCancelOrder.FontSize = Device.GetNamedSize(NamedSize.Medium, LabelCancelOrder);
				}

				GridEditCancel.IsVisible = isEnabled;
				if (isEnabled)
				{
					GridEditOrder.GestureRecognizers.Add(new TapGestureRecognizer
					{
						Command = new Command(() =>
						{
							SetEdit(this.Content, true);
							GridUpdateShopping.IsVisible = true;
						})
					});
					GridCancelOrder.GestureRecognizers.Add(new TapGestureRecognizer
					{
						Command = new Command(async () =>
						{
							var cancel = await DisplayAlert(AppResources.Cancel, AppResources.CancelMessage, AppResources.Yes, AppResources.No);
							if (cancel)
							{

								this.IsBusy = true;
								await Task.Run(() =>
								{
									try
									{
										Shared.APIs.IUsers.CancelScheduledService(Shared.UserId, scheduledService.ID);
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
																var pages = Navigation.NavigationStack.Reverse().ToList();
																IServicesTabPage servicesTabPage = null;
																foreach (var page in pages)
																{
																	if (page is IServicesTabPage)
																	{
																		servicesTabPage = page as IServicesTabPage;
																		break;
																	}
																}
																if (servicesTabPage != null)
																{
																	servicesTabPage.SelectScheduleTab(true);
																}

																Navigation.PopAsync().ConfigureAwait(false);
																this.IsBusy = false;
															}, TaskScheduler.FromCurrentSynchronizationContext());
							}
						})
					});
				}

				SetEdit(this.Content, false);
				GridUpdateShopping.IsVisible = false;
			}
		}

		public override void ReloadPage()
		{
			base.ReloadPage();
			if (ScheduledService != null)
			{
				LoadScheduledService(ScheduledService);
			}
			else
			{
				LoadScheduledService(mScheduledServiceId);
			}
		}

		public ShoppingItemView CreateShoppingItemView(ShoppingListItemViewModel item, ScheduledShopping scheduledShopping)
		{
			ShoppingItemView shoppingItemView = new ShoppingItemView();
			shoppingItemView.BindingContext = item;
			shoppingItemView.Clicked += (sender, e) =>
			{
				if (StackLayoutShoppingItems.IsEnabled)
				{
					var shoppingListItemViewModel = item as ShoppingListItemViewModel;
					var addItemShoppingPage = new AddOrEditItemShoppingPage(shoppingListItemViewModel.Model);
					addItemShoppingPage.Done += (sender2, e2) =>
					{
						shoppingListItemViewModel.Model = e2;
						shoppingListItemViewModel.UpdateModel();
						scheduledShopping.ShoppingList.Items = ShoppingListItemViewModels.Select(t => t.Model).ToList();
					};
					Utils.PushAsync(Navigation, addItemShoppingPage, true);
					//ListViewShoppingListItem.SelectedItem = null;
				}
			};
			return shoppingItemView;
		}

	    public void SetEdit(View view, bool isEdit)
		{
			IsEdit = isEdit;
			if (GridEditOrder == view || GridCancelOrder == view)
			{
				GridEditOrder.IsEnabled = true;
				GridCancelOrder.IsEnabled = true;
			}
			if (view is CheckBox || view is CustomRadioButton)
			{
				//fix checkbox & radiobutton for iOS
				view.IsEnabled = Device.RuntimePlatform == Device.iOS || isEdit;
			}
			else if (EntryProvideCommentErrand == view)
			{
				EntryProvideCommentErrand.IsVisible = isEdit;
				LabelProvideCommentErrand.IsVisible = !isEdit;
			}
			else if (EntryProvideCommentSpecial == view)
			{
				EntryProvideCommentSpecial.IsVisible = isEdit;
				LabelProvideCommentSpecial.IsVisible = !isEdit;
			}
			else if (EntryProvideCommentPersonal == view)
			{
				EntryProvideCommentPersonal.IsVisible = isEdit;
				LabelProvideCommentPersonal.IsVisible = !isEdit;
			}
			else if (EntryProvideCommentShopping == view)
			{
				EntryProvideCommentShopping.IsVisible = isEdit;
				LabelProvideCommentShopping.IsVisible = !isEdit;
			}
			else if (view == StackLayoutShoppingItems)
			{
				StackLayoutShoppingItems.IsEnabled = isEdit;
			}
			else if (view == LabelAddItemsShopping)
			{
				LabelAddItemsShopping.IsVisible = isEdit;
			}
			else if (!(view is Label))
			{
				if (view is Layout<View>)
				{
					foreach (var childView in (view as Layout<View>).Children)
					{
						SetEdit(childView, isEdit);
					}
				}
				else if (view is ScrollView)
				{
					SetEdit(((ScrollView)view).Content, isEdit);
				}
				else
				{
					view.IsEnabled = isEdit;
				}
			}
		}

		public bool CheckPersonal(ScheduledPersonalService scheduledPersonalService)
		{
			if (scheduledPersonalService.Type == ScheduledPersonalService.PersonalServiceType.HouseSitting
					&& CheckBoxsHouseSitting.Count(t => t.Checked) == 0)
			{
				return false;
			}
			if (StackLayoutDatePickerEndDatePersonal.IsVisible && scheduledPersonalService.EndDate < scheduledPersonalService.ServiceDate)
			{
				return false;
			}
			return true;
		}

		public bool CheckDayErrand(ScheduledErrand scheduledErrand)
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


		public bool CheckErrand(ScheduledErrand scheduledErrand)
		{
			if (string.IsNullOrEmpty(scheduledErrand.ErrandLocationName))
			{
				return false;
			}

			return true;
		}
   }

}
