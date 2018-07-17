using System;
using ColonyConcierge.Mobile.Customer;
using ColonyConcierge.Mobile.Customer.iOS;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ImageTouch), typeof(ImageTouchRenderer))]
namespace ColonyConcierge.Mobile.Customer.iOS
{
	public class ImageTouchRenderer : ImageRenderer
	{
		void Control_TouchesBegan(Foundation.NSSet arg1, UIKit.UIEvent arg2)
		{

		}

		ImageTouch ImageTouch;

		//void Control_Touch(object sender, TouchEventArgs e)
		//{
		//	if (ImageTouch != null)
		//	{
		//		switch (e.Event.Action)
		//		{
		//			case Android.Views.MotionEventActions.Pointer1Down:
		//			case Android.Views.MotionEventActions.Down:
		//				ImageTouch.RaiseOnTouch(true);
		//				break;
		//			case Android.Views.MotionEventActions.Up:
		//			case Android.Views.MotionEventActions.Pointer1Up:
		//			case Android.Views.MotionEventActions.Cancel:
		//				ImageTouch.RaiseOnTouch(false);
		//				break;
		//		}
		//	}
		//}

		public override void TouchesEnded(Foundation.NSSet touches, UIKit.UIEvent evt)
		{
			base.TouchesEnded(touches, evt);
			if (ImageTouch != null)
			{
				ImageTouch.RaiseOnTouch(false);
			}
		}

		public override void TouchesBegan(Foundation.NSSet touches, UIKit.UIEvent evt)
		{
			base.TouchesBegan(touches, evt);
			if (ImageTouch != null)
			{
				ImageTouch.RaiseOnTouch(true);
			}
		}

		public override void TouchesCancelled(Foundation.NSSet touches, UIKit.UIEvent evt)
		{
			base.TouchesCancelled(touches, evt);
			if (ImageTouch != null)
			{
				ImageTouch.RaiseOnTouch(false);
			}
		}

		protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Image> e)
		{
			base.OnElementChanged(e);

			if (e.OldElement != null)
			{
			}
			if (e.NewElement != null)
			{
				ImageTouch = (ImageTouch)e.NewElement;
			}
		}
	}
}
