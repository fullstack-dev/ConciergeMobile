using System;
using CoreGraphics;
using UIKit;

namespace ColonyConcierge.Mobile.Customer.iOS
{
	public static class ViewExtensions
	{
		public static void SetDefaultFont(this UIView view)
		{
			if (view.Subviews != null && view.Subviews.Length > 0)
			{
				foreach (var subview in view.Subviews)
				{
					subview.SetDefaultFont();
				}
			}

			if (view is UILabel)
			{
				var text = view as UILabel;
				if ((text.Font.FontDescriptor.SymbolicTraits & UIFontDescriptorSymbolicTraits.Bold) > 0)
				{
					text.Font = UIFont.FromName(Appearance.Instance.FontFamilyBold, text.Font.PointSize);
				}
				else
				{
					text.Font = UIFont.FromName(Appearance.Instance.FontNameDefault, text.Font.PointSize);
				}
            }
			else if(view is UITextView)
			{
				var text = view as UITextView;
				if ((text.Font.FontDescriptor.SymbolicTraits & UIFontDescriptorSymbolicTraits.Bold) > 0)
				{
					text.Font = UIFont.FromName(Appearance.Instance.FontFamilyBold, text.Font.PointSize);
				}
				else
				{
					text.Font = UIFont.FromName(Appearance.Instance.FontNameDefault, text.Font.PointSize);
				}
			}
			else if(view is UITextField)
			{
				var text = view as UITextField;
				if ((text.Font.FontDescriptor.SymbolicTraits & UIFontDescriptorSymbolicTraits.Bold) > 0)
				{
					text.Font = UIFont.FromName(Appearance.Instance.FontFamilyBold, text.Font.PointSize);
				}
				else
				{
					text.Font = UIFont.FromName(Appearance.Instance.FontNameDefault, text.Font.PointSize);
				}
			}
		}

		public static UIView FindFirstResponder(this UIView view)
		{
			if (view.IsFirstResponder)
			{
				return view;
			}
			foreach (UIView subView in view.Subviews)
			{
				var firstResponder = subView.FindFirstResponder();
				if (firstResponder != null)
					return firstResponder;
			}
			return null;
		}

		public static double GetViewRelativeBottom(this UIView view, UIView rootView)
		{
			var viewRelativeCoordinates = rootView.ConvertPointFromView(view.Frame.Location, view);
			var activeViewRoundedY = Math.Round(viewRelativeCoordinates.Y, 2);

			return activeViewRoundedY + view.Frame.Height;
		}

		public static bool IsKeyboardOverlapping(this UIView activeView, UIView rootView, CGRect keyboardFrame)
		{
			var activeViewBottom = activeView.GetViewRelativeBottom(rootView);
			var pageHeight = rootView.Frame.Height;
			var keyboardHeight = keyboardFrame.Height;

			var isOverlapping = activeViewBottom >= (pageHeight - keyboardHeight);

			return isOverlapping;
		}
	}
}
