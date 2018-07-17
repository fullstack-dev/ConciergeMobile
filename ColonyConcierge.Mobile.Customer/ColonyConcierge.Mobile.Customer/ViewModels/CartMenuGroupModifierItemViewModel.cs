using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace ColonyConcierge.Mobile.Customer
{
	public class CartMenuGroupModifierItemViewModel : BindableObject
	{
		public MenuGroupModifierItem Model { get; set; }

		private Action<CartMenuGroupModifierItemViewModel> mDeleteAction;
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


		public CartMenuGroupModifierItemViewModel(MenuGroupModifierItem menuGroupModifierItem, Action<CartMenuGroupModifierItemViewModel> deleteAction)
		{
			Model = menuGroupModifierItem;
			mDeleteAction = deleteAction;
		}
	}
}
