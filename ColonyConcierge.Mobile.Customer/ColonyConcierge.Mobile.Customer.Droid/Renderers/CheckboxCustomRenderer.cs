using System;
using System.ComponentModel;
using Android.OS;
using Android.Text;
using Android.Text.Method;
using ColonyConcierge.Mobile.Customer;
using ColonyConcierge.Mobile.Customer.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using XLabs.Forms.Controls;

[assembly: ExportRenderer(typeof(CheckBox), typeof(CheckboxCustomRenderer))]
[assembly: ExportRenderer(typeof(CheckBoxCustom), typeof(CheckboxCustomRenderer))]
[assembly: ExportRenderer(typeof(CustomRadioButton), typeof(RadioButtonCustomRenderer))]
[assembly: ExportRenderer(typeof(RadioButtonCustom), typeof(RadioButtonCustomRenderer))]

namespace ColonyConcierge.Mobile.Customer.Droid
{
	public class CheckboxCustomRenderer : CheckBoxRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<CheckBox> e)
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

	public class RadioButtonCustomRenderer : RadioButtonRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<CustomRadioButton> e)
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
