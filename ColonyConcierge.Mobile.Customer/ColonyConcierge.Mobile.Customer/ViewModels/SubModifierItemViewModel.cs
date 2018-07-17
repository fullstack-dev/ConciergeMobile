using System;
using ColonyConcierge.APIData.Data;
using Xamarin.Forms;
using System.Linq;
using ColonyConcierge.Mobile.Customer.Localization.Resx;

namespace ColonyConcierge.Mobile.Customer
{
	public class SubMenuModifierItemViewModel : BindableObject
	{
		public object ForceUpdateSize { get; set; }
		public string Title { get; set; }
		public bool IsGroup
		{
			get
			{
				return SubMenuModifierVM == null;
			}
		}
		public bool IsItem
		{
			get
			{
				return !IsGroup;
			}
		}

		public ModifierItemViewModel ModifierItemView { get; set; }
		public RMenuModifierGroup SubMenuModifierGroup { get; set; }

		private RMenuModifierVM mSubMenuModifierVM;
		public RMenuModifierVM SubMenuModifierVM 
		{
			get
			{
				return mSubMenuModifierVM;
			}
			set
			{
				OnPropertyChanging(nameof(SubMenuModifierVM));
				mSubMenuModifierVM = value;
				OnPropertyChanged(nameof(SubMenuModifierVM));
				OnPropertyChanged(nameof(IsSelected));
				OnPropertyChanged(nameof(IsShowDetail));
			}
		}

		public string Price
		{
			get
			{
				if (mSubMenuModifierVM != null && mSubMenuModifierVM.MenuModifier != null
				    && mSubMenuModifierVM.MenuModifier.Price.HasValue)
				{
					return mSubMenuModifierVM.MenuModifier.Price.Value.ToString("0.00");
				}
				return null;
			}
		}

		public bool IsShowDetail
		{
			get
			{
				return SubMenuModifierVM != null ? !string.IsNullOrEmpty(SubMenuModifierVM.MenuModifier.DetailedDescription) : false;
			}
		}

		public bool IsAllowMultipleInstances
		{
			get
			{
				return IsSelected && (SubMenuModifierVM != null ? SubMenuModifierVM.MenuModifier.AllowMultipleInstances : false);
			}
		}

		private bool mIsSelected;
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
				if (!mIsSelected)
				{
					//Quantity = 1;
				}
				else
				{
					var itemsSelected = ModifierItemView.SubMenuModifierItemViews.Where(t => t.IsSelected).ToList();
					var countSelected = itemsSelected.Sum(t => t.Quantity);
					var maxApplied = SubMenuModifierGroup.MaxApplied;
					if (maxApplied.HasValue && countSelected - maxApplied > 0)
					{
						Quantity = Math.Max(1, Quantity - (countSelected - maxApplied.Value));
					}
					//OnPropertyChanged(nameof(ForceUpdateSize));
				}
				OnPropertyChanged(nameof(IsSelected));
				OnPropertyChanged(nameof(IsAllowMultipleInstances));
			}
		}

		private int mQuantity = 1;
		public int QuantityPrevious { get; set; } = 1;
		public int Quantity
		{
			get
			{
				return mQuantity;
			}
			set
			{
				var valueMax = Math.Max(1, value);

				OnPropertyChanging(nameof(Quantity));
				QuantityPrevious = mQuantity;
				mQuantity = valueMax;
				OnPropertyChanged(nameof(Quantity));
			}
		}

		public bool IsEnabled
		{
			get
			{
				if (ModifierItemView.MenuModifierVM != null 
				    && SubMenuModifierGroup != null 
				    && (SubMenuModifierGroup.MinApplied != 1 || SubMenuModifierGroup.MaxApplied != 1)
					&& !this.IsSelected)
				{
					var itemsSelected = ModifierItemView.SubMenuModifierItemViews.Where(t => t.IsSelected).ToList();
					var countSelected = itemsSelected.Sum(t => t.Quantity);
					if (SubMenuModifierGroup.MaxApplied.HasValue && SubMenuModifierGroup.MaxApplied <= countSelected)
					{
						return false;
					}
					return true;
				}
				return true;
			}
		}

		public bool IsEnabledAdd
		{
			get
			{
				if (ModifierItemView.MenuModifierVM != null
					&& SubMenuModifierGroup != null
					&& (SubMenuModifierGroup.MinApplied != 1 || SubMenuModifierGroup.MaxApplied != 1))
				{
					var itemsSelected = ModifierItemView.SubMenuModifierItemViews.Where(t => t.IsSelected).ToList();
					var countSelected = itemsSelected.Sum(t => t.Quantity);
					if (SubMenuModifierGroup.MaxApplied.HasValue && SubMenuModifierGroup.MaxApplied <= countSelected)
					{
						return false;
					}
					return true;
				}
				return true;
			}
		}

		public void Save()
		{
			if (SubMenuModifierVM != null)
			{
				SubMenuModifierVM.Quantity = Quantity;
				SubMenuModifierVM.IsSelected = IsSelected;
			}
		}

		public void RefreshEnabled()
		{
			OnPropertyChanged(nameof(IsEnabled));
			OnPropertyChanged(nameof(IsEnabledAdd));
		}

		public SubMenuModifierItemViewModel(ModifierItemViewModel modifierItemView, RMenuModifierGroup subMenuModifierGroup)
		{
			ModifierItemView = modifierItemView;
			SubMenuModifierGroup = subMenuModifierGroup;

			var minApplied = SubMenuModifierGroup.MinApplied;
			var maxApplied = SubMenuModifierGroup.MaxApplied;

			if (minApplied.HasValue && minApplied > 0)
			{
				if (maxApplied.HasValue && maxApplied != minApplied)
				{
					Title = string.Format(AppResources.ChooseRequired, minApplied + "-" + maxApplied + " " + SubMenuModifierGroup.DisplayName + " -");
				}
				else
				{
					Title = string.Format(AppResources.ChooseRequired, minApplied + " " + SubMenuModifierGroup.DisplayName + " -");
				}
			}
			else
			{
				if (maxApplied.HasValue && maxApplied > 0)
				{
					Title = string.Format(AppResources.ChooseOptional, "up to " + maxApplied + " " + SubMenuModifierGroup.DisplayName + " -");
				}
				else
				{
					Title = string.Format(AppResources.ChooseOptional, SubMenuModifierGroup.DisplayName + " -");
				}
			}
		}

		public SubMenuModifierItemViewModel(ModifierItemViewModel modifierItemView, RMenuModifierGroup subMenuModifierGroup, RMenuModifierVM subMenuModifier)
		{
			ModifierItemView = modifierItemView;
			SubMenuModifierVM = subMenuModifier;
			SubMenuModifierGroup = subMenuModifierGroup;

			Quantity = Math.Max(1, SubMenuModifierVM.Quantity);
			IsSelected = SubMenuModifierVM.IsSelected;

			if (!modifierItemView.IsSelected)
			{
				Quantity = 1;
				this.IsSelected = SubMenuModifierVM.MenuModifier.ApplyByDefault;
			}
		}
	}
}
