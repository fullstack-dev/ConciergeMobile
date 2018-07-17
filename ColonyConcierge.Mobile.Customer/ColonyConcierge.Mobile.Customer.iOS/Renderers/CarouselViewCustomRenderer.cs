using System;
using System.ComponentModel;
using CarouselView.FormsPlugin.Abstractions;
using CarouselView.FormsPlugin.iOS;
using ColonyConcierge.Mobile.Customer;
using ColonyConcierge.Mobile.Customer.iOS;
using CoreGraphics;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using XLabs.Forms.Controls;

[assembly: ExportRenderer(typeof(CarouselViewControlCustom), typeof(CarouselViewCustomRenderer))]

namespace ColonyConcierge.Mobile.Customer.iOS
{
	public class CarouselViewCustomRenderer : CarouselViewRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<CarouselViewControl> e)
		{
			base.OnElementChanged(e);

			if (this.Element != null && this.Control != null)
			{
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (this.Element is CarouselViewControlCustom && this.Control != null)
			{
				var marginBottom = (this.Element as CarouselViewControlCustom).MarginBottom * this.Bounds.Height / this.Element.Height;
				foreach (var subView in this.Control.Subviews)
				{
					if (subView is UIPageControl)
					{
						subView.Transform = CGAffineTransform.MakeTranslation(0, (float)-marginBottom);
					}
				}
			}
		}
	}
}
