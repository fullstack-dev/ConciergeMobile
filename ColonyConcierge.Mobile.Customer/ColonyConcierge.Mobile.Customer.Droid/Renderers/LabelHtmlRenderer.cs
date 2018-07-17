using System;
using System.ComponentModel;
using Android.OS;
using Android.Text;
using Android.Text.Method;
using ColonyConcierge.Mobile.Customer;
using ColonyConcierge.Mobile.Customer.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(LabelHtml), typeof(LabelHtmlRenderer))]
namespace ColonyConcierge.Mobile.Customer.Droid
{
	public class LabelHtmlRenderer :LabelRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Label> e)
		{
			base.OnElementChanged(e);
			if (e.OldElement != null || this.Element == null)
				return;

			if (e.OldElement != null)
				e.OldElement.PropertyChanged -= OnElementPropertyChanged;

			(e.NewElement as LabelHtml).PropertyChanged += NewElement_PropertyChanged;

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

			if (!string.IsNullOrEmpty((e.NewElement as LabelHtml).HtmlText))
			{
				if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
				{
					this.Control.TextFormatted = Html.FromHtml((e.NewElement as LabelHtml).HtmlText, FromHtmlOptions.ModeLegacy);
				}
				else
				{
#pragma warning disable CS0618 // Type or member is obsolete
					this.Control.TextFormatted = Html.FromHtml((e.NewElement as LabelHtml).HtmlText);
#pragma warning restore CS0618 // Type or member is obsolete
				}
			}
			this.Control.LinksClickable = true;
			this.Control.MovementMethod = LinkMovementMethod.Instance;
		}

		void NewElement_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(LabelHtml.HtmlText))
			{
				if (!string.IsNullOrEmpty((this.Element as LabelHtml).HtmlText))
				{
					if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
					{
						this.Control.TextFormatted = Html.FromHtml((this.Element as LabelHtml).HtmlText, FromHtmlOptions.ModeLegacy);
					}
					else
					{
	#pragma warning disable CS0618 // Type or member is obsolete
                        this.Control.TextFormatted = Html.FromHtml((this.Element as LabelHtml).HtmlText);
	#pragma warning restore CS0618 // Type or member is lete
					}
				}
			}
		}
	}
}
