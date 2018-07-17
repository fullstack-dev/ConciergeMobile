using System;
using System.ComponentModel;
using ColonyConcierge.Mobile.Customer.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Picker), typeof(PickerCustomRenderer))]
[assembly: ExportRenderer(typeof(DatePicker), typeof(DatePickerCustomRenderer))]
[assembly: ExportRenderer(typeof(TimePicker), typeof(TimePickerCustomRenderer))]

namespace ColonyConcierge.Mobile.Customer.Droid
{
	public class PickerCustomRenderer : PickerRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Picker> e)
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

	public class DatePickerCustomRenderer : DatePickerRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.DatePicker> e)
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

	public class TimePickerCustomRenderer : TimePickerRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.TimePicker> e)
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
