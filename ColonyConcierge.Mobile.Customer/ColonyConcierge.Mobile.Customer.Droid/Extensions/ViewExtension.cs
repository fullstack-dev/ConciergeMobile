using System;
using Android.Graphics;
using Android.Views;
using Android.Widget;

namespace ColonyConcierge.Mobile.Customer.Droid
{
	public static class ViewExtension
	{
		public static void SetDefaultFont(this View view)
		{
			if (view is ViewGroup)
			{
				var viewGroup = view as ViewGroup;
				for (int i = 0; i<viewGroup.ChildCount; i++)
				{
					viewGroup.GetChildAt(i).SetDefaultFont();
				}
			}
			else if (view is TextView)
			{
				var textView = (view as TextView);
				if (textView.Typeface != null && textView.Typeface.Style == TypefaceStyle.Bold)
				{
					Typeface tf = Appearance.Instance.TypefaceBold;
					textView.SetTypeface(tf, textView.Typeface.Style);
				}
				else
				{
					Typeface tf = Appearance.Instance.TypefaceDefault;
					textView.SetTypeface(tf, TypefaceStyle.Normal);
				}
			}
		}
	}
}
