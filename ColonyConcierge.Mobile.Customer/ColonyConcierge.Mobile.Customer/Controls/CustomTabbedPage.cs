using System;

using Xamarin.Forms;

namespace ColonyConcierge.Mobile.Customer
{
	public class CustomTabbedPage : TabbedPage
	{
		public static readonly BindableProperty TabBarHiddenProperty =
			BindableProperty.Create(nameof(TabBarHidden), typeof(bool), typeof(CustomTabbedPage), false);

		public bool TabBarHidden
		{
			get { return (bool)GetValue(TabBarHiddenProperty); }
			set { SetValue(TabBarHiddenProperty, value); }
		}

		public CustomTabbedPage()
		{
		}
	}
}

