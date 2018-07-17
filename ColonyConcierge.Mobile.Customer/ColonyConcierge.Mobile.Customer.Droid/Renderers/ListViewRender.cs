using System;
using Android.Support.V4.Widget;
using ColonyConcierge.Mobile.Customer.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(ListView), typeof(ListViewRender))]
namespace ColonyConcierge.Mobile.Customer.Droid
{
	public class ListViewRender : ListViewRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<ListView> e)
		{
			base.OnElementChanged(e);
			if (this.Control != null && Control.Parent is SwipeRefreshLayout)
			{
				var x = Control.Parent as SwipeRefreshLayout;
				x.SetColorSchemeColors(Droid.Appearance.Instance.PrimaryColor.ToAndroid());
			}
		}
	}
}
