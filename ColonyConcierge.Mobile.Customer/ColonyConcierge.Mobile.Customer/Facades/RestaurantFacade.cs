using System;
using System.Collections.Generic;
using System.Linq;
using ColonyConcierge.APIData.Data;
using ColonyConcierge.Mobile.Customer.Localization.Resx;
using Xamarin.Forms;
using XLabs;

namespace ColonyConcierge.Mobile.Customer
{
	public class RestaurantVM : BindableObject
	{
		public object ForceUpdateSize { get; set; }

		public int LocationId { get; set; }
		public RestaurantLocation Location { get; set; }
		public bool Closed { get; set; } = false;
		public string Address { get; set; }
		public string LogoURL { get; set; }
		public int RestaurantID { get; set; }
		public List<RMenuVM> Menus { get; set; }
		public RestaurantSearchResultData SearchResultData { get; set; }
		public Address AddressObj { get; set; }
		public string labelTimeZone {get; set;}
        public string RestaurantTitle 
        { 
            get
            {
                return SearchResultData.RestaurantDisplayName + " - " + Location.DisplayName;
            }
        }

		private bool mIsSelected = false;
		public bool IsSelected
		{
			get
			{
				return mIsSelected;
			}
			set
			{
				OnPropertyChanging(nameof(IsSelected));
				mIsSelected = value;
				OnPropertyChanged(nameof(IsSelected));
				OnPropertyChanged(nameof(BackgroundColor));
			}
		}

		public Color BackgroundColor
		{
			get
			{
				return IsSelected ? AppearanceBase.Instance.OrangeColor : AppearanceBase.Instance.LightGray;
			}
		}

		public bool ClosedMessageVisible
		{
			get
			{
				return !string.IsNullOrEmpty(ClosedMessage);
			}
		}

		public bool IsNextDeliveryTime
		{
			get
			{
				if (SearchResultData != null)
				{
					var appServices = DependencyService.Get<IAppServices>();
					var restaurantTimeZone = appServices.GetTimeZoneById(Location.TimeZone);
					var currentTime = DateTime.UtcNow.AddMilliseconds(restaurantTimeZone.GetUtcOffset(DateTime.UtcNow).TotalMilliseconds);
					if (SearchResultData.IsOpenForDelivery)
					{
						if (SearchResultData.NextDeliveryCutoff != null)
						{
							var nextDeliveryTime = TimeZoneInfo.ConvertTime(DateTime.Parse(SearchResultData.NextDeliveryCutoff.Time), TimeZoneInfo.Utc);
							nextDeliveryTime = nextDeliveryTime.AddMilliseconds(restaurantTimeZone.GetUtcOffset(nextDeliveryTime).TotalMilliseconds);
							return nextDeliveryTime.AddMinutes(-120) >= currentTime;
						}
						return SearchResultData.NextDeliveryTime != null;
					}
					else
					{
						if (SearchResultData.NextDeliveryTime == null && SearchResultData.NextPickupTime == null)
						{
							return false;
						}
						else if (SearchResultData.NextDeliveryTime == null ||
						         (SearchResultData.IsPickupAvailable && SearchResultData.NextDeliveryTime > SearchResultData.NextPickupTime) ||
									!SearchResultData.IsDeliveryAvailable)
						{
							return false;
						}
						else
						{
							if (SearchResultData.NextDeliveryCutoff != null)
							{
								var nextDeliveryTime = TimeZoneInfo.ConvertTime(DateTime.Parse(SearchResultData.NextDeliveryCutoff.Time), TimeZoneInfo.Utc);
								nextDeliveryTime = nextDeliveryTime.AddMilliseconds(restaurantTimeZone.GetUtcOffset(nextDeliveryTime).TotalMilliseconds);
								return nextDeliveryTime.AddMinutes(-120) >= currentTime;
							}
							return SearchResultData.NextDeliveryTime != null;
						}
					}
				}
				return false;
			}
		}

		public bool IsNextPickupTime
		{
			get
			{
				if (SearchResultData != null)
				{
					var appServices = DependencyService.Get<IAppServices>();
					var restaurantTimeZone = appServices.GetTimeZoneById(Location.TimeZone);
					var currentTime = DateTime.UtcNow.AddMilliseconds(restaurantTimeZone.GetUtcOffset(DateTime.UtcNow).TotalMilliseconds);
					if (SearchResultData.IsOpenForDelivery)
					{
						return false;
					}
					else
					{
						if (SearchResultData.NextDeliveryTime == null && SearchResultData.NextPickupTime == null)
						{
							return true;
						}
						else if (SearchResultData.NextDeliveryTime == null ||
						         (SearchResultData.IsPickupAvailable && SearchResultData.NextDeliveryTime > SearchResultData.NextPickupTime) ||
									!SearchResultData.IsDeliveryAvailable)
						{
							if (SearchResultData.NextPickupCutoff != null)
							{
								var nextPickupTime = TimeZoneInfo.ConvertTime(DateTime.Parse(SearchResultData.NextPickupCutoff.Time), TimeZoneInfo.Utc);
								nextPickupTime = nextPickupTime.AddMilliseconds(restaurantTimeZone.GetUtcOffset(nextPickupTime).TotalMilliseconds);
								return nextPickupTime.AddMinutes(-120) >= currentTime;
							}

							return SearchResultData.NextPickupTime != null;
						}
						else
						{
							return false;
						}
					}
				}
				return false;
			}
		}

		public string ClosedMessage
		{
			get
			{
				if (SearchResultData != null)
				{
					var appServices = DependencyService.Get<IAppServices>();
					var restaurantTimeZone = appServices.GetTimeZoneById(Location.TimeZone);
					var currentTime = DateTime.UtcNow.AddMilliseconds(restaurantTimeZone.GetUtcOffset(DateTime.UtcNow).TotalMilliseconds);

					if (SearchResultData.IsOpenForDelivery)
					{
						if (SearchResultData.NextDeliveryTime != null && SearchResultData.NextDeliveryCutoff != null)
						{
							var nextDeliveryTime = TimeZoneInfo.ConvertTime(DateTime.Parse(SearchResultData.NextDeliveryTime.Time), TimeZoneInfo.Utc);
							nextDeliveryTime = nextDeliveryTime.AddMilliseconds(restaurantTimeZone.GetUtcOffset(nextDeliveryTime).TotalMilliseconds);
							var nextDeliveryCutOff = TimeZoneInfo.ConvertTime(DateTime.Parse(SearchResultData.NextDeliveryCutoff.Time), TimeZoneInfo.Utc);
							nextDeliveryCutOff = nextDeliveryCutOff.AddMilliseconds(restaurantTimeZone.GetUtcOffset(nextDeliveryCutOff).TotalMilliseconds);
							if (nextDeliveryTime.Date > currentTime.Date)
							{
								return AppResources.NoMoreDeliveriesToday;
							}
							else if (nextDeliveryCutOff.AddMinutes(-120) >= currentTime)
							{
								return AppResources.PreorderOnlyDeliveryMessage + " " +
									 nextDeliveryTime.ToString("ddd, MMM, dd") + " at " + nextDeliveryTime.ToString("h:mm tt");
							}
						}
					}
					else
					{
						if (SearchResultData.NextDeliveryTime == null && SearchResultData.NextPickupTime == null)
						{
							return AppResources.CloseAtThisTime;
						}
						else if (SearchResultData.NextDeliveryTime == null ||
						         (SearchResultData.IsPickupAvailable && SearchResultData.NextDeliveryTime > SearchResultData.NextPickupTime) ||
						   			 !SearchResultData.IsDeliveryAvailable)
						{
							if (SearchResultData.NextPickupTime != null && SearchResultData.NextPickupCutoff != null)
							{
								var nextPickupTime = TimeZoneInfo.ConvertTime(DateTime.Parse(SearchResultData.NextPickupTime.Time), TimeZoneInfo.Utc);
								nextPickupTime = nextPickupTime.AddMilliseconds(restaurantTimeZone.GetUtcOffset(nextPickupTime).TotalMilliseconds);
								var nextPickupCutOff = TimeZoneInfo.ConvertTime(DateTime.Parse(SearchResultData.NextPickupCutoff.Time), TimeZoneInfo.Utc);
								nextPickupCutOff = nextPickupCutOff.AddMilliseconds(restaurantTimeZone.GetUtcOffset(nextPickupCutOff).TotalMilliseconds);
								if (nextPickupCutOff.AddMinutes(-120) >= currentTime)
								{
									return AppResources.PreorderOnlyPickupMessage + " " +
										nextPickupTime.ToString("ddd, MMM, dd") + " at " + nextPickupTime.ToString("h:mm tt");
								}
							}
						}
						else
						{
							if (SearchResultData.NextDeliveryTime != null && SearchResultData.NextDeliveryCutoff != null)
							{
								var nextDeliveryTime = TimeZoneInfo.ConvertTime(DateTime.Parse(SearchResultData.NextDeliveryTime.Time), TimeZoneInfo.Utc);
								nextDeliveryTime = nextDeliveryTime.AddMilliseconds(restaurantTimeZone.GetUtcOffset(nextDeliveryTime).TotalMilliseconds);
								var nextDeliveryCutOff = TimeZoneInfo.ConvertTime(DateTime.Parse(SearchResultData.NextDeliveryCutoff.Time), TimeZoneInfo.Utc);
								nextDeliveryCutOff = nextDeliveryCutOff.AddMilliseconds(restaurantTimeZone.GetUtcOffset(nextDeliveryCutOff).TotalMilliseconds);
								if (nextDeliveryTime.Date > currentTime.Date)
								{
									return AppResources.NoMoreDeliveriesToday;
								}
								else if (nextDeliveryCutOff.AddMinutes(-120) >= currentTime)
								{
									return AppResources.PreorderOnlyDeliveryMessage + " " +
										 nextDeliveryTime.ToString("ddd, MMM, dd") + " at " + nextDeliveryTime.ToString("h:mm tt");
								}
							}
						}
					}
				}
				return string.Empty;
			}
		}

		public string CategoriesText
		{
			get
			{
				string categories = string.Empty;
				if (SearchResultData != null && SearchResultData.ResaurantCategories != null)
				{
					foreach (var category in SearchResultData.ResaurantCategories)
					{
						if (!string.IsNullOrEmpty(categories))
						{
							categories += ", ";
						}
						categories += category.DisplayName;
					}
				}
				return categories;
			}
		}
	}

	public class RMenuVM
	{
		public RMenu Menu { get; set; }
		public bool IsAvailable { get; set; } = true;
		public List<RMenuGroupVM> MenuGroups { get; set; }

	}
	public class RMenuGroupVM
	{
		public RMenuGroup Group { get; set; }
		public List<MenuItemVM> MenuItems { get; set; }
	}
	public class MenuItemVM
	{
		public List<decimal?> ModifierPrices { get; set; }
		public decimal? MenuItemOriginalPrice { get; set; }
		public RMenuItem MenuItem { get; set; }
	}
	public class RMenuGroupModifierVM
	{
		public RMenuModifierGroup MenuModifierGroup { get; set; }
		public List<RMenuModifierVM> MenuModifiers { get; set; }
	}
	public class RMenuModifierVM
	{
		public decimal? ModifierPrice { get; set; }
		public bool Additive { get; set; } = true;
		public bool IsSelected { get; set; } = false;
		public int Quantity { get; set; } = 1;
		public RMenuModifier MenuModifier { get; set; }
		public List<RMenuModifierGroup> SubMenuModifierGroups { get; set; }
		public List<RMenuModifierVM> SubMenuModifiers { get; set; }
	}

	public class RMenuItemVM
	{
		public int LocationId { get; set; }
		public int MenuId { get; set; }
		public RMenuItem MenuItem { get; set; }
	}

	public class RestaurantFacade
	{
		public static readonly IReadOnlyDictionary<string, string> DeliveryStatus = new Dictionary<string, string>()
		{
			{"Invalid", ""}, {"InException", ""},
			{"Entered", "Entered"}, {"Processing", "Being Delivered"}, {"Approved", "Approved"},
			{"Rejected", "Rejected"}, {"Canceled", "Canceled"}, {"Pending", "Pending"}, {"InProgress", "In Progress"},
			{"Complete", "Complete"}, {"AwaitingApproval", "Awaiting Approval"}, {"InReview", "Being Reviewed"},
			{"InEarlyReview", "Being Reviewed"}, {"AwaitingCharges", "Processing Payment"},
			{"AwaitingPayment", "Processing Payment"}, {"AwaitingDispatch", "Prepping for Delivery"},
			{"AwaitingReport", "Wating on Update"}, {"StaleNeverApproved", "Awaiting Approval"}, 
			{"PaymentFailed", "Payment Declined"}, {"AwaitingJobCreation", "Payment Completed"}, {"AwaitingJobAssignment", "Payment Completed"}
		};

		public static List<RestaurantVM> restaurants = new List<RestaurantVM>();

		public List<DateTime> GetDateRestaurantCloses(TimeZoneInfo restaurantTimeZone, int locid, DateTime startDate, DateTime endDate)
		{
			var dateRestaurantCloses = new List<DateTime>();
			if (locid > 0)
			{
				var location = Shared.APIs.IRestaurant.GetLocation(locid);
				var holidays = Shared.APIs.IRestaurant.GetLocationHolidays(locid, startDate.ToString("MM/dd/yyyy"), endDate.ToString("MM/dd/yyyy"));
				var holidayDays = new List<DateTime>();
				if (holidays != null)
				{
					holidayDays = holidays.SelectMany((t) =>
					{
						var dateTimes = new List<DateTime>(); 
						var dateTimeUTC = TimeZoneInfo.ConvertTime(DateTime.Parse(t.StartTime.Time), TimeZoneInfo.Utc);
						var dateTimeZone = dateTimeUTC.AddMilliseconds(restaurantTimeZone.GetUtcOffset(dateTimeUTC).TotalMilliseconds);
						var endDateTimeUTC = TimeZoneInfo.ConvertTime(DateTime.Parse(t.EndTime.Time), TimeZoneInfo.Utc);
						var endDateTimeZone = endDateTimeUTC.AddMilliseconds(restaurantTimeZone.GetUtcOffset(endDateTimeUTC).TotalMilliseconds);
						int d = 0;  
						while (dateTimeZone.AddDays(d).Date <= endDateTimeZone.Date)
						{
							dateTimes.Add(dateTimeZone.AddDays(d).Date);
							d++;
						}
						return dateTimes;
					}).Distinct().ToList();
				}
				int i = 0;  
				while (startDate.AddDays(i).Date <= endDate.Date)
				{
					var date = startDate.AddDays(i).Date;
					if (holidayDays.Contains(date))
					{
						dateRestaurantCloses.Add(date);
					}
					else if (!location.DaysOfWeekOpen.ToString().Contains(date.DayOfWeek.ToString()))
					{
						dateRestaurantCloses.Add(date);
					}
					i++;
				}

			}
			return dateRestaurantCloses;
		}

		public string SearchHoliday(int locid, DateTime selectedDate)
		{
			if (locid > 0)
			{
				var dayOfWeek = selectedDate.DayOfWeek.ToString();
				var location = Shared.APIs.IRestaurant.GetLocation(locid);
				var holidays = Shared.APIs.IRestaurant.GetLocationHolidays(locid, selectedDate.ToString("MM/dd/yyyy"), selectedDate.ToString("MM/dd/yyyy"));
				if (holidays.Count > 0)
					return "Holiday";
				else if (!location.DaysOfWeekOpen.ToString().Contains(dayOfWeek))
					return "Closed";
			}
			return "";
		}

		public List<RMenuOrderingAvailableSlot> RefreshMenus(int locid, DateTime startDate, bool isDelivery, GroupedDeliveryDestination groupedDeliveryDestination = null)
		{
			return RefreshMenus(locid, startDate, startDate, isDelivery, groupedDeliveryDestination);
		}

		public List<RMenuOrderingAvailableSlot> RefreshMenus(int locid, DateTime startDate, DateTime endDate, bool isDelivery, GroupedDeliveryDestination groupedDeliveryDestination = null)
		{
			List<RMenuOrderingAvailableSlot> MenuOrderingAvailables = new List<RMenuOrderingAvailableSlot>();

			if (locid > 0)
			{
				var menus = Shared.APIs.IRestaurant.GetMenus(locid);
				return RefreshMenus(menus, locid, startDate, endDate, isDelivery);
			}
			return MenuOrderingAvailables;
		}

		public List<RMenuOrderingAvailableSlot> RefreshMenus(List<RMenu> menus, int locid, DateTime dt, DateTime dtEnd, bool isDelivery, GroupedDeliveryDestination groupedDeliveryDestination = null)
		{
			List<RMenuOrderingAvailableSlot> MenuOrderingAvailables = new List<RMenuOrderingAvailableSlot>();
			var orderDate = dt.ToString("yyyy-MM-dd HH:mm:ssZ");//string.Format("{0}-{1}-{2} hh:mm:ss", dt.Year, dt.Month, dt.Day);
			var orderDateEnd = string.Format("{0}-{1}-{2}", dtEnd.Year, dtEnd.Month, dtEnd.Day);

			List<ColonyConcierge.APIData.Data.RMenuOrderingAvailableSlot> availableTimes = new List<RMenuOrderingAvailableSlot>();
			if (groupedDeliveryDestination == null)
			{
				availableTimes = Shared.APIs.IRestaurant.GetLocationAvailableOrderTimes(locid, orderDate, orderDateEnd);
			}
			else
			{
				//foreach (var menu in menus)
				{
					var availableTimesMenu = Shared.APIs.IRestaurant.GetLocationAvailableOrderTimes(locid, orderDate, orderDateEnd, groupedDeliveryDestination.ID);
					if (availableTimesMenu != null)
					{
						availableTimes.AddRange(availableTimesMenu);
					}
				}
			}
			if (availableTimes != null)
			{
				MenuOrderingAvailables.AddRange(availableTimes.Where(w => w.IsDelivery == isDelivery));
			}
			//foreach (var menu in menus)
			//{
			//	var availableTimes = Shared.APIs.IRestaurant.GetAvailableOrderTimes(locid, menu.ID, orderDate, orderDateEnd);
			//	if (availableTimes != null)
			//	{
			//		foreach (var availableTime in availableTimes)
			//		{
			//			availableTime.RelatedLocationID = menu.ID;
			//		}
			//		MenuOrderingAvailables.AddRange(availableTimes.Where(w => w.IsDelivery == isDelivery));
			//	}
			//}

			return MenuOrderingAvailables;
		}

		public List<RestaurantVM> GetRestaurantByAddress(object lat, object lon, object for_delivery = null, object filter_zip = null, object pickup_radius_meters = null, GroupedDeliveryDestination groupedDeliveryDestination = null)
		{
			List<RestaurantVM> result = new List<RestaurantVM>();

			List<ColonyConcierge.APIData.Data.RestaurantLocation> locations = new List<RestaurantLocation>();
			if (groupedDeliveryDestination != null)
			{
				locations = Shared.APIs.IRestaurant.FindLocationsByLocation(lat, lon, true, filter_zip, 0, groupedDeliveryDestination.ID);
			}
			else
			{
				locations = Shared.APIs.IRestaurant.FindLocationsByLocation(lat, lon, true, filter_zip, -1);
			}
			//var locations = Shared.APIs.IRestaurant.FindLocations(Shared.LocalAddress.BasicAddress.ToAddress(), true, filter_zip, -1);

			foreach (var location in locations)
			{
				RestaurantVM restaurant = new RestaurantVM();
				restaurant.LocationId = location.ID;
				restaurant.AddressObj = location.Address;
				restaurant.Address = location.Address.Line1 + " " + location.Address.City + ", " + location.Address.StateProv + " " + location.Address.ZipCode;
				restaurant.Address = restaurant.Address.Replace(".", "").Replace(" ", "-").Replace("#", "");
				restaurant.RestaurantID = location.RestaurantID;
				restaurant.SearchResultData = location.SearchResultData;
				restaurant.Location = location;
				restaurant.labelTimeZone = location.TimeZone;

				if (!string.IsNullOrEmpty(location.LogoUrl))
				{
					restaurant.LogoURL = location.LogoUrl;
				}
				else
				{
					restaurant.LogoURL = "";
				}


				result.Add(restaurant);
			}

			restaurants = result;
			return result;
		}

		public List<RMenuGroupModifierVM> GetMenuItemModifiers(RMenuItemVM model)
		{
			List<RMenuGroupModifierVM> GroupModeifierVMList = new List<RMenuGroupModifierVM>();
			List<RMenuModifierPrice> ModifierPriceList = new List<RMenuModifierPrice>();

			//Add menu item modifier Price list  
			if (model.MenuItem.ModifierPriceIDs != null && model.MenuItem.ModifierPriceIDs.Count > 0)
			{
				foreach (var modifierPrice in model.MenuItem.ModifierPriceIDs)
				{
					var pricemodel = Shared.APIs.IRestaurant.GetModifierPrice(model.LocationId, model.MenuId, model.MenuItem.ID, modifierPrice);
					ModifierPriceList.Add(pricemodel);
				}
			}

			//Filter and add menu item modifiers
			if (model.MenuItem.ModifierGroupIDs != null && model.MenuItem.ModifierGroupIDs.Count > 0)
			{
				foreach (var modifier in model.MenuItem.ModifierGroupIDs)
				{
					RMenuGroupModifierVM GroupModeifierVM = new RMenuGroupModifierVM();
					List<RMenuModifierVM> menuModifierVMList = new List<RMenuModifierVM>();
					GroupModeifierVM.MenuModifierGroup = Shared.APIs.IRestaurant.GetMenuModifierGroup(model.LocationId, model.MenuId, modifier);
					var GroupModifiers = Shared.APIs.IRestaurant.GetMenuModifiers(model.LocationId, model.MenuId, modifier);
					foreach (var GroupModifier in GroupModifiers)
					{
						RMenuModifierVM menuModifierVM = new RMenuModifierVM();
						menuModifierVM.MenuModifier = GroupModifier;
						menuModifierVM.IsSelected = GroupModifier.ApplyByDefault;
						menuModifierVM.SubMenuModifiers = new List<RMenuModifierVM>();
						menuModifierVM.SubMenuModifierGroups = new List<RMenuModifierGroup>();
						foreach (var submodifierGroup in GroupModifier.SubmodifierGroupIDs)
						{
							menuModifierVM.SubMenuModifierGroups.Add(Shared.APIs.IRestaurant.GetMenuModifierGroup(model.LocationId, model.MenuId, submodifierGroup));
							menuModifierVM.SubMenuModifiers.AddRange(Shared.APIs.IRestaurant.GetMenuModifiers(model.LocationId, model.MenuId, submodifierGroup).Select(s => new RMenuModifierVM { MenuModifier = s, IsSelected = s.ApplyByDefault }));
						}

						var modifierPrice = ModifierPriceList.Where(w => w.ModifierID == GroupModifier.ID).FirstOrDefault();
						if (modifierPrice != null)
						{
							menuModifierVM.ModifierPrice = modifierPrice.Price;
							menuModifierVM.Additive = modifierPrice.Additive;
						}
						else
						{
							menuModifierVM.ModifierPrice = GroupModifier.Price;
						}
						menuModifierVMList.Add(menuModifierVM);
					}

					//Add menu item modifiers
					GroupModeifierVM.MenuModifiers = menuModifierVMList;
					GroupModeifierVMList.Add(GroupModeifierVM);
				}
			}

			return GroupModeifierVMList;
		}

		//public List<RMenuGroupModifierVM> GetMenuItemModifiers(RMenuItemVM model)
		//{
		//	List<RMenuGroupModifierVM> result = new List<RMenuGroupModifierVM>();
		//	List<RMenuModifierPrice> ModifierPriceList = new List<RMenuModifierPrice>();

		//	//Add menu item modifier Price list  
		//	if (model.MenuItem.ModifierPriceIDs != null && model.MenuItem.ModifierPriceIDs.Count > 0)
		//	{
		//		foreach (var modifierPrice in model.MenuItem.ModifierPriceIDs)
		//		{
		//			var pricemodel = Shared.APIs.IRestaurant.GetModifierPrice(model.LocationId, model.MenuId, model.MenuItem.ID, modifierPrice);
		//			ModifierPriceList.Add(pricemodel);
		//		}
		//	}

		//	//Filter and add menu item modifiers
		//	if (model.MenuItem.ModifierGroupIDs != null && model.MenuItem.ModifierGroupIDs.Count > 0)
		//	{
		//		foreach (var modifier in model.MenuItem.ModifierGroupIDs)
		//		{
		//			RMenuGroupModifierVM GroupModeifierVM = new RMenuGroupModifierVM();
		//			List<RMenuModifierVM> menuModifierVMList = new List<RMenuModifierVM>();
		//			GroupModeifierVM.MenuModifierGroup = Shared.APIs.IRestaurant.GetMenuModifierGroup(model.LocationId, model.MenuId, modifier);
		//			var GroupModifiers = Shared.APIs.IRestaurant.GetMenuModifiers(model.LocationId, model.MenuId, modifier);
		//			foreach (var GroupModifier in GroupModifiers)
		//			{
		//				RMenuModifierVM menuModifierVM = new RMenuModifierVM();
		//				menuModifierVM.MenuModifier = GroupModifier;
		//				menuModifierVM.IsSelected = GroupModifier.ApplyByDefault;
		//				menuModifierVM.SubMenuModifiers = new List<RMenuModifierVM>();
		//				foreach (var SubmodifierGroup in GroupModifier.SubmodifierGroupIDs)
		//				{
		//					menuModifierVM.SubMenuModifierGroup = Shared.APIs.IRestaurant.GetMenuModifierGroup(model.LocationId, model.MenuId, SubmodifierGroup);
		//					menuModifierVM.SubMenuModifiers.AddRange(Shared.APIs.IRestaurant.GetMenuModifiers(model.LocationId, model.MenuId, SubmodifierGroup).Select(s => new RMenuModifierVM { MenuModifier = s, IsSelected = s.ApplyByDefault }));
		//				}
		//				var modifierPrice = ModifierPriceList.Where(w => w.ModifierID == GroupModifier.ID).FirstOrDefault();
		//				if (modifierPrice != null)
		//				{
		//					menuModifierVM.ModifierPrice = modifierPrice.Price;
		//					menuModifierVM.Additive = modifierPrice.Additive;
		//				}
		//				else
		//				{
		//					menuModifierVM.ModifierPrice = GroupModifier.Price;
		//				}
		//				menuModifierVMList.Add(menuModifierVM);
		//			}

		//			//Add menu item modifiers
		//			GroupModeifierVM.MenuModifiers = menuModifierVMList;
		//			result.Add(GroupModeifierVM);
		//		}
		//	}
		//	return result;
		//}

		public List<RMenuOrderingAvailableSlot> SearchAvailableOrderTimes(int locid, DateTime startDate, bool isDelivery, GroupedDeliveryDestination groupedDeliveryDestination = null)
		{
			return SearchAvailableOrderTimes(locid, startDate, startDate, isDelivery, groupedDeliveryDestination);
		}

		public List<RMenuOrderingAvailableSlot> SearchAvailableOrderTimes(int locid, DateTime dt, DateTime dtEnd, bool isDelivery, GroupedDeliveryDestination groupedDeliveryDestination = null)
		{
			if (locid > 0)
			{
				//var location = Shared.APIs.IRestaurant.GetLocation(locid);
				//var menus = Shared.APIs.IRestaurant.GetMenus(locid);
				List<RMenuOrderingAvailableSlot> MenuOrderingAvailables = new List<RMenuOrderingAvailableSlot>();

				var orderDate = dt.ToString("yyyy-MM-dd HH:mm:ssZ");// string.Format("{0}-{1}-{2} hh:mm:ss", dt.Year, dt.Month, dt.Day);
				//var orderDateEnd = string.Format("{0}-{1}-{2}", dtEnd.Year, dtEnd.Month, dtEnd.Day);
				var orderDateEnd = dtEnd.ToString("yyyy-MM-dd HH:mm:ssZ");// string.Format("{0}-{1}-{2} hh:mm:ss", dt.Year, dt.Month, dt.Day);


				List<ColonyConcierge.APIData.Data.RMenuOrderingAvailableSlot> availableTimes = new List<RMenuOrderingAvailableSlot>();
				if (groupedDeliveryDestination == null)
				{
					availableTimes = Shared.APIs.IRestaurant.GetLocationAvailableOrderTimes(locid, orderDate, orderDateEnd);
				}
				else
				{
					//foreach (var menu in menus)
					{
						var availableTimesMenu = Shared.APIs.IRestaurant.GetLocationAvailableOrderTimes(locid, orderDate, orderDateEnd, groupedDeliveryDestination.ID);
						if (availableTimesMenu != null)
						{
							availableTimes.AddRange(availableTimesMenu);
						}
					}
				}

				if (availableTimes != null)
				{
					MenuOrderingAvailables.AddRange(availableTimes.Where(w => w.IsDelivery == isDelivery));
				}

				//foreach (var menu in menus)
				//{
				//	var availableTimes = Shared.APIs.IRestaurant.GetAvailableOrderTimes(locid, menu.ID, orderDate, orderDateEnd);
				//	if (availableTimes != null)
				//	{
				//		MenuOrderingAvailables.AddRange(availableTimes.Where(w => w.IsDelivery == isDelivery));
				//	}
				//	//MenuOrderingAvailables.AddRange(availableTimes.Where(w => !MenuOrderingAvailables.Select(s => s.StartTime).Contains(w.StartTime)));
				//}

				return SearchAvailableOrderTimes(MenuOrderingAvailables);

				//if (MenuOrderingAvailables.Count > 0)
				//{
				//	// return Ok(MenuOrderingAvailables.Select(s => DateTime.Parse(s.StartTime.Time).ToUniversalTime().AddMinutes(s.LeadTimeMinutes).ToString("hh:mm tt")).Distinct());
				//	//return Ok(MenuOrderingAvailables.GroupBy(x => x.StartTime.Time).Select(s => new System.Web.Mvc.SelectListItem { Text = s.First().StartTime.Time, Value = s.First().ID.ToString() }).Distinct());
				//	return MenuOrderingAvailables.GroupBy(x => x.StartTime.Time).Select(s => s.First()).ToList();
				//}
			}
			return new List<RMenuOrderingAvailableSlot>();
		}


		public List<RMenuOrderingAvailableSlot> SearchAvailableOrderTimes(List<RMenuOrderingAvailableSlot> refreshMenuAvailables)
		{
			if (refreshMenuAvailables != null && refreshMenuAvailables.Count > 0)
			{
				return refreshMenuAvailables.GroupBy(x => x.StartTime.Time).Select(s => s.First()).ToList();
			}
			else 
			{
				return new List<RMenuOrderingAvailableSlot>();
			}
		}

		public RestaurantVM GetMenus(string address, int locid, string date = null)
		{
			var locationAddress = address.Replace("-", " ");

			DateTime selectedDate = Convert.ToDateTime(date);
			var dayOfWeek = selectedDate.DayOfWeek.ToString();
			var holidays = Shared.APIs.IRestaurant.GetLocationHolidays(locid, date, date);

			//Add Restaurant in RestaurantVM
			RestaurantVM result = new RestaurantVM();
			result.LocationId = locid;

			var restaurant = new RestaurantVM();
			foreach (var res in restaurants)
			{
				if (res.LocationId == locid)
				{
					restaurant = res;
				}
			}

			//a tempararaly work around for restaurant logos.
			if (!string.IsNullOrEmpty(restaurant.LogoURL))
			{
				result.LogoURL = restaurant.LogoURL;
			}
			else
			{
				result.LogoURL = "";
			}


			result.SearchResultData = restaurant.SearchResultData;
			result.AddressObj = restaurant.AddressObj;

			result.Address = locationAddress;
			if (holidays.Count > 0 || !restaurant.Location.DaysOfWeekOpen.ToString().Contains(dayOfWeek))
				result.Closed = true;


			//Add Menus of Restaurant in MenuVM
			List<RMenuVM> Menus = new List<RMenuVM>();
			var menus = Shared.APIs.IRestaurant.GetMenus(locid);
			foreach (var menu in menus)
			{
				RMenuVM Menu = new RMenuVM();
				List<RMenuGroupVM> MenuGroups = new List<RMenuGroupVM>();
				var menusgroup = Shared.APIs.IRestaurant.GetMenuGroups(locid, menu.ID);
				foreach (var group in menusgroup)
				{
					var menuItems = Shared.APIs.IRestaurant.GetChildMenuItems(locid, menu.ID, group.ID);
					RMenuGroupVM GroupMenu = new RMenuGroupVM
					{
						Group = group,
						MenuItems = menuItems.Select(s => MapMenuItemBasePrice(new RMenuItemVM { LocationId = locid, MenuId = menu.ID, MenuItem = s })).ToList()
					};
					MenuGroups.Add(GroupMenu);
				}
				Menu.Menu = menu;
				Menu.MenuGroups = MenuGroups;
				Menus.Add(Menu);
			}
			result.Menus = Menus;

			return result;
		}

		public MenuItemVM MapMenuItemBasePrice(RMenuItemVM model)
		{
			List<decimal?> PriceList = new List<decimal?>();
			MenuItemVM menuItemVM = new MenuItemVM { MenuItemOriginalPrice = model.MenuItem.BasePrice };
			if (model.MenuItem.BasePrice == null)
			{
				if (model.MenuItem.ModifierPriceIDs != null && model.MenuItem.ModifierPriceIDs.Count > 0)
				{
					foreach (var modifierPrice in model.MenuItem.ModifierPriceIDs)
					{
						var pricemodel = Shared.APIs.IRestaurant.GetModifierPrice(model.LocationId, model.MenuId, model.MenuItem.ID, modifierPrice);

						PriceList.Add(pricemodel.Price);
					}
					model.MenuItem.BasePrice = PriceList.Where(f => f > 0).Min();
				}
			}

			menuItemVM.MenuItem = model.MenuItem;
			return menuItemVM;
		}

	}
}
