using System;
using System.Collections.Generic;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using ColonyConcierge.Mobile.Customer;
using ColonyConcierge.Mobile.Customer.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Maps.Android;
using Xamarin.Forms.Platform.Android;
using Support = Android.Support.V7.Widget;

[assembly: ExportRenderer(typeof(TabbedPage), typeof(TabbedPageCustomRenderer))]
namespace ColonyConcierge.Mobile.Customer.Droid
{
	public class TabbedPageCustomRenderer : Xamarin.Forms.Platform.Android.AppCompat.TabbedPageRenderer
	{
		TabLayout _tabLayout;
		private ViewGroup _toolbar;

		protected override void OnElementChanged(ElementChangedEventArgs<TabbedPage> e)
		{
			base.OnElementChanged(e);
		}

		protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
		{
			base.OnSizeChanged(w, h, oldw, oldh);
			if (this.Element != null && _toolbar != null)
			{
				_toolbar.SetDefaultFont();
			}
		}

		public override void OnViewAdded(Android.Views.View child)
		{
			base.OnViewAdded(child);
			if (child is TabLayout)
			{
				_tabLayout = (TabLayout)child;
				_tabLayout.SetDefaultFont();
			}

			if (child.GetType() == typeof(Support.Toolbar) ||
				child.GetType() == typeof(Toolbar))
			{
				_toolbar = (ViewGroup)child;
			}
		}

		protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);
			if (this.Element != null && _tabLayout != null)
			{
				_tabLayout.SetDefaultFont();
			}
			if (this.Element != null && _toolbar != null)
			{
				_toolbar.SetDefaultFont();
			}
		}
	}
}
