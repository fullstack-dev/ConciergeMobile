using System;
using ColonyConcierge.Mobile.Customer;
using ColonyConcierge.Mobile.Customer.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(ImageTouch), typeof(ImageTouchRenderer))]
namespace ColonyConcierge.Mobile.Customer.Droid
{
	public class ImageTouchRenderer : ImageRenderer
	{
		ImageTouch ImageTouch;

		void Control_Touch(object sender, TouchEventArgs e)
		{
			try
			{
				if (ImageTouch != null)
				{
					switch (e.Event.Action)
					{
						case Android.Views.MotionEventActions.Pointer1Down:
						case Android.Views.MotionEventActions.Down:
							ImageTouch.RaiseOnTouch(true);
							break;
						case Android.Views.MotionEventActions.Up:
						case Android.Views.MotionEventActions.Pointer1Up:
						case Android.Views.MotionEventActions.Cancel:
							ImageTouch.RaiseOnTouch(false);
							break;
					}
				}
			}
			catch (Exception)
			{
				return;
			}
		}

		protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Image> e)
		{
			try
			{
				base.OnElementChanged(e);

				if (e.OldElement != null)
				{
					Control.Touch -= Control_Touch;
				}
				if (e.NewElement != null)
				{
					ImageTouch = (ImageTouch)e.NewElement;
					Control.Touch += Control_Touch;
				}
			}
			catch (Exception)
			{
				return;
			}
		}
	}
}
