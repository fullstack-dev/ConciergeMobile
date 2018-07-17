using System;
using System.ComponentModel;
using Android.OS;
using Android.Text;
using Android.Text.Method;
using CarouselView.FormsPlugin.Abstractions;
using CarouselView.FormsPlugin.Android;
using ColonyConcierge.Mobile.Customer;
using ColonyConcierge.Mobile.Customer.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using XLabs.Forms.Controls;

[assembly: ExportRenderer(typeof(CarouselViewControlCustom), typeof(CarouselViewCustomRenderer))]

namespace ColonyConcierge.Mobile.Customer.Droid
{
	public class CarouselViewCustomRenderer : CarouselView.FormsPlugin.Android.CarouselViewRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<CarouselViewControl> e)
		{
			base.OnElementChanged(e);
		}

		private double GetHeight(Android.Views.View view)
		{
			if (view.Height < 1 && view.Parent is Android.Views.View)
			{
				return GetHeight(view.Parent as Android.Views.View);
			}
			else
			{
				return view.Height;
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (this.Element is CarouselViewControlCustom && this.Control != null)
			{
				var marginBottom = (this.Element as CarouselViewControlCustom).MarginBottom * GetHeight(this) / this.Element.Height;
				var indicator = this.FindViewById<CirclePageIndicator>(Resource.Id.indicator);
				if (indicator != null)
				{
					indicator.TranslationY = -(float)marginBottom;
				}
			}
		}
	}
}
