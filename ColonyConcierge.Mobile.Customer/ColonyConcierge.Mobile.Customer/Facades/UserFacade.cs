using System;
using System.Threading.Tasks;
using ColonyConcierge.APIData.Data;
using Xamarin.Forms;

namespace ColonyConcierge.Mobile.Customer
{
	public class UserFacade
	{
		public void RequireLogin(Page currentPage, Action doneAction)
		{
			if (Shared.IsLoggedIn)
			{
				doneAction();
			}
			else
			{
				if (Device.RuntimePlatform == Device.iOS)
				{
					var signinPage = new SigninPage()
					{
						ServiceAddress = Shared.LocalAddress
					};
					var customNavigationPage = new CustomNavigationPage(signinPage);
					signinPage.ShowBack(customNavigationPage);
					signinPage.Back += () =>
					{
						if (signinPage.Navigation.ModalStack != null 
						    && signinPage.Navigation.ModalStack.Count > 0)
						{
							signinPage.Navigation.PopModalAsync();
						}
					};
					signinPage.Done += (arg) =>
					{
						signinPage.Navigation.PopModalAsync();
						doneAction();
					};
					currentPage.Navigation.PushModalAsync(customNavigationPage, true);
				}
				else 
				{
					var authPage = new AuthPage(Shared.LocalAddress);
					authPage.ShowBack();
					authPage.Back += () =>
					{
						if (authPage.Navigation.ModalStack != null 
						    && authPage.Navigation.ModalStack.Count > 0)
						{
							authPage.Navigation.PopModalAsync();
						}
					};
					authPage.Done += (arg) =>
					{
						authPage.Navigation.PopModalAsync();
						doneAction();
					};
					currentPage.Navigation.PushModalAsync(authPage, true);
				}
			}
		}

		public void GetCurrentUser(Page currentPage, Action<User> doneAction)
		{
			if (Shared.IsLoggedIn)
			{
				var userModel = Shared.APIs.IUsers.GetCurrentUser();
				doneAction(userModel);
			}
			else 
			{
				var authPage = new AuthPage();
				authPage.Done += (arg) =>
				{
					authPage.Navigation.PopModalAsync();
					doneAction(arg);
				};
				currentPage.Navigation.PushModalAsync(authPage, true);
			}
		}
	}
}
