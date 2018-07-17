using System;
using System.ComponentModel;
using ColonyConcierge.Mobile.Customer;
using ColonyConcierge.Mobile.Customer.iOS;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(Picker), typeof(PickerCustomRenderer))]
[assembly: ExportRenderer(typeof(DatePicker), typeof(DatePickerCustomRenderer))]
[assembly: ExportRenderer(typeof(TimePicker), typeof(TimePickerCustomRenderer))]

namespace ColonyConcierge.Mobile.Customer.iOS
{
	public class PickerCustomRenderer : PickerRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<Picker> e)
		{
			base.OnElementChanged(e);
			if (this.Element != null && this.Control != null)
			{
				this.Control.SetDefaultFont();
			}
		}
	}

	public class DatePickerCustomRenderer : DatePickerRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<DatePicker> e)
		{
			base.OnElementChanged(e);
			if (this.Element != null && this.Control != null)
			{
				this.Control.SetDefaultFont();
			}
		}
	}

	public class TimePickerCustomRenderer : TimePickerRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<TimePicker> e)
		{
			base.OnElementChanged(e);
			if (this.Element != null && this.Control != null)
			{
				this.Control.SetDefaultFont();
			}
		}
	}
}
