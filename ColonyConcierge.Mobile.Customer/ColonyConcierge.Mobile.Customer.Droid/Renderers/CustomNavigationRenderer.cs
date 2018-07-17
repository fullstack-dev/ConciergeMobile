using System;
using System.Threading.Tasks;
using Android.Support.V7.Widget;
using Android.Views;
using ColonyConcierge.Mobile.Customer;
using ColonyConcierge.Mobile.Customer.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Maps.Android;
using Xamarin.Forms.Platform.Android;
using Support = Android.Support.V7.Widget;

[assembly: ExportRenderer(typeof(CustomNavigationPage), typeof(CustomNavigationRenderer))]
namespace ColonyConcierge.Mobile.Customer.Droid
{
	public class CustomNavigationRenderer : Xamarin.Forms.Platform.Android.AppCompat.NavigationPageRenderer, Android.Views.View.IOnClickListener
	{
		private ViewGroup _toolbar;

		public CustomNavigationRenderer() : base()
		{
		}

		public new void OnClick(Android.Views.View v)
		{
			base.OnClick(v);
		}

		protected override void OnElementChanged(ElementChangedEventArgs<NavigationPage> e)
		{
			base.OnElementChanged(e);

			if (e.NewElement is INavigationPageController)
			{
				var navController = (INavigationPageController)e.NewElement;
				navController.PopRequested += (sender2, e2) =>
				{
					if (this.Element is CustomNavigationPage)
					{
						var customNavigationPage = this.Element as CustomNavigationPage;
						if (customNavigationPage.PopView != null && customNavigationPage.PopView(e2.Page))
						{
						}
					}
				};

				navController.PopToRootRequested += (sender2, e2) =>
				{
					if (this.Element is CustomNavigationPage)
					{
						var customNavigationPage = this.Element as CustomNavigationPage;
						if (customNavigationPage.PopToRoot != null && customNavigationPage.PopToRoot(e2.Page))
						{
						}
					}
				};

				navController.PushRequested += (sender2, e2) =>
				{
					if (this.Element is CustomNavigationPage)
					{
						var customNavigationPage = this.Element as CustomNavigationPage;
						if (customNavigationPage.PushView != null && customNavigationPage.PushView(e2.Page))
						{
						}
					}
				};
			}
		}

		protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (this.Element != null && _toolbar != null)
			{
				_toolbar.SetDefaultFont();
			}
		}

		protected override void OnAttachedToWindow()
		{
			base.OnAttachedToWindow();

			if (this.Element != null && _toolbar != null)
			{
				_toolbar.SetDefaultFont();
			}
		}

		public override void OnViewAdded(Android.Views.View child)
		{
			base.OnViewAdded(child);

			if (child.GetType() == typeof(Support.Toolbar) ||
				child.GetType() == typeof(Toolbar))
			{
				_toolbar = (ViewGroup)child;
				_toolbar.SetDefaultFont();
			}
		}

		protected override void OnDetachedFromWindow()
		{
			if (Element == null)
			{
				return;
			}
			base.OnDetachedFromWindow();
		}
	}
}
