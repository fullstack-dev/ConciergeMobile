using System;
using System.Collections.Generic;
using System.ComponentModel;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Webkit;
using ColonyConcierge.Mobile.Customer;
using ColonyConcierge.Mobile.Customer.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Xamarin.Forms.ScrollView), typeof(ScrollAndroidViewRenderer))]
namespace ColonyConcierge.Mobile.Customer.Droid
{
	public class ScrollAndroidViewRenderer : ScrollViewRenderer
	{
		protected override void OnElementChanged(VisualElementChangedEventArgs e)
		{
			base.OnElementChanged(e);
			if (e.OldElement != null || this.Element == null)
			{
				return;
			}

			if (e.OldElement != null)
			{
				e.OldElement.PropertyChanged -= OnElementPropertyChanged;
			}

			if (e.NewElement != null)
			{
				e.NewElement.PropertyChanged += OnElementPropertyChanged;
			}
		}


		protected void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			try
			{
				if (this.ViewGroup != null && this.Element != null)
				{
					if (ChildCount > 0)
					{
						GetChildAt(0).HorizontalScrollBarEnabled = false;
					}
				}
			}
			catch (Exception){}
		}
	}
}
