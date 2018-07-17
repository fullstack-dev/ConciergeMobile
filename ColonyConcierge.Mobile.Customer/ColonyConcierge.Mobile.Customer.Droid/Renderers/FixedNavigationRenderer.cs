using System;
using Android.Graphics;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ColonyConcierge.Mobile.Customer.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android.AppCompat;
using Support = Android.Support.V7.Widget;

[assembly: ExportRenderer(typeof(NavigationPage), typeof(FixedNavigationRenderer))]
namespace ColonyConcierge.Mobile.Customer.Droid
{

	// WARNING
	// There is a crash happens for unknown reason:
	// System.MissingMethodException: No constructor found for 
	//   Xamarin.Forms.Platform.Android.AppCompat.NavigationPageRenderer::.ctor(System.IntPtr, Android.Runtime.JniHandleOwnership)
	// see https://bugzilla.xamarin.com/show_bug.cgi?id=40258
	// --
	// As a solution we override NavigationPageRenderer to provide the constructor.
	// Although then there is a NullReferenceException happens in 
	//  OnDetachedFromWindow() method which we are fixing too.
	public class FixedNavigationRenderer : NavigationPageRenderer
	{
		private ViewGroup _toolbar;

		public FixedNavigationRenderer()
			: base()
		{
		}

		public FixedNavigationRenderer(IntPtr javaReference, JniHandleOwnership transfer)
			: base()
		{
		}

		public override void OnViewAdded(Android.Views.View child)
		{
			base.OnViewAdded(child);

			if (child.GetType() == typeof(Support.Toolbar) ||
			    child.GetType() == typeof(Toolbar))
			{
				_toolbar = (ViewGroup)child;
			}
		}

		protected override void OnElementChanged(Xamarin.Forms.Platform.Android.ElementChangedEventArgs<NavigationPage> e)
		{
			base.OnElementChanged(e);
		}

		protected override void OnAttachedToWindow()
		{
			base.OnAttachedToWindow();

			if (this.Element != null && _toolbar != null)
			{
				_toolbar.SetDefaultFont();
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
