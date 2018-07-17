using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ColonyConcierge.Mobile.Customer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RestaurantMenuListView : ContentView
	{
		public ListView ListViewMenus
		{
			get
			{
				return this.ListView;
			}
		}

		public ActivityIndicatorView IndicatorView
		{
			get
			{
				return this.ActivityIndicatorView;
			}
		}

		public RestaurantMenuListView()
		{
			InitializeComponent();
		}
	}
}
