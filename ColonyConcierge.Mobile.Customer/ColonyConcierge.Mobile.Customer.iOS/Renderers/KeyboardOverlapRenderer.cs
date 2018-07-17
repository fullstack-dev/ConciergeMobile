using System;
using System.Diagnostics;
using ColonyConcierge.Mobile.Customer.iOS;
using CoreGraphics;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(Page), typeof(ColonyConcierge.Mobile.Customer.iOS.KeyboardOverlapRenderer))]
namespace ColonyConcierge.Mobile.Customer.iOS
{
	[Preserve(AllMembers = true)]
	public class KeyboardOverlapRenderer : PageRenderer
	{
		NSObject _keyboardShowObserver;
		NSObject _keyboardHideObserver;
		CGRect keyboardFrame;

		public static new void Init()
		{
			var now = DateTime.Now;
		}

		public override void ViewWillAppear(bool animated)
		{
			try
			{
				base.ViewWillAppear(animated);

				var page = Element as ContentPage;
				if (page != null)
				{
					var contentScrollView = page.Content as ScrollView;
					if (contentScrollView != null)
					{
						return;
					}

					RegisterForKeyboardNotifications();
				}
			}
			catch (Exception)
			{
				//page is disposed
			}
		}

		public override void ViewWillDisappear(bool animated)
		{
			try
			{
				base.ViewWillDisappear(animated);

				UnregisterForKeyboardNotifications();
			}
			catch (Exception)
			{
				//page is disposed
			}
		}

		void RegisterForKeyboardNotifications()
		{
			if (_keyboardShowObserver == null)
				_keyboardShowObserver = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillShowNotification, OnKeyboardShow);
			if (_keyboardHideObserver == null)
				_keyboardHideObserver = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, OnKeyboardHide);
			//NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillChangeFrameNotification, OnKeyboardShow);
		}

		void UnregisterForKeyboardNotifications()
		{
			if (_keyboardShowObserver != null)
			{
				NSNotificationCenter.DefaultCenter.RemoveObserver(_keyboardShowObserver);
				_keyboardShowObserver.Dispose();
				_keyboardShowObserver = null;
			}

			if (_keyboardHideObserver != null)
			{
				NSNotificationCenter.DefaultCenter.RemoveObserver(_keyboardHideObserver);
				_keyboardHideObserver.Dispose();
				_keyboardHideObserver = null;
			}
		}

		protected virtual void OnKeyboardShow(NSNotification notification)
		{
			try
			{
				if (this.Element == null || this.View == null || !IsViewLoaded)
				{
					return;
				}

				keyboardFrame = UIKeyboard.FrameEndFromNotification(notification);
				ShiftPageUp(keyboardFrame.Height);
			}
			catch (Exception)
			{
				//page is disposed
			}
		}

		private void OnKeyboardHide(NSNotification notification)
		{
			try
			{
				ShiftPageDown();
			}
			catch (Exception)
			{
				//page is disposed
			}
		}

		private UIScrollView FindScrollView(UIView superView)
		{
			try
			{
				if (superView is UIScrollView)
				{
					return superView as UIScrollView;
				}
				else if (superView != null)
				{
					var scrollView = FindScrollView(superView.Superview);
					if (scrollView != null)
					{
						return scrollView;
					}
				}
			}
			catch (Exception)
			{
				//page is closed
				return null;
			}
			return null;
		}

		public double GetScrollVerticalOffset(UIView child)
		{
			var offset = child.Frame.Y;
			var parent = child.Superview;
			try
			{
				while (parent != null && !(parent is UIScrollView))
				{
					offset += parent.Frame.Y;
					parent = parent.Superview;
				}
				if (parent == null)
				{
					return 0;
				}
			}
			catch (Exception)
			{
				//page is closed
				return offset;
			}

			return offset;
		}

		private void ShiftPageUp(nfloat keyboardHeight)
		{
			var pageFrame = Element.Bounds;
			if (Element is ContentPage && View != null)
			{
				var contentPage = Element as ContentPage;
				if (contentPage.Content != null)
				{
					contentPage.Content.HeightRequest = pageFrame.Height - keyboardHeight;
					contentPage.Content.VerticalOptions = LayoutOptions.StartAndExpand;
					var activeView = View.FindFirstResponder();
					if (activeView != null)
					{
						var scrollView = FindScrollView(activeView.Superview);
						if (scrollView != null)
						{
							var offset = GetScrollVerticalOffset(activeView);
							if (offset > scrollView.Frame.Height / 2.5)
							{
								var maxScrollY = scrollView.ContentSize.Height - scrollView.Frame.Height;
								var scrollY = offset - (double)(scrollView.Frame.Height / 3);
								scrollY = Math.Min(maxScrollY, scrollY);
								Device.BeginInvokeOnMainThread(() =>
								{
									scrollView.SetContentOffset(new CGPoint(0, scrollY), true);
								});
							}
							else if (offset <= 20)
							{
								Device.BeginInvokeOnMainThread(() =>
								{
									try
									{
										scrollView.SetContentOffset(new CGPoint(0, 0), false);
									}
									catch (Exception)
									{
											//page is disposed
									}
								});
							}
						}
					}
				}
			}
		}

		private void ShiftPageDown()
		{
			if (Element is ContentPage)
			{
				var contentPage = Element as ContentPage;
				if (contentPage.Content != null)
				{
					var pageFrame = Element.Bounds;
					contentPage.Content.HeightRequest = pageFrame.Height;
					contentPage.Content.VerticalOptions = LayoutOptions.FillAndExpand;
				}
			}
		}

		private double CalculateShiftByAmount(double pageHeight, nfloat keyboardHeight, double activeViewBottom)
		{
			return (pageHeight - activeViewBottom) - keyboardHeight;
		}
	}
}