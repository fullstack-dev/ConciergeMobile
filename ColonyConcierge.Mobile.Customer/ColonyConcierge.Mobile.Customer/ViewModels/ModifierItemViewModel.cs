using System;
using Xamarin.Forms;
using System.Linq;
using System.Collections.Generic;
using ColonyConcierge.Mobile.Customer.Localization.Resx;

namespace ColonyConcierge.Mobile.Customer
{
	public class ModifierItemViewModel : BindableObject
	{
		public event EventHandler SaveChanged;

		public object ForceUpdateSize { get; set; }
		public string Title { get; set; }
		public bool IsGroup
		{
			get
			{
				return MenuModifierVM == null;
			}
		}
		public bool IsItem
		{
			get
			{
				return !IsGroup;
			}
		}

		private List<SubMenuModifierItemViewModel> mSubMenuModifierItemViews = new List<SubMenuModifierItemViewModel>();
		public List<SubMenuModifierItemViewModel> SubMenuModifierItemViews
		{
			get
			{
				return mSubMenuModifierItemViews;
			}
			set
			{
				mSubMenuModifierItemViews = value;
			}
		}


		public RMenuGroupModifierVM MenuGroupModifierVM
		{
			get;
			set;
		}

		private RMenuModifierVM mMenuModifierVM;
		public RMenuModifierVM MenuModifierVM 
		{
			get
			{
				return mMenuModifierVM;
			}
			set
			{
				OnPropertyChanging(nameof(MenuModifierVM));
				mMenuModifierVM = value;
				OnPropertyChanged(nameof(MenuModifierVM));
				OnPropertyChanged(nameof(IsSelected));
				OnPropertyChanged(nameof(IsShowDetail));
				OnPropertyChanged(nameof(IsHaveSub));
				RefreshEnabled();
			}
		}

		public void RefreshEnabled()
		{
			OnPropertyChanged(nameof(IsEnabled));
			OnPropertyChanged(nameof(IsEnabledAdd));
		}

		public bool IsAllowMultipleInstances
		{
			get
			{
				return IsSelected && (MenuModifierVM != null ? MenuModifierVM.MenuModifier.AllowMultipleInstances : false);
			}
		}

		public bool IsEnabled
		{
			get
			{
				if (MenuGroupModifierVM != null 
				    && MenuModifierVM != null
		    		&& (MenuGroupModifierVM.MenuModifierGroup.MinApplied != 1 || MenuGroupModifierVM.MenuModifierGroup.MaxApplied != 1)
				    && !MenuModifierVM.IsSelected)
				{
					var itemsSelected = MenuGroupModifierVM.MenuModifiers.Where(t => t.IsSelected).ToList();
					var countSelected = itemsSelected.Sum(t => t.Quantity);
					if (MenuGroupModifierVM.MenuModifierGroup.MaxApplied.HasValue 
					    && MenuGroupModifierVM.MenuModifierGroup.MaxApplied <= countSelected)
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
				if (MenuGroupModifierVM != null
					&& MenuModifierVM != null
					&& (MenuGroupModifierVM.MenuModifierGroup.MinApplied != 1 || MenuGroupModifierVM.MenuModifierGroup.MaxApplied != 1))
				{
					var itemsSelected = MenuGroupModifierVM.MenuModifiers.Where(t => t.IsSelected).ToList();
					var countSelected = itemsSelected.Sum(t => t.Quantity);
					if (MenuGroupModifierVM.MenuModifierGroup.MaxApplied.HasValue
						&& MenuGroupModifierVM.MenuModifierGroup.MaxApplied <= countSelected)
					{
						return false;
					}
					return true;
				}
				return true;
			}
		}

		public string Price
		{
			get
			{
				return MenuModifierVM != null && MenuModifierVM.ModifierPrice.HasValue ? MenuModifierVM.ModifierPrice.Value.ToString("0.00") : null;
			}
		}

		public bool IsHaveSub
		{
			get
			{
				return MenuModifierVM != null ? MenuModifierVM.SubMenuModifiers.Count > 0 : false;
			}
		}

		public bool IsShowDetail
		{
			get
			{
				return MenuModifierVM != null ? !string.IsNullOrEmpty(MenuModifierVM.MenuModifier.DetailedDescription) : false;
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
				SetSelected(value);
				if (!mIsSelected)
				{
					//Quantity = 1;
				}
				else 
				{
					var itemsSelected = MenuGroupModifierVM.MenuModifiers.Where(t => t.IsSelected).ToList();
					var countSelected = itemsSelected.Sum(t => t.Quantity);
					var maxApplied = MenuGroupModifierVM.MenuModifierGroup.MaxApplied;
					if (maxApplied.HasValue && countSelected - maxApplied > 0)
					{
						Quantity = Math.Max(1 , Quantity - (countSelected - maxApplied.Value));
					}
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
				if (MenuModifierVM != null)
				{
					MenuModifierVM.Quantity = mQuantity;
				}
				OnPropertyChanged(nameof(Quantity));
			}
		}

		public bool IsShowModifierSelectedText
		{
			get
			{
				return !string.IsNullOrEmpty(SubMenuModifierSelectedText);
			}
		}

		public string SubMenuModifierSelectedText
		{
			get
			{
				string text = string.Empty;
				if (MenuModifierVM != null && MenuModifierVM.SubMenuModifiers != null)
				{
					var subMenuModifiersSelected = MenuModifierVM.SubMenuModifiers
					                                             .Where(t => (MenuModifierVM.IsSelected && t.IsSelected)
					                                                    || (!MenuModifierVM.IsSelected && t.MenuModifier.ApplyByDefault))
					                                             .ToList();
					foreach (var subMenuModifier in subMenuModifiersSelected)
					{
						if (string.IsNullOrEmpty(text))
						{
							text += subMenuModifier.MenuModifier.DisplayName;
						}
						else
						{
							text += ("/" + subMenuModifier.MenuModifier.DisplayName);
						}
					}
				}
				return text;
			}
		}

		private void SetSelected(bool isSelected)
		{
			mIsSelected = isSelected;
			MenuModifierVM.IsSelected = isSelected;
			if (!isSelected)
			{
				foreach (var subMenuModifierItemView in SubMenuModifierItemViews)
				{
					subMenuModifierItemView.SubMenuModifierVM.Quantity = 1;
					subMenuModifierItemView.SubMenuModifierVM.IsSelected = false;
				}

				SubMenuModifierItemViews.Clear();
			}
		}

		public void Refresh()
		{
			RefreshEnabled();
			OnPropertyChanged(nameof(SubMenuModifierSelectedText));
			OnPropertyChanged(nameof(IsShowModifierSelectedText));
			if (IsShowModifierSelectedText)
			{
				//OnPropertyChanged(nameof(ForceUpdateSize));
			}
		}

		public void Save()
		{
			//SetSelected(IsSelected);
			//MenuModifierVM.Quantity = Quantity;

			if (SubMenuModifierItemViews != null)
			{
				foreach (var subMenuModifierItemView in SubMenuModifierItemViews)
				{
					subMenuModifierItemView.Save();
				}
			}
			if (SaveChanged != null)
			{
				SaveChanged(this, EventArgs.Empty);
			}
			OnPropertyChanged(nameof(SubMenuModifierSelectedText));
			OnPropertyChanged(nameof(IsShowModifierSelectedText));
			if (IsShowModifierSelectedText)
			{
				//OnPropertyChanged(nameof(ForceUpdateSize));
			}
		}

		public ModifierItemViewModel(RMenuGroupModifierVM menuGroupModifierVM)
		{
			MenuGroupModifierVM = menuGroupModifierVM;
			var minApplied = MenuGroupModifierVM.MenuModifierGroup.MinApplied;
			var maxApplied = MenuGroupModifierVM.MenuModifierGroup.MaxApplied;

			if (minApplied.HasValue && minApplied > 0)
			{
				if (maxApplied.HasValue && maxApplied != minApplied)
				{
					Title = string.Format(AppResources.ChooseRequired, minApplied + "-" + maxApplied + " " + MenuGroupModifierVM.MenuModifierGroup.DisplayName + " -");
				}
				else
				{
					Title = string.Format(AppResources.ChooseRequired, minApplied + " " + MenuGroupModifierVM.MenuModifierGroup.DisplayName + " -");
				}
			}
			else
			{
				if (maxApplied.HasValue && maxApplied > 0)
				{
					Title = string.Format(AppResources.ChooseOptional, "up to " + maxApplied + " " + MenuGroupModifierVM.MenuModifierGroup.DisplayName + " -");
				}
				else
				{
					Title = string.Format(AppResources.ChooseOptional, MenuGroupModifierVM.MenuModifierGroup.DisplayName + " -");
				}
			}
		}

		public ModifierItemViewModel(RMenuGroupModifierVM menuGroupModifierVM, RMenuModifierVM menuModifier, bool applyByDefault = true)
		{
			MenuModifierVM = menuModifier;
			MenuGroupModifierVM = menuGroupModifierVM;

			Quantity = Math.Max(1, MenuModifierVM.Quantity);
			IsSelected = MenuModifierVM.IsSelected;

			if (applyByDefault)
			{
				if (!this.IsSelected)
				{
					Quantity = 1;
					this.IsSelected = MenuModifierVM.MenuModifier.ApplyByDefault;
				}
			}
		}
	}
}
