
using System.Threading.Tasks;
using ColonyConcierge.Mobile.Customer;
using ColonyConcierge.Mobile.Customer.iOS;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CustomNavigationPage), typeof(CustomNavigationRenderer))]
[assembly: ExportRenderer(typeof(NavigationPage), typeof(CustomNavigationRenderer))]

namespace ColonyConcierge.Mobile.Customer.iOS
{
	public class CustomNavigationRenderer : NavigationRenderer
	{
		public override UIKit.UIViewController PopViewController(bool animated)
		{
			if (this.Element is CustomNavigationPage)
			{
				var customNavigationPage = this.Element as CustomNavigationPage;
				if (customNavigationPage.PopView != null && customNavigationPage.PopView(customNavigationPage.CurrentPage))
				{
				}
			}

			return base.PopViewController(animated);
		}


		protected override Task<bool> OnPopToRoot(Page page, bool animated)
		{
			if (this.Element is CustomNavigationPage)
			{
				var customNavigationPage = this.Element as CustomNavigationPage;
				if (customNavigationPage.PopToRoot != null && customNavigationPage.PopToRoot(page))
				{
					return Task.FromResult(true);
				}
			}
			return base.OnPopToRoot(page, animated);
		}

		protected override System.Threading.Tasks.Task<bool> OnPushAsync(Page page, bool animated)
		{
			if (this.Element is CustomNavigationPage)
			{
				var customNavigationPage = this.Element as CustomNavigationPage;
				if (customNavigationPage.PushView != null && customNavigationPage.PushView(page))
				{
					return Task.FromResult(true);
				}
			}
			return base.OnPushAsync(page, animated);
		}
	}
}
