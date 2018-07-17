using System;

using Xamarin.Forms;

namespace ColonyConcierge.Mobile.Customer
{
	public class ContentPageBase : ContentPage, IReloadPage
	{
		public ContentPageBase()
		{
		}

		public bool IsErrorPage
		{
			get;
			set;
		}

		public virtual void ReloadPage()
		{
			IsErrorPage = false;
		}

		public virtual void ShowLoadErrorPage()
		{
			if (!IsErrorPage)
			{
				IsErrorPage = true;
				Device.BeginInvokeOnMainThread(() =>
				{
					var errorView = new ErrorView();
					var content = Content;
					Content = errorView;
					errorView.TryAgain += (sender, e) =>
					{
                        this.IsErrorPage = false;
						Content = content;
						ReloadPage();
					};
				});
			}
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
		}

		public static bool CheckCurrentPage(Page currentPage)
		{
			try
			{
				if (Application.Current != null)
				{
					if (Application.Current.MainPage is MasterDetailPage)
					{
						var masterDetailPage = Application.Current.MainPage as MasterDetailPage;
						if ((masterDetailPage).Detail is NavigationPage)
						{
							return ((masterDetailPage).Detail as NavigationPage).CurrentPage == currentPage;
						}
						else
						{
							return masterDetailPage.Detail == currentPage;
						}
					}
					else
					{
						if (Application.Current.MainPage is NavigationPage)
						{
							return (Application.Current.MainPage as NavigationPage).CurrentPage == currentPage;
						}
						else
						{
							return Application.Current.MainPage == currentPage;
						}
					}
				}
			}
			catch (Exception)
			{
				//Check page when app pause/stop
				return false;
			}
			return false;
		}

		public bool CheckCurrentPage()
		{
			return CheckCurrentPage(this);
		}
	}
}

