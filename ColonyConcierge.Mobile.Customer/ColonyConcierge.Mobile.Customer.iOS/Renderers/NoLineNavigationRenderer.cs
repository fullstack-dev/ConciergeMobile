using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using UIKit;
using ColonyConcierge.Mobile.Customer.iOS;
using System;

[assembly: ExportRenderer(typeof(NavigationPage), typeof(NoLineNavigationRenderer))]
namespace ColonyConcierge.Mobile.Customer.iOS
{
	public class NoLineNavigationRenderer : NavigationRenderer
	{
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			try
			{
				// remove lower border and shadow of the navigation bar
				NavigationBar.SetBackgroundImage(new UIImage(), UIBarMetrics.Default);
				NavigationBar.ShadowImage = new UIImage();
			}
			catch (Exception){}
		}
	}
}