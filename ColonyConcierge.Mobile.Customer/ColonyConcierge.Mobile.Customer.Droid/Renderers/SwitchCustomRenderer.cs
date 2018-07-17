using System;
using System.ComponentModel;
using Android.OS;
using Android.Text;
using Android.Text.Method;
using ColonyConcierge.Mobile.Customer;
using ColonyConcierge.Mobile.Customer.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Switch), typeof(SwitchCustomRenderer))]
namespace ColonyConcierge.Mobile.Customer.Droid
{
	public class SwitchCustomRenderer : SwitchRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Switch> e)
		{
			try
			{
				base.OnElementChanged(e);
				if (this.Element != null && this.Control != null)
				{
					this.Control.SetDefaultFont();
				}

			}
			catch (Exception)
			{
				return;
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			try
			{
				base.OnElementPropertyChanged(sender, e);
			}
			catch (Exception)
			{
				return;
			}
		}
	}
}
