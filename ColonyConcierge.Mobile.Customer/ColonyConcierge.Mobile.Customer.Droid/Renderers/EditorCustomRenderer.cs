using System;
using System.ComponentModel;
using Android.OS;
using Android.Text;
using Android.Text.Method;
using ColonyConcierge.Mobile.Customer;
using ColonyConcierge.Mobile.Customer.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Editor), typeof(EditorCustomRenderer))]
namespace ColonyConcierge.Mobile.Customer.Droid
{
	public class EditorCustomRenderer :EditorRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Editor> e)
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
			}
			catch (Exception)
			{
				return;
			}
		}
	}
}
