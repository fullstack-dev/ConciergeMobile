using System;
using ColonyConcierge.Mobile.Customer;
using ColonyConcierge.Mobile.Customer.iOS;
using Foundation;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using System.Linq;

[assembly: ExportRenderer(typeof(CustomTabbedPage), typeof(CustomTabbedPageRenderer))]
namespace ColonyConcierge.Mobile.Customer.iOS
{
	public class CustomTabbedPageRenderer : TabbedRenderer
	{
		protected override void OnElementChanged(VisualElementChangedEventArgs e)
		{
			base.OnElementChanged(e);


			if (this.Tabbed is CustomTabbedPage)
			{
				CustomTabbedPage customTabbedPage = this.Element as CustomTabbedPage;
				var visible = customTabbedPage.TabBarHidden;
				//TabBar.Hidden = visible;

				var frame = this.TabBar.Frame;
				var height = frame.Size.Height;
				var offsetY = (visible ? -height : height);

				// animation
				this.TabBar.Frame = new CoreGraphics.CGRect(frame.X, frame.Y - offsetY, frame.Width, frame.Height);
				this.View.Frame = new CoreGraphics.CGRect(0, 0, this.View.Frame.Width, this.View.Frame.Height + offsetY);

				this.View.SetNeedsDisplay();
				this.View.LayoutIfNeeded();


				// The solution to the space left behind the invisible tab bar
				//if (TabBar.Hidden)
				//{
				//	View.Subviews[1].Frame = new CoreGraphics.CGRect(View.Subviews[1].Frame.X, View.Subviews[1].Frame.Y, View.Subviews[1].Frame.Width, 0);
				//	View.Subviews[0].Frame = new CoreGraphics.CGRect(0, 0, 320, 568);
				//}
				//else
				//{
				//	View.Subviews[1].Frame = new CoreGraphics.CGRect(View.Subviews[1].Frame.X, View.Subviews[1].Frame.Y, View.Subviews[1].Frame.Width, 49);
				//}
			}
		}
	}
}
