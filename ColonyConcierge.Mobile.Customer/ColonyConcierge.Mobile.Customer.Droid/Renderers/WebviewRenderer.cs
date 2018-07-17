using System;
using System.Collections.Generic;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Webkit;
using ColonyConcierge.Mobile.Customer;
using ColonyConcierge.Mobile.Customer.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Maps.Android;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Xamarin.Forms.WebView), typeof(WebviewAndroidRenderer))]
namespace ColonyConcierge.Mobile.Customer.Droid
{
	public class WebviewAndroidRenderer : WebViewRenderer
	{
		protected override Android.Webkit.WebView CreateNativeControl()
		{
			var webView = base.CreateNativeControl();
			webView.SetWebViewClient(new Client());
			webView.SetWebChromeClient(new WebChromeClient());
			return webView;
		}

		protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.WebView> e)
		{
			base.OnElementChanged(e);
			if (this.Element != null)
			{
				this.Element.Navigating += (sender, e2) =>
				{
					var url = e2.Url;
					if (url.ToLower().StartsWith("mailto:", StringComparison.CurrentCulture))
					{
						try
						{
							var uri = new Uri(url);
							Device.OpenUri(uri);
							e2.Cancel = true;
						}
						catch (Exception)
						{
							e2.Cancel = true;
						}
					}
					else if (url.ToLower().StartsWith("tel:", StringComparison.CurrentCulture))
					{
						try
						{
							var uri = new Uri(url);
							Device.OpenUri(uri);
							e2.Cancel = true;
						}
						catch (Exception)
						{
							e2.Cancel = true;
						}
					}
				};
			}
		}

		class Client : WebViewClient
		{
			//public override bool ShouldOverrideUrlLoading(Android.Webkit.WebView view, string url)
			//{
			//	return base.ShouldOverrideUrlLoading(view, url);
			//}

			//public override bool ShouldOverrideUrlLoading(Android.Webkit.WebView view, IWebResourceRequest request)
			//{
			//	return base.ShouldOverrideUrlLoading(view, request);
			//}
		}
    }
}
