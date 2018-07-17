using System;
using System.Collections.Generic;
using System.Linq;
using ColonyConcierge.APIData.Data;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace ColonyConcierge.Mobile.Customer
{
	public class EntryCustom : Entry
	{
		private bool mNeedShowKeyboard = false;
		public bool NeedShowKeyboard 
		{
			get
			{
				return mNeedShowKeyboard;
			}
			set
			{
				mNeedShowKeyboard = value;
				OnPropertyChanged(nameof(NeedShowKeyboard));
			}
		}

	}
}
