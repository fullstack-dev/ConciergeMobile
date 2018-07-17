using System;
using System.Collections.Generic;
using System.Linq;
using ColonyConcierge.APIData.Data;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace ColonyConcierge.Mobile.Customer
{
	public class CustomNavigationPage : NavigationPage
	{
		public CustomNavigationPage()
		{
		}

		public CustomNavigationPage(Page root) : base(root)
		{
		}

		public Func<Page, bool> PopView;
		public Func<Page, bool> PopToRoot;
		public Func<Page, bool> PushView;
	}
}