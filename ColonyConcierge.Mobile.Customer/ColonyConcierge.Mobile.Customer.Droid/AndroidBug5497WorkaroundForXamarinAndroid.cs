using System;
using System.Collections.Generic;
using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace ColonyConcierge.Mobile.Customer.Droid
{
	public class AndroidBug5497WorkaroundForXamarinAndroid
	{
		private Activity mActivity;
		private IWindowManager mWindowManager;
		private Android.Views.View mChildOfContent;
		private FrameLayout mContent;
		private int mUsableHeightPrevious;
		private FrameLayout.LayoutParams mFrameLayoutParams;

		public static void AssistActivity(Activity activity, IWindowManager windowManager)
		{
			AndroidBug5497WorkaroundForXamarinAndroid _;
			_ = new AndroidBug5497WorkaroundForXamarinAndroid(activity, windowManager);
		}

		private AndroidBug5497WorkaroundForXamarinAndroid(Activity activity, IWindowManager windowManager)
		{
			mActivity = activity;
			mWindowManager = windowManager;
			Init();
		}

		private void Init()
		{
			try
			{
				var softButtonsHeight = GetSoftbuttonsbarHeight(mWindowManager);
				mContent = (FrameLayout)mActivity.FindViewById(Android.Resource.Id.Content);
				mChildOfContent = mContent.GetChildAt(0);
				mFrameLayoutParams = (FrameLayout.LayoutParams)mChildOfContent.LayoutParameters;
				var vto = mChildOfContent.ViewTreeObserver;
				vto.GlobalLayout += (sender, e) => PossiblyResizeChildOfContent(softButtonsHeight);
			}
			catch (Exception)
			{
				Handler handler = new Handler();
				handler.PostDelayed(() =>
				{
					Init();
				}, 1000);
			}
		}

		private void PossiblyResizeChildOfContent(int softButtonsHeight)
		{
			try
			{
				var usableHeightNow = ComputeUsableHeight();
				var usableHeightSansKeyboard = mChildOfContent.RootView.Height - softButtonsHeight;
				var heightDifference = usableHeightSansKeyboard - usableHeightNow;
				if (heightDifference > (usableHeightSansKeyboard / 4))
				{
					//ShiftPageUp(heightDifference);

					// keyboard probably just became visible
					if (usableHeightNow != mUsableHeightPrevious)
					{
						mFrameLayoutParams.Height = usableHeightSansKeyboard - heightDifference + (softButtonsHeight / 2);
						mChildOfContent.RequestLayout();
					}

					var focusView = this.mActivity.CurrentFocus;
					if (focusView != null)
					{
						var scrollView = FindParentScrollView(focusView);
						var editTexts = FindAllEditText(scrollView);
						if (scrollView != null)
						{
							var x = scrollView.ScrollX;
							var y = scrollView.ScrollY;
							scrollView.ScrollX = x;
							scrollView.ScrollY = y;
							scrollView.PostDelayed(() =>
							{
								try
								{
									if (scrollView is ScrollViewRenderer)
									{
										if ((scrollView as ScrollViewRenderer).Element == null)
										{
											return;
										}
									}
									scrollView.ScrollX = x;
									scrollView.ScrollY = y;
									if (focusView != null)
									{
										var offset = GetScrollVerticalOffset(focusView);
										var index = editTexts.FindIndex(et => et == focusView);
										if (index > 0)
										{
											var editTextPrevious = editTexts[index - 1];
											var editTextPreviousPosition = (int)GetScrollVerticalOffset(editTextPrevious);
											var editTextPosition = (int)GetScrollVerticalOffset(focusView);
											var scrollY = (int)(offset - scrollView.Height / 5);
											var maxScroll = scrollView.GetChildAt(0).Height - scrollView.Height;
											if (index == editTexts.Count - 1)
											{
												var newScrollY = Math.Min(maxScroll, editTextPreviousPosition);
												if (newScrollY + scrollView.Height > editTextPosition + focusView.Bottom)
												{
													scrollY = newScrollY;
												}
												else
												{
													scrollY = editTextPosition;
												}
											}
											else
											{
												var editTextNext = editTexts[index + 1];
												var editTextNextPosition = (int)GetScrollVerticalOffset(editTextNext);
												scrollY = Math.Min(maxScroll, editTextPosition);
												if (scrollY >= editTextPreviousPosition
													&& scrollY < editTextPreviousPosition + editTextPrevious.Bottom)
												{
													var newScrollY = editTextPreviousPosition;
													if (newScrollY + scrollView.Height > editTextNextPosition + editTextNext.Bottom)
													{
														scrollY = newScrollY;
													}
												}
											}
											scrollView.SmoothScrollTo(scrollView.ScrollX, scrollY);
										}
									}
								}
								catch (Exception)
								{
									//view is dispose
								}
							}, 100);
						}
					}
				}
				else if (usableHeightNow != mUsableHeightPrevious)
				{
					// keyboard probably just became hidden
					mChildOfContent.PostDelayed(() =>
					{
						mFrameLayoutParams.Height = ViewGroup.LayoutParams.MatchParent;
						mChildOfContent.RequestLayout();
					}, 200);
					mFrameLayoutParams.Height = usableHeightSansKeyboard + 1;
					mChildOfContent.RequestLayout();
				}
				mUsableHeightPrevious = usableHeightNow;
			}
			catch (Exception)
			{
				return;
			}
		}

		public double GetScrollVerticalOffset(Android.Views.View child)
		{
			var offset = child.Top;
			var parent = child.Parent as Android.Views.View;
			while (!(parent is Android.Widget.ScrollView) && parent != null)
			{
				offset += parent.Top;
				parent = parent.Parent as Android.Views.View;
			}
			if (parent == null)
			{
				return 0;
			}

			return offset;
		}

		private void ShiftPageUp(double keyboardHeight)
		{
			var pageFrame = (Xamarin.Forms.Application.Current.MainPage as ContentPage);
			if (Xamarin.Forms.Application.Current.MainPage is MasterDetailPage)
			{
				var mainPage = Xamarin.Forms.Application.Current.MainPage as MasterDetailPage;
                pageFrame = (mainPage.Detail as NavigationPage).CurrentPage as ContentPage;
			}
			if (pageFrame != null)
			{
				keyboardHeight = pageFrame.Height * keyboardHeight / (mContent.Height - GetTitleBarHeight());
				pageFrame.Content.HeightRequest = pageFrame.Height - keyboardHeight;
				pageFrame.Content.VerticalOptions = LayoutOptions.StartAndExpand;
			}
		}

		private void ShiftPageDown(double keyboardHeight)
		{
			var pageFrame = (Xamarin.Forms.Application.Current.MainPage as ContentPage);
			if (Xamarin.Forms.Application.Current.MainPage is MasterDetailPage)
			{
				var mainPage = Xamarin.Forms.Application.Current.MainPage as MasterDetailPage;
				pageFrame = (mainPage.Detail as NavigationPage).CurrentPage as ContentPage;
			}
			if (pageFrame != null)
			{
				pageFrame.Content.HeightRequest = pageFrame.Height;
				pageFrame.Content.VerticalOptions = LayoutOptions.FillAndExpand;
			}
		}

		private List<EditText> FindAllEditText(Android.Views.View parentView)
		{
			var editTexts = new List<EditText>();
			if (parentView is EditText)
			{
				editTexts.Add(parentView as EditText);
			}
			else if (parentView is ViewGroup)
			{
				ViewGroup viewGroup = parentView as ViewGroup;
				for (int i = 0; i < viewGroup.ChildCount; i++)
				{
					Android.Views.View childView = viewGroup.GetChildAt(i);
					editTexts.AddRange(FindAllEditText(childView));
				}
			}

			return editTexts;
		}

		private Android.Widget.ScrollView FindParentScrollView(Android.Views.View childView)
		{
			if (childView is Android.Widget.ScrollView)
			{
				return childView as Android.Widget.ScrollView;
			}
			else if (childView != null)
			{
				return FindParentScrollView(childView.Parent as Android.Views.View);
			}
			return null;
		}

		private int ComputeUsableHeight()
		{
			var titleBarHeight = GetTitleBarHeight();
			var rect = new Rect();
			mContent.GetWindowVisibleDisplayFrame(rect);
			return rect.Bottom - rect.Top
				       + (mContent.Height == mContent.RootView.Height ? titleBarHeight : 0);
		}

		private int GetTitleBarHeight()
		{
			var titleBarHeight = 0;
			try
			{
				Rect rectangle = new Rect();
				Window window = this.mActivity.Window;
				window.DecorView.GetWindowVisibleDisplayFrame(rectangle);

				int statusBarHeight = rectangle.Top;
				int contentViewTop = window.FindViewById(Window.IdAndroidContent).Top;
				titleBarHeight =  statusBarHeight - contentViewTop;
			}
			catch (Exception)
			{
				titleBarHeight = 0;
			}
			return titleBarHeight;
		}

		private int GetSoftbuttonsbarHeight(IWindowManager windowManager)
		{
			if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
			{
				var metrics = new DisplayMetrics();
				windowManager.DefaultDisplay.GetMetrics(metrics);
				int usableHeight = metrics.HeightPixels;
				windowManager.DefaultDisplay.GetRealMetrics(metrics);
				int realHeight = metrics.HeightPixels;
				if (realHeight > usableHeight)
					return realHeight - usableHeight;
				else
					return 0;
			}
			return 0;
		}
	}
}
