using System;
using System.ComponentModel;
using ColonyConcierge.Mobile.Customer;
using ColonyConcierge.Mobile.Customer.iOS;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using XLabs.Forms.Controls;

[assembly: ExportRenderer(typeof(CheckBox), typeof(CheckboxCustomRenderer))]
[assembly: ExportRenderer(typeof(CheckBoxCustom), typeof(CheckboxCustomRenderer))]
[assembly: ExportRenderer(typeof(CustomRadioButton), typeof(RadioButtonCustomRenderer))]
[assembly: ExportRenderer(typeof(RadioButtonCustom), typeof(RadioButtonCustomRenderer))]

namespace ColonyConcierge.Mobile.Customer.iOS
{
	public class CheckboxCustomRenderer : CheckBoxRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<CheckBox> e)
		{
			base.OnElementChanged(e);

			if (this.Element != null && this.Control != null)
			{
				this.Control.SetDefaultFont();
			}
		}
	}

	public class RadioButtonCustomRenderer : RadioButtonRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<CustomRadioButton> e)
		{
			base.OnElementChanged(e);

			if (this.Element != null && this.Control != null)
			{
				this.Control.SetDefaultFont();
			}
		}
	}
}
