using System;
using System.ComponentModel;
using Android.OS;
using Android.Text;
using Android.Text.Method;
using ColonyConcierge.Mobile.Customer;
using ColonyConcierge.Mobile.Customer.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Label), typeof(LabelCustomRenderer))]
namespace ColonyConcierge.Mobile.Customer.Droid
{
	public class LabelCustomRenderer :LabelRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Label> e)
		{
			try
			{
				base.OnElementChanged(e);
				if (this.Element != null)
				{
					if (this.Element.FontAttributes == FontAttributes.Bold)
					{
						this.Element.FontFamily = Appearance.Instance.FontFamilyBold;
					}
					else
					{
                        this.Element.FontFamily = Appearance.Instance.FontFamilyDefault;
					}
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

				if (this.Element != null && e.PropertyName == nameof(this.Element.FontAttributes))
				{
					if (this.Element.FontAttributes == FontAttributes.Bold)
					{
						this.Element.FontFamily = Appearance.Instance.FontFamilyBold;
					}
					else
					{
						this.Element.FontFamily = Appearance.Instance.FontFamilyDefault;
					}
				}
			}
			catch (Exception)
			{
				return;
			}
		}
	}
}
