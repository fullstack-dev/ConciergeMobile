using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace ColonyConcierge.Mobile.Customer
{
	public class MenuItemViewModel : BindableObject
	{
		public object ForceUpdateSize 
		{
			get;
			set;
		}

		private List<MenuGroupModifierItem> mListMenuGroupModifierItemSelected = new List<MenuGroupModifierItem>();
		public List<MenuGroupModifierItem> ListMenuGroupModifierItemSelected
		{
			get
			{
				return mListMenuGroupModifierItemSelected;
			}
		}

		public bool IsMenuSection
		{
			get
			{
				return RestaurantMenuItem == null;
			}
		}
		public bool IsMenuItem 
		{
			get
			{
				return RestaurantMenuItem != null;
			}
		}

		public string DisplayName
		{
			get
			{
				if (RestaurantMenuItem != null)
				{
					return RestaurantMenuItem.DisplayName;
										//+ (string.IsNullOrEmpty(Description) ? string.Empty : "\n");
				}
				return string.Empty;
			}
		}

		public string Description
		{
			get
			{
				if (RestaurantMenuItem != null && !string.IsNullOrEmpty(RestaurantMenuItem.DetailedDescription))
				{
					return RestaurantMenuItem.DetailedDescription.Trim();
				}
				return string.Empty;
			}
		}

		public bool IsDescription
		{
			get
			{
				return !string.IsNullOrEmpty(Description);
			}
		}

		private bool mIsEnabled = true;
		public bool IsEnabled
		{
			get
			{
				return mIsEnabled;
			}
			set
			{
				OnPropertyChanging(nameof(IsEnabled));
				mIsEnabled = value;
				OnPropertyChanged(nameof(IsEnabled));
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
				NeedUpdateSize();
			}
		}

		public void NeedUpdateSize()
		{
            OnPropertyChanged(nameof(ForceUpdateSize));
		}

		public string SectionName 
		{
			get;
			set;
		}

		public string SectionDetail
		{
			get;
			set;
		}

		public RestaurantMenuItemViewModel RestaurantMenuItem 
		{ 
			get; 
			set; 
		}

		public MenuItemViewModel()
		{
		}
	}

	public class MenuGroupModifierItem : BindableObject
	{
		public MenuItemViewModel MenuItemView { get; set; }
		public int Quantity { get; set; } = 1;
		public List<RMenuGroupModifierVM> ListRMenuGroupModifierVM { get; set; } = new List<RMenuGroupModifierVM>();
		public string Comment { get; set; }

		private decimal mPrice = 0;
		public decimal Price
		{
			get
			{
				return mPrice;
			}
		}

		public string DisplayPrice
		{
			get
			{
				if (Price > 0) return "$" + Price.ToString ("0.00");
				return string.Empty;
			}
		}

		public void UpdatePrice()
		{
			var price = RestaurantMenuItemModifierPage.UpdatePrice(MenuItemView.RestaurantMenuItem.MenuItemVM, ListRMenuGroupModifierVM, this.Quantity);
			if (price.HasValue)
			{
				mPrice = price.Value;
			}
			else
			{
				mPrice = 0;
			}
			OnPropertyChanged(nameof(Price));
			OnPropertyChanged(nameof(DisplayPrice));
		}

		public void RaiseAllOnChanged()
		{
			OnPropertyChanged(nameof(MenuItemView));
			OnPropertyChanged(nameof(ListRMenuGroupModifierVM));
			OnPropertyChanged(nameof(Quantity));
			OnPropertyChanged(nameof(Comment));
			OnPropertyChanged(nameof(Price));
			OnPropertyChanged(nameof(DisplayPrice));
		}

		public MenuGroupModifierItem(MenuItemViewModel menuItemView, List<RMenuGroupModifierVM> listRMenuGroupModifierVM, int quantity, string comment ="")
		{
			MenuItemView = menuItemView;
			ListRMenuGroupModifierVM = listRMenuGroupModifierVM;
			Quantity = quantity;
			Comment = comment;
		}
	}
}
