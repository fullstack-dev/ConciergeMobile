using System;
using System.ComponentModel;
using ColonyConcierge.Mobile.Customer;
using ColonyConcierge.Mobile.Customer.iOS;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using XLabs.Forms.Controls;

[assembly: ExportRenderer(typeof(Switch), typeof(SwitchCustomRenderer))]

namespace ColonyConcierge.Mobile.Customer.iOS
{
	public class SwitchCustomRenderer : SwitchRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<Switch> e)
		{
			base.OnElementChanged(e);

			if (this.Element != null && this.Control != null)
			{
				this.Control.SetDefaultFont();
			}
		}
	}
}
