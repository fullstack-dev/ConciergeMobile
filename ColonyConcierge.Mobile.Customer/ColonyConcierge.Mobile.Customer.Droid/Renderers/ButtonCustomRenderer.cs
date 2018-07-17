using System;
using System.ComponentModel;
using Android.OS;
using Android.Text;
using Android.Text.Method;
using ColonyConcierge.Mobile.Customer;
using ColonyConcierge.Mobile.Customer.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Button), typeof(ButtonCustomRenderer))]
namespace ColonyConcierge.Mobile.Customer.Droid
{
	public class ButtonCustomRenderer : ButtonRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Button> e)
		{
			try
			{
				base.OnElementChanged(e);
				if (this.Element != null)
				{
					this.Element.FontFamily = Appearance.Instance.FontFamilyDefault;
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
