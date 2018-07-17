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
    public partial class ScheduledPaymentPage : ContentPageBase
	{
		private bool mFirstLoad = true;
		private bool mIsReloadTimeSlot = false;

		public RestaurantDetailPage RestaurantDetailPage
		{
			get;
			set;
		}
		AddressFacade mAddressFacade = new AddressFacade();
		ExtendedAddress mServiceAddress;
		private IAppServices mAppServices;
		List<RMenuOrderingAvailableSlot> MenuOrderingAvailables = new List<RMenuOrderingAvailableSlot>();
		DateTime SelectedDate;
		bool IsDeliverySelected;
		List<DateTime> Dates = new List<DateTime>();
		List<DateTime> Times = new List<DateTime>();
		bool IsValidateAddress = false;

		private int mSelectedIndex = 0;
		public int SelectedIndex
		{
			get
			{
				return mSelectedIndex ;
			}
			set
			{
				if (mSelectedIndex != value)
				{
					OnPropertyChanging(nameof(SelectedIndex));
					mSelectedIndex = value;
					OnPropertyChanged(nameof(SelectedIndex));
				}
			}
		}

		public ScheduledPaymentPage(RestaurantDetailPage restaurantDetailPage)
		{
			RestaurantDetailPage = restaurantDetailPage;
			MenuOrderingAvailables = restaurantDetailPage.RefreshMenuAvailables;
			SelectedDate = restaurantDetailPage.SelectedDate;
			IsDeliverySelected = restaurantDetailPage.IsDeliverySelected;

			InitializeComponent();

			mAppServices = DependencyService.Get<IAppServices>();
			GridDeliveryAddress.IsVisible = IsDeliverySelected || restaurantDetailPage.GroupedDeliveryDestination != null;

			LabelProvideComment.IsVisible = string.IsNullOrEmpty(EntryProvideComment.Text);
			EntryProvideComment.TextChanged += (sender, e) =>
			{
				LabelProvideComment.IsVisible = string.IsNullOrEmpty(EntryProvideComment.Text);
			};

			addressEntry.GroupedDeliveryDestinationChanged += (sender, e) =>
			{
				restaurantDetailPage.GroupedDeliveryDestination = e;
				mServiceAddress = e.Address;
				mAppServices.TrackEvent("RMP Date", "click", "address");
				if (mServiceAddress != null)
				{
					addressEntry.Text = mServiceAddress.BasicAddress.ToCannonicalString();
					apartmentEntry.Text = mServiceAddress.BasicAddress.Line2;
					ValidateAddress(true);
				}
				else
				{
					addressEntry.Text = string.Empty;
					apartmentEntry.Text = string.Empty;
				}

				ButtonCheckout.IsEnabled = mServiceAddress != null;
			};

			addressEntry.AddressChanged += (sender, e) =>
			{
				mServiceAddress = e;
				mAppServices.TrackEvent("RMP Date", "click", "address");
				if (mServiceAddress != null)
				{
					addressEntry.Text = mServiceAddress.BasicAddress.ToCannonicalString();
					apartmentEntry.Text = mServiceAddress.BasicAddress.Line2;
					ValidateAddress();
				}
				else
				{
					addressEntry.Text = string.Empty;
					apartmentEntry.Text = string.Empty;
				}

				ButtonCheckout.IsEnabled = mServiceAddress != null;
			};

			mServiceAddress = mAddressFacade.GetUserAddress();
			if (mServiceAddress != null)
			{
				addressEntry.Text = mServiceAddress.BasicAddress.ToAddress();
				apartmentEntry.Text = mServiceAddress.BasicAddress.Line2;
			}
			else 
			{
				addressEntry.Text = string.Empty;
				apartmentEntry.Text = string.Empty;
			}

			apartmentEntry.TextChanged += (sender, e) =>
			{
				if (mServiceAddress != null)
				{
					mServiceAddress.BasicAddress.Line2 = apartmentEntry.Text;
				}
			};

			//addressEntry.IsEnabled = restaurantDetailPage.GroupedDeliveryDestination == null;
			addressEntry.IsGroupedDeliveryDestination = restaurantDetailPage.GroupedDeliveryDestination != null;
			apartmentEntry.IsEnabled = restaurantDetailPage.GroupedDeliveryDestination == null;
			//apartmentEntry.IsVisible = apartmentEntry.IsEnabled || !string.IsNullOrEmpty(apartmentEntry.Text);

			ButtonCheckout.IsEnabled = mServiceAddress != null;

			this.Title = restaurantDetailPage.RestaurantVM.SearchResultData.RestaurantDisplayName;

			SelectedIndex = RestaurantDetailPage.IsDeliverySelected ? 0 : 1;

			var date = RestaurantDetailPage.GetDateTimeRestaurantFromUtc(DateTime.UtcNow);
			if (RestaurantDetailPage.MinDate != null)
			{
				date = RestaurantDetailPage.MinDate.Value;
			}
			Dates = new List<DateTime>();
			for (int i = 0; i < RestaurantDetailPage.NextDays; i++)
			{
				var currentTime = RestaurantDetailPage.GetDateTimeRestaurantFromUtc(DateTime.UtcNow);
				var menuIds = RestaurantDetailPage.ListMenuItemViews.Values.SelectMany(t => t)
												  .SelectMany(t => t.ListMenuGroupModifierItemSelected)
												  .Select(s => s.MenuItemView.RestaurantMenuItem.MenuId).Distinct().ToList();
				var dateMenuOrderingAvailables = MenuOrderingAvailables
										.Where(s => RestaurantDetailPage.GetDateTimeRestaurantFromLocal(s.StartTime.Time).Date == date.AddDays(i).Date)
										.Where(s => RestaurantDetailPage.GetDateTimeRestaurantFromLocal(s.CutoffTime.Time) > currentTime)
										.ToList();
				var times = dateMenuOrderingAvailables
						.Where(s =>
						{
							var menuAvailableIDs = dateMenuOrderingAvailables
													.Where(ma => ma.StartTime.Time.Equals(s.StartTime.Time))
													.SelectMany(ma => ma.RelatedMenuIDs)
													.ToList();
							return !menuIds.Except(menuAvailableIDs).Any();
						})
						.Count();
				

				if (times > 0 && !RestaurantDetailPage.DateRestaurantCloses.Contains(date.AddDays(i).Date))
				{
					Dates.Add(date.AddDays(i));
				}
			}
			var itemDates = Dates.Select(d => d.ToString("ddd, MMM, dd")).ToList();
			foreach (var itemDate in itemDates)
			{
				PickerDate.Items.Add(itemDate);
			}
			var dateSelectedIndex = itemDates.FindIndex(t => t == SelectedDate.ToString("ddd, MMM, dd"));
			PickerDate.SelectedIndex = dateSelectedIndex;

			UpdateTimeSource();

			foreach (var item in BindableRadioGroupDeliveryPickup.Items)
			{
				item.CheckedChanged += (object sender, XLabs.EventArgs<bool> e) =>
				{
					var countCheck = BindableRadioGroupDeliveryPickup.Items.Count(t => t.Checked);
					if (countCheck == 0 && SelectedIndex >= 0)
					{
						BindableRadioGroupDeliveryPickup.Items[SelectedIndex].Checked = true;
					}
				};
			}
			BindableRadioGroupDeliveryPickup.Items[SelectedIndex].Checked = true;
			BindableRadioGroupDeliveryPickup.Items[0].IsEnabled = RestaurantDetailPage.RestaurantVM.SearchResultData.IsDeliveryAvailable;
			BindableRadioGroupDeliveryPickup.Items[0].TextColor = RestaurantDetailPage.RestaurantVM.SearchResultData.IsDeliveryAvailable ? Color.Black : Color.Gray;
			BindableRadioGroupDeliveryPickup.Items[1].IsEnabled = RestaurantDetailPage.RestaurantVM.SearchResultData.IsPickupAvailable;
			BindableRadioGroupDeliveryPickup.Items[1].TextColor = RestaurantDetailPage.RestaurantVM.SearchResultData.IsPickupAvailable ? Color.Black : Color.Gray;

			BindableRadioGroupDeliveryPickup.CheckedChanged += (sender, e) =>
			{
				var isDeliverySelected = BindableRadioGroupDeliveryPickup.SelectedIndex == 0;
				if (IsDeliverySelected != isDeliverySelected)
				{
					mAppServices.TrackEvent("RMP Pickup/Delivery", "click", "address");

					IsDeliverySelected = isDeliverySelected;
					RestaurantDetailPage.IsDeliverySelected = isDeliverySelected;
					GridDeliveryAddress.IsVisible = IsDeliverySelected || restaurantDetailPage.GroupedDeliveryDestination != null;

                    ReloadTimeSlot();

					//this.IsBusy = true;
					//Task.Run(() =>
					//{
					//	try
					//	{
					//		var dateUTC = DateTime.UtcNow;
					//		var dateRestaurant = RestaurantDetailPage.GetDateTimeRestaurantFromUtc(dateUTC);
					//		//var dateRestaurant = RestaurantDetailPage.GetDateLocalTimeRestaurant(SelectedDate.Date).ToUniversalTime();
					//		var restaurantFacade = new RestaurantFacade();
					//		MenuOrderingAvailables = restaurantFacade.RefreshMenus(RestaurantDetailPage.RestaurantVM.LocationId, dateUTC, dateRestaurant.AddDays(RestaurantDetailPage.CheckNextDays), IsDeliverySelected);
					//	}
					//	catch (Exception)
					//	{
					//		Device.BeginInvokeOnMainThread(async () =>
					//		{
					//			await Utils.ShowErrorMessage(AppResources.SomethingWentWrong, 2);
					//		});
					//	}
					//}).ContinueWith(t =>
					//{
					//	if (MenuOrderingAvailables == null)
					//	{
					//		MenuOrderingAvailables = new List<RMenuOrderingAvailableSlot>();
					//	}
					//	else
					//	{
					//		MenuOrderingAvailables = MenuOrderingAvailables
					//								.OrderBy(s => s.StartTime)
					//								.ToList();
					//	}
     //                   UpdateTimeSource();
					//	this.IsBusy = IsValidateAddress;
					//}, TaskScheduler.FromCurrentSynchronizationContext());
				}
			};
			StackLayoutBindableRadioGroupDeliveryPickup.IsVisible = restaurantDetailPage.GroupedDeliveryDestination == null;
			GridApartment.IsVisible = restaurantDetailPage.GroupedDeliveryDestination == null;

			this.SizeChanged += (sender, e) =>
			{
				foreach (var item in BindableRadioGroupDeliveryPickup.Items)
				{
					item.WidthRequest = (BindableRadioGroupDeliveryPickup.Width - BindableRadioGroupDeliveryPickup.Padding.Left) / 2;
				}
			};

			PickerDate.SelectedIndexChanged += (sender, e) =>
			{
				if (PickerDate.SelectedIndex >= 0)
				{
					var dateSelected = Dates[PickerDate.SelectedIndex].Date;
					if (SelectedDate != dateSelected)
					{
						mAppServices.TrackEvent("RMP Date", "click", "address");
					}

					SelectedDate = dateSelected;
                    UpdateTimeSource();

					ReloadTimeSlot();
                    
				}
			};

			PickerTime.SelectedIndexChanged += (sender, e) =>
			{
				if (PickerTime.SelectedIndex >= 0)
				{
					var indexTime = PickerTime.SelectedIndex;
					var dateSelected = Dates[PickerDate.SelectedIndex].Date;
					dateSelected = dateSelected.AddHours(Times[indexTime].Hour).AddMinutes(Times[indexTime].Minute);
					if (SelectedDate != dateSelected)
					{
						mAppServices.TrackEvent("RMP Date", "click", "address");
					}
					SelectedDate = dateSelected;
					RestaurantDetailPage.SelectedDate = dateSelected;
				}
			};

			ButtonCheckout.Clicked += (sender, e) =>
			{
				if (PickerTime.SelectedIndex >= 0 && mServiceAddress != null)
				{
					UserFacade userFacade = new UserFacade();
					userFacade.RequireLogin(this, () =>
					{
						this.IsBusy = true;
						var dateSelected = Dates[PickerDate.SelectedIndex].Date;
						dateSelected = dateSelected.AddHours(Times[PickerTime.SelectedIndex].Hour).AddMinutes(Times[PickerTime.SelectedIndex].Minute);
						SelectedDate = dateSelected;
						var restaurantFacade = new RestaurantFacade();
						var refreshMenuAvailables = new List<RMenuOrderingAvailableSlot>();
						var menuOrderingAvailables = new List<RMenuOrderingAvailableSlot>();
						Task.Run(() =>
						{
							try
							{

								refreshMenuAvailables = restaurantFacade.RefreshMenus(RestaurantDetailPage.RestaurantVM.LocationId, date, date.AddDays(RestaurantDetailPage.CheckNextDays), IsDeliverySelected, RestaurantDetailPage.GroupedDeliveryDestination);
								if (refreshMenuAvailables != null)
								{
									refreshMenuAvailables = refreshMenuAvailables.OrderBy(t => t.StartTime).ToList();

									menuOrderingAvailables = restaurantFacade.SearchAvailableOrderTimes(refreshMenuAvailables)
															 .OrderBy(t => t.StartTime)
															 .ToList();
								}

								//menuOrderingAvailables = restaurantFacade.SearchAvailableOrderTimes(RestaurantDetailPage.RestaurantVM.LocationId, date.AddDays(days).ToString("MM/dd/yyyy"), date.AddDays(RestaurantDetailPage.CheckNextDays).ToString("MM/dd/yyyy"), IsDeliverySelected)
								//						 .OrderBy(t => t.StartTime)
								//						 .ToList();
							}
							catch (Exception)
							{
								Device.BeginInvokeOnMainThread(async () =>
								{
									await Utils.ShowErrorMessage(AppResources.SomethingWentWrong, 2);
								});
							}
						}).ContinueWith(async t =>
						{
							if (refreshMenuAvailables != null && refreshMenuAvailables.Count > 0)
							{
								var listMenuGroupModifierItemSelected = restaurantDetailPage.GetMenuItemViewsClose(refreshMenuAvailables, SelectedDate).SelectMany(s => s.ListMenuGroupModifierItemSelected).ToList();
								var answer = true;
								if (listMenuGroupModifierItemSelected.Count > 0)
								{
									await DisplayAlert(AppResources.Warning, AppResources.MenuIsClosed, AppResources.OK);
									restaurantDetailPage.RefreshMenuAvailables = refreshMenuAvailables;
									restaurantDetailPage.MenuOrderingAvailables = menuOrderingAvailables;
									restaurantDetailPage.SelectedDate = SelectedDate;
									restaurantDetailPage.IsDeliverySelected = IsDeliverySelected;
									restaurantDetailPage.ClearMenuItemViewsClose(refreshMenuAvailables, SelectedDate);

									var pages = Navigation.NavigationStack.Reverse().Take(1).ToList();
									foreach (var page in pages)
									{
										if (page != restaurantDetailPage)
										{
											Navigation.RemovePage(page);
										}
										else
										{
											break;
										}
									}
									await Navigation.PopAsync().ConfigureAwait(false);
								}
								if (answer)
								{
									restaurantDetailPage.RefreshMenuAvailables = refreshMenuAvailables;
									restaurantDetailPage.MenuOrderingAvailables = menuOrderingAvailables;
									restaurantDetailPage.SelectedDate = SelectedDate;
									if (IsDeliverySelected != restaurantDetailPage.IsDeliverySelected)
									{
										restaurantDetailPage.SelectTimeDeliveryOrPickup(IsDeliverySelected);
									}
									restaurantDetailPage.ClearMenuItemViewsClose(refreshMenuAvailables, SelectedDate);

									string comment = EntryProvideComment.Text;
									var placeOrderPage = new PlaceOrderPage(comment, RestaurantDetailPage, mServiceAddress);
									await Utils.PushAsync(Navigation, placeOrderPage, true);
								}
							}

							this.IsBusy = IsValidateAddress;
						}, TaskScheduler.FromCurrentSynchronizationContext());
					});
				}
			};
		}

		public void ReloadTimeSlot()
		{
            this.IsBusy = true;
			Task.Run(() =>
			{
				Utils.IReloadPageCurrent = this;
				try
				{
					var dateUTC = DateTime.UtcNow;
					var dateRestaurant = RestaurantDetailPage.GetDateTimeRestaurantFromUtc(dateUTC);
					//var dateRestaurant = RestaurantDetailPage.GetDateLocalTimeRestaurant(SelectedDate.Date).ToUniversalTime();
					var restaurantFacade = new RestaurantFacade();
					var menuOrderingAvailables = restaurantFacade
						.RefreshMenus(RestaurantDetailPage.RestaurantVM.LocationId, dateUTC, dateRestaurant.AddDays(RestaurantDetailPage.CheckNextDays), IsDeliverySelected, RestaurantDetailPage.GroupedDeliveryDestination);
					if (menuOrderingAvailables != null)
					{
						MenuOrderingAvailables = menuOrderingAvailables
													.OrderBy(t => t.StartTime)
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
				if (Utils.IReloadPageCurrent == this)
				{
					Utils.IReloadPageCurrent = null;
				};
			}).ContinueWith(t =>
			{
				if (!this.IsErrorPage)
				{
					UpdateTimeSource();
				}
				else
				{
					mIsReloadTimeSlot = true;
					//PickerDate.Unfocus();
					//PickerTime.Unfocus();
				}
				this.IsBusy = IsValidateAddress;

			}, TaskScheduler.FromCurrentSynchronizationContext());
		}

		public override void ReloadPage()
		{
			base.ReloadPage();

			this.Content.HeightRequest = this.Height;
			this.Content.VerticalOptions = LayoutOptions.FillAndExpand;

			if (mIsReloadTimeSlot)
			{
				mIsReloadTimeSlot = false;
				ReloadTimeSlot();
			}
			else
			{
				ValidateAddress();
			}
		}

		public void ValidateAddress(bool isReloadTimeSlot = false)
		{
			if (mServiceAddress != null)
			{
				IsValidateAddress = true;
				this.IsBusy = true;
				var restaurants = new List<RestaurantVM>();
				RestaurantFacade restaurantFacade = new RestaurantFacade();
				var zipCode = mServiceAddress.BasicAddress.ZipCode;
				Task.Run(() =>
				{
					Utils.IReloadPageCurrent = this;
					var refreshMenuAvailables = new List<RMenuOrderingAvailableSlot>();
					var menuOrderingAvailables = new List<RMenuOrderingAvailableSlot>();
					try
					{
						restaurants = restaurantFacade.GetRestaurantByAddress(mServiceAddress.Latitude, mServiceAddress.Longitude, true, zipCode, -1, RestaurantDetailPage.GroupedDeliveryDestination);
						if (restaurants != null && isReloadTimeSlot)
						{

							var date = RestaurantDetailPage.GetDateTimeRestaurantFromUtc(DateTime.UtcNow);

							refreshMenuAvailables = restaurantFacade.RefreshMenus(RestaurantDetailPage.RestaurantVM.LocationId, date, date.AddDays(RestaurantDetailPage.CheckNextDays), IsDeliverySelected, RestaurantDetailPage.GroupedDeliveryDestination);
							if (refreshMenuAvailables != null && refreshMenuAvailables.Count > 0)
							{
								refreshMenuAvailables = refreshMenuAvailables.OrderBy(t => t.StartTime).ToList();

								menuOrderingAvailables = restaurantFacade.SearchAvailableOrderTimes(refreshMenuAvailables)
														 .OrderBy(t => t.StartTime)
														 .ToList();
								
								RestaurantDetailPage.RefreshMenuAvailables = refreshMenuAvailables;
								RestaurantDetailPage.MenuOrderingAvailables = menuOrderingAvailables;
								MenuOrderingAvailables = RestaurantDetailPage.RefreshMenuAvailables;
							}
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
				}).ContinueWith(async (t) =>
				{
					if (!this.IsErrorPage && restaurants != null)
					{
						var restaurantVM = restaurants.FirstOrDefault(s => s.RestaurantID ==  RestaurantDetailPage.RestaurantVM.RestaurantID
							  && (RestaurantDetailPage.GroupedDeliveryDestination == null || s.SearchResultData.IsGroupedDeliveryAvailable));
						if (restaurantVM == null)
						{
							await DisplayAlert(AppResources.Warning,
									   string.Format(AppResources.RestaurantAvailableMessage, RestaurantDetailPage.RestaurantVM.SearchResultData.RestaurantDisplayName, zipCode), AppResources.OK);
							this.IsBusy = false;
							Application.Current.MainPage = new HomePage();
						}
						else if (!restaurantVM.Location.SearchResultData.IsDeliveryAvailable
							&& RestaurantDetailPage.IsDeliverySelected)
						{
							BindableRadioGroupDeliveryPickup.SelectedIndex = 1;
							RestaurantDetailPage.RestaurantVM = restaurantVM;
							await DisplayAlert(AppResources.Warning, AppResources.PickupOnlyMessage, AppResources.OK);
							BindableRadioGroupDeliveryPickup.Items[0].IsEnabled = RestaurantDetailPage.RestaurantVM.SearchResultData.IsDeliveryAvailable;
							BindableRadioGroupDeliveryPickup.Items[0].TextColor = RestaurantDetailPage.RestaurantVM.SearchResultData.IsDeliveryAvailable ? Color.Black : Color.Gray;
							BindableRadioGroupDeliveryPickup.Items[1].IsEnabled = RestaurantDetailPage.RestaurantVM.SearchResultData.IsPickupAvailable;
							BindableRadioGroupDeliveryPickup.Items[1].TextColor = RestaurantDetailPage.RestaurantVM.SearchResultData.IsPickupAvailable ? Color.Black : Color.Gray;
							if (isReloadTimeSlot)
							{
                                UpdateTimeSource();
							}
							this.IsBusy = false;
						}
						else
						{
							BindableRadioGroupDeliveryPickup.Items[0].IsEnabled = RestaurantDetailPage.RestaurantVM.SearchResultData.IsDeliveryAvailable;
							BindableRadioGroupDeliveryPickup.Items[0].TextColor = RestaurantDetailPage.RestaurantVM.SearchResultData.IsDeliveryAvailable ? Color.Black : Color.Gray;
							BindableRadioGroupDeliveryPickup.Items[1].IsEnabled = RestaurantDetailPage.RestaurantVM.SearchResultData.IsPickupAvailable;
							BindableRadioGroupDeliveryPickup.Items[1].TextColor = RestaurantDetailPage.RestaurantVM.SearchResultData.IsPickupAvailable ? Color.Black : Color.Gray;
							if (isReloadTimeSlot)
							{
                                UpdateTimeSource();
							}
							this.IsBusy = false;
						}
					}
					IsValidateAddress = false;
				}, TaskScheduler.FromCurrentSynchronizationContext());
			}
		}

		bool IsAppearing = false;
		bool timeSlotTrigger = false;
		bool timeSlotTriggerLocked = false;
		protected override void OnDisappearing()
		{
			base.OnDisappearing();
			IsAppearing = false;
			timeSlotTrigger = false;
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			IsAppearing = true;
			addressEntry.Unfocus();

			if (mFirstLoad)
			{
				mFirstLoad = false;
				ValidateAddress();
			}

			if (!timeSlotTrigger)
			{
				timeSlotTrigger = true;
				SearchAvailableOrderTimes();
				Device.StartTimer(new TimeSpan(0, 0, 0, 2), () =>
				{
					if (!timeSlotTriggerLocked && timeSlotTrigger)
					{
						try
						{
							timeSlotTriggerLocked = true;
							if (!OrderClosed)
							{
								SearchAvailableOrderTimes();
							}
							timeSlotTriggerLocked = false;
						}
						catch (Exception)
						{
							timeSlotTriggerLocked = false;
							return timeSlotTrigger;
						}
					}
					return timeSlotTrigger;
				});
			}
		}

		private bool OrderClosed = false;
		public void CloseOrder()
		{
			OrderClosed = true;
			Device.BeginInvokeOnMainThread(async () =>
			{
				try
				{
					if (this.Navigation.NavigationStack.LastOrDefault() == this && IsAppearing)
					{
						await DisplayAlert(AppResources.MenuIsClosed, AppResources.OrderExpired, AppResources.OK);
						var pages = Navigation.NavigationStack.Reverse().Skip(1).ToList();
						foreach (var page in pages)
						{
							if (page != RestaurantDetailPage)
							{
								Navigation.RemovePage(page);
							}
							else
							{
								Navigation.RemovePage(page);
								break;
							}
						}
						await Navigation.PopAsync(true).ConfigureAwait(false);
					}
				}
				catch (Exception) { }
			});
		}

		public void SearchAvailableOrderTimes()
		{
			if (!this.IsBusy && !this.IsErrorPage)
			{
				var currentDate = RestaurantDetailPage.GetDateTimeRestaurantFromUtc(DateTime.UtcNow);
				var selectedDate = RestaurantDetailPage.SelectedDate;
				if (currentDate.Date > RestaurantDetailPage.SelectedDate)
				{
					CloseOrder();
					return;
				}
				else
				{
					RMenuOrderingAvailableSlot menuOrderingAvailableSlot = MenuOrderingAvailables
							.Where(s => RestaurantDetailPage.GetDateTimeRestaurantFromLocal(s.CutoffTime.Time).Date == SelectedDate.Date)
							.Where(s => RestaurantDetailPage.GetDateTimeRestaurantFromLocal(s.CutoffTime.Time) > currentDate)
							.FirstOrDefault();
					if (menuOrderingAvailableSlot != null)
					{
						var minDate = RestaurantDetailPage.GetDateTimeRestaurantFromLocal(menuOrderingAvailableSlot.StartTime.Time);
						if (minDate > RestaurantDetailPage.SelectedDate)
						{
							RestaurantDetailPage.MenuOrderingAvailables = MenuOrderingAvailables;
							RestaurantDetailPage.SelectedDate = minDate;
							RestaurantDetailPage.IsDeliverySelected = IsDeliverySelected;
							SelectedDate = RestaurantDetailPage.SelectedDate;
							var timeSelectedIndex = PickerTime.Items.ToList().FindIndex(t => t == SelectedDate.ToString("h:mm tt"));
							PickerTime.SelectedIndex = timeSelectedIndex;
							if (CheckCurrentPage())
							{
								Utils.ShowSuccessMessage((RestaurantDetailPage.IsDeliverySelected ? AppResources.Delivery : AppResources.Pickup)
														 + " " + AppResources.TimeChanged + " " + RestaurantDetailPage.SelectedDate.ToString("h:mm tt") + ".", AppResources.Restaurant, 5);
							}
						}
					}
					else
					{
						CloseOrder();
						return;
					}
				}

				var menuIds = RestaurantDetailPage.ListMenuItemViews.Values.SelectMany(t => t)
												   .SelectMany(t => t.ListMenuGroupModifierItemSelected)
												  .Select(s => s.MenuItemView.RestaurantMenuItem.MenuId).Distinct().ToList();
				foreach (var menuId in menuIds)
				{
					var minutes = RestaurantDetailPage.SearchAvailableOrderTimes(menuId, false);
					if (minutes <= 0)
					{
						CloseOrder();
						break;
					}
					else if (minutes > 0 && minutes <= 5)
					{
						RestaurantDetailPage.SearchAvailableOrderTimes(menuId, true);
						break;
					}
				}
			}
		}

		public void UpdateTimeSource()
		{
			var currentTime = RestaurantDetailPage.GetDateTimeRestaurantFromUtc(DateTime.UtcNow);
			var menuIds = RestaurantDetailPage.ListMenuItemViews.Values.SelectMany(t => t)
															  .SelectMany(t => t.ListMenuGroupModifierItemSelected)
											 				  .Select(s => s.MenuItemView.RestaurantMenuItem.MenuId).Distinct().ToList();

			var dateMenuOrderingAvailables = MenuOrderingAvailables
								.Where(s => RestaurantDetailPage.GetDateTimeRestaurantFromLocal(s.CutoffTime.Time).Date == SelectedDate.Date)
								.Where(s => RestaurantDetailPage.GetDateTimeRestaurantFromLocal(s.CutoffTime.Time) > currentTime)
								.ToList();

			Times = dateMenuOrderingAvailables
					.Where(s =>
					{
						var menuAvailableIDs = dateMenuOrderingAvailables
									.Where(ma => ma.StartTime.Time.Equals(s.StartTime.Time))
									.SelectMany(ma => ma.RelatedMenuIDs)
									.ToList();

						return !menuIds.Except(menuAvailableIDs).Any();
					})
					//.Where(s => RestaurantDetailPage.GetDateTimeRestaurantFromLocal(s.CutoffTime.Time).Date == SelectedDate.Date)
					//.Where(s => RestaurantDetailPage.GetDateTimeRestaurantFromLocal(s.CutoffTime.Time) > currentTime)
					.Select(menuOrderingAvailable =>
					{
						string timeString = menuOrderingAvailable.StartTime.Time;
						return RestaurantDetailPage.GetDateTimeRestaurantFromLocal(timeString);
					}).ToList();
			var itemTimes = Times.Select(t => t.ToString("h:mm tt")).Distinct().ToList();

			try
			{
				PickerTime.SelectedIndex = -1;
				foreach (var item in PickerTime.Items.ToList())
				{
					PickerTime.Items.Remove(item);
				}
			}
			catch (Exception) { }

			foreach (var itemTime in itemTimes)
			{
				PickerTime.Items.Add(itemTime);
			}

			var timeSelectedIndex = itemTimes.FindIndex(t => t == SelectedDate.ToString("h:mm tt"));
			if (timeSelectedIndex >= 0)
			{
				PickerTime.SelectedIndex = timeSelectedIndex;
			}
			else
			{
				if (Dates.Count > 0 && PickerDate.SelectedIndex >= 0)
				{
					var dateSelected = Dates[PickerDate.SelectedIndex].Date;

					if (itemTimes.Count > 0)
					{
						PickerTime.SelectedIndex = 0;
						var time = DateTime.ParseExact(itemTimes[0], "h:mm tt", null);
						dateSelected = dateSelected.AddHours(time.Hour).AddMinutes(time.Minute);
					}

					SelectedDate = dateSelected;
					RestaurantDetailPage.SelectedDate = dateSelected;
				}
			}
		}
	}
}
