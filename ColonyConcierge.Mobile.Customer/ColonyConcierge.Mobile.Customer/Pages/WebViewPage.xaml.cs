using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ColonyConcierge.Mobile.Customer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WebViewPage : ContentPageBase
	{
		public string Url { get; set; }

		public WebViewPage(string url)
		{
			InitializeComponent();
			Url = url;
			Uri uri = new Uri(url);
			Browser.Source = uri;
			this.IsBusy = true;
			Browser.Navigated += (sender, e) =>
			{
				var appServices = DependencyService.Get<IAppServices>();
				appServices.SetNetworkBar(false);
				this.IsBusy = false;
				if (e.Result != WebNavigationResult.Success)
				{
					ShowLoadErrorPage();
				}
			};
		}

		public override void ReloadPage()
		{
			base.ReloadPage();

            this.IsBusy = true;
			WebView webview = new WebView();
			webview.VerticalOptions = LayoutOptions.FillAndExpand;
			webview.HorizontalOptions = LayoutOptions.FillAndExpand;
			StackLayoutBrowser.Children.Clear();
			StackLayoutBrowser.Children.Add(webview);
			webview.Navigated += (sender, e) =>
			{
				var appServices = DependencyService.Get<IAppServices>();
				appServices.SetNetworkBar(false);
				this.IsBusy = false;
				if (e.Result != WebNavigationResult.Success)
				{
					ShowLoadErrorPage();
				}
			};
			Uri uri = new Uri(Url);
			webview.Source = uri;
		}

		protected override bool OnBackButtonPressed()
		{
			Application.Current.MainPage = new HomePage();
			return true;
		}
	}
}
