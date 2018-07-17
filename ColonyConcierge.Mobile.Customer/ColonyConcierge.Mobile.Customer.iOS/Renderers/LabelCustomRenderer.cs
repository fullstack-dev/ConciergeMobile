using System;
using System.ComponentModel;
using ColonyConcierge.Mobile.Customer;
using ColonyConcierge.Mobile.Customer.iOS;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(Label), typeof(LabelCustomRenderer))]
namespace ColonyConcierge.Mobile.Customer.iOS
{
	public class LabelCustomRenderer : LabelRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
		{
			base.OnElementChanged(e);

			if (this.Element != null)
			{
				if (this.Element.FontAttributes == FontAttributes.Bold)
				{
					this.Element.FontFamily = iOS.Appearance.Instance.FontFamilyBold;
				}
				else
				{
					this.Element.FontFamily = iOS.Appearance.Instance.FontFamilyDefault;
				}
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
						this.Element.FontFamily = iOS.Appearance.Instance.FontFamilyBold;
					}
					else
					{
						this.Element.FontFamily = iOS.Appearance.Instance.FontFamilyDefault;
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
