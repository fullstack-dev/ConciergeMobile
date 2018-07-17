using System;
using ColonyConcierge.Mobile.Customer.iOS;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ListView), typeof(ListViewRender))]
namespace ColonyConcierge.Mobile.Customer.iOS
{
	public class ListViewRender : ListViewRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<ListView> e)
		{
			base.OnElementChanged(e);
			if (ViewController is UITableViewController)
			{
				var tableViewController = ViewController as UITableViewController;
				if (tableViewController.RefreshControl != null)
				{
					tableViewController.RefreshControl.TintColor = iOS.Appearance.Instance.PrimaryColor.ToUIColor();
				}
			}
		}
	}
}
