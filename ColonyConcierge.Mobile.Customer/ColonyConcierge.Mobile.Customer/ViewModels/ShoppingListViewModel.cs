using System;
using System.Collections.Generic;
using ColonyConcierge.APIData.Data;
using Xamarin.Forms;

namespace ColonyConcierge.Mobile.Customer
{
	public class ShoppingListViewModel : BindableObject
	{
		private bool mForceUpdateSize = false;
		public bool ForceUpdateSize
		{
			get
			{
				return mForceUpdateSize;
			}
			set
			{
				OnPropertyChanging(nameof(ForceUpdateSize));
				mForceUpdateSize = value;
				OnPropertyChanged(nameof(ForceUpdateSize));
			}
		}
		public ShoppingList Model { get; set; }

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
				ForceUpdateSize = true;
			}
		}

		private Action<ShoppingListViewModel> mDeleteAction;
		private Command mDeleteCommand;
		public Command DeleteCommand
		{
			get
			{
				mDeleteCommand = mDeleteCommand ?? new Command(() =>
				{
					if (mDeleteAction != null)
					{
						mDeleteAction.Invoke(this);
					}
				});
				return mDeleteCommand;
			}
		}


		public ShoppingListViewModel(ShoppingList shoppingList, Action<ShoppingListViewModel> deleteAction = null)
		{
			Model = shoppingList;
			mDeleteAction = deleteAction;
		}
	}
}
