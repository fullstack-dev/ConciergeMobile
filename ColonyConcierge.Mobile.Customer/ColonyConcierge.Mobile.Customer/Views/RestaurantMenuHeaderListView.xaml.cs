using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms.Xaml;

namespace ColonyConcierge.Mobile.Customer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RestaurantMenuHeaderListView : ContentView
	{
		//private bool locked;
		//private ScrollView ScrollViewMenus;
		//private double? ScrollViewMenusX;

		public RestaurantMenuHeaderListView()
		{
			//this.BindingContextChanged += (sender, e) =>
			//{
			//	Task.Run(() =>
			//	{
			//		while (locked)
			//		{
			//			new System.Threading.ManualResetEvent(false).WaitOne(20);
			//		}
			//	}).ContinueWith((t) =>
			//	{
			//		GridMenus.Children.Clear();
			//		if (this.BindingContext is RestaurantDetailPage)
			//		{
			//			var restaurantDetailPage = this.BindingContext as RestaurantDetailPage;

			//			ScrollViewMenusX = restaurantDetailPage.ScrollViewMenus.ScrollX;
			//			restaurantDetailPage.ScrollViewMenus = new ScrollView
			//			{
			//				Orientation = ScrollOrientation.Horizontal,
			//				Content = restaurantDetailPage.ScrollViewMenus.Content
			//			};
			//			ScrollViewMenus = restaurantDetailPage.ScrollViewMenus;
			//			ScrollViewMenus.SizeChanged += (sender2, e2) =>
			//			{
			//				Device.BeginInvokeOnMainThread(async () =>
			//				{
			//					locked = false;
			//					try
			//					{
			//						if (ScrollViewMenus != null && ScrollViewMenusX.HasValue)
			//						{
			//							var x = ScrollViewMenusX.Value;
			//							ScrollViewMenusX = null;
			//							await ScrollViewMenus.ScrollToAsync(x, 0, false);
			//						}
			//					}
			//					catch (Exception)
			//					{
			//						ScrollViewMenusX = null;
			//					}
			//					locked = true;
			//				});
			//			};

			//			GridMenus.Children.Add(ScrollViewMenus);
			//		}
			//	}, TaskScheduler.FromCurrentSynchronizationContext());
			//};

			InitializeComponent();
		}
	}
}
