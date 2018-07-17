using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ColonyConcierge.Mobile.Customer.Localization.Resx;
using Plugin.Toasts;
using Xamarin.Forms;
using System.Diagnostics;

namespace ColonyConcierge.Mobile.Customer
{
	public static class Utils
	{
		private static List<string> Errors = new List<string>();
		public static IReloadPage IReloadPageCurrent = null;
		private static bool lockNavigation = false;

		public static bool IsPullDataFailMessage
		{
			get
			{
				if (Errors.Contains(AppResources.PullDataFailMessage))
				{
					return true;
				}
				return false;
			}
		}

		public static Task PushAsync(INavigation navigation, Page page, bool anamited = true)
		{
			if (!lockNavigation)
			{
				lockNavigation = true;

				var appServices = DependencyService.Get<IAppServices>();
				appServices.SetNetworkBar(false);
				appServices.TrackPage(page.GetType().Name);

				NavigationPage.SetBackButtonTitle(page, AppResources.Back);
				return navigation.PushAsync(page, anamited).ContinueWith((arg) =>
				{
					//NavigationPage.SetBackButtonTitle(page, AppResources.Back);
					lockNavigation = false;
				});
			}
			else 
			{
				return Task.FromResult(0);
			}
		}
		public static Task PushModalAsync(INavigation navigation, Page page, bool anamited = true)
		{
			if (!lockNavigation)
			{
				lockNavigation = true;

				var appServices = DependencyService.Get<IAppServices>();
				appServices.SetNetworkBar(false);
				appServices.TrackPage(page.GetType().Name);

				NavigationPage.SetBackButtonTitle(page, AppResources.Back);
				return navigation.PushModalAsync(page, anamited).ContinueWith((arg) =>
				{
					//NavigationPage.SetBackButtonTitle(page, AppResources.Back);
					lockNavigation = false;
				});
			}
			else
			{
				return Task.FromResult(0);
			}
		}

		public static Task<bool> ShowWarningMessage(string message, double seconds = 3)
		{
			return ShowWarningMessage(message, string.Empty, seconds);
		}

		public static Task<bool> ShowWarningMessage(string message, string title = "", double seconds = 3)
		{
			var notificator = DependencyService.Get<IToastNotificator>();
			return notificator.Notify(ToastNotificationType.Warning, string.IsNullOrEmpty(title)? AppResources.Warning : title, message, TimeSpan.FromSeconds(seconds));
		}

		public static Task<bool> ShowSuccessMessage(string message, double seconds = 2)
		{
			return ShowSuccessMessage(message, string.Empty, seconds);
		}

		public static Task<bool> ShowSuccessMessage(string message, string title = "", double seconds = 2)
		{
			var notificator = DependencyService.Get<IToastNotificator>();
			return notificator.Notify(ToastNotificationType.Success, string.IsNullOrEmpty(title) ? AppResources.Success : title, message, TimeSpan.FromSeconds(seconds));
		}

		public static Task<bool> ShowErrorMessage(string message, double seconds = 5)
		{
			var notificator = DependencyService.Get<IToastNotificator>();

			if (Errors.Contains(message))
			{
				return Task.FromResult(false);
			}

			if ((message == AppResources.SomethingWentWrong && Errors.Count > 0)
			    || Errors.Contains(AppResources.PullDataFailMessage))
			{
				return Task.FromResult(false);
			}

			Errors.Add(message);

			return notificator.Notify(ToastNotificationType.Error, AppResources.Error, message, TimeSpan.FromSeconds(seconds))
				              .ContinueWith(t =>
								{
									Errors.Remove(message);
									return t.Result;
								});
		}

		public static Task<bool> ShowErrorMessage(Exception ex, double seconds = 5)
		{
			var message = AppResources.SomethingWentWrong;
			if (ex is CustomException)
			{
				message = ex.Message;
			}

            if (Debugger.IsAttached)
                Debugger.Break();

            return ShowErrorMessage(message, seconds);
		}
	}
}
