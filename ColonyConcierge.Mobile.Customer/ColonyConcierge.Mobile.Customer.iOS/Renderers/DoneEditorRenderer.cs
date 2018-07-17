using System;
using System.Drawing;
using ColonyConcierge.Mobile.Customer;
using ColonyConcierge.Mobile.Customer.iOS;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(Editor), typeof(DoneEditorRenderer))]
namespace ColonyConcierge.Mobile.Customer.iOS
{
	public class DoneEditorRenderer : EditorRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
		{
			base.OnElementChanged(e);
			if (this.Control != null)
			{
				this.Control.AutocorrectionType = UITextAutocorrectionType.No;
			}
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
	}
}
