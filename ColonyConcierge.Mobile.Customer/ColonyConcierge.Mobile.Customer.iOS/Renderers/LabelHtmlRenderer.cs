using System;
using System.ComponentModel;
using ColonyConcierge.Mobile.Customer;
using ColonyConcierge.Mobile.Customer.iOS;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(LabelHtml), typeof(LabelHtmlRenderer))]
namespace ColonyConcierge.Mobile.Customer.iOS
{
	public class LabelHtmlRenderer : ViewRenderer<LabelHtml, UITextView>
	{
		protected override void OnElementChanged(ElementChangedEventArgs<LabelHtml> e)
		{
			base.OnElementChanged(e);

			if (e.OldElement != null || this.Element == null)
				return;

			if (e.OldElement != null)
				e.OldElement.PropertyChanged -= OnElementPropertyChanged;

			e.NewElement.PropertyChanged += NewElement_PropertyChanged;

			if (this.Control == null)
			{
				var yourTextView = new UITextView();
				yourTextView.Editable = false;
				yourTextView.DataDetectorTypes = UIDataDetectorType.All;
				//webView.SetWebChromeClient(new ChromeClient());
				SetNativeControl(yourTextView);

				if (!string.IsNullOrEmpty((e.NewElement as LabelHtml).HtmlText))
				{
					var attr = new NSAttributedStringDocumentAttributes();
					var nsError = new NSError();
					attr.DocumentType = NSDocumentType.HTML;
					var myHtmlData = NSData.FromString((e.NewElement as LabelHtml).HtmlText, NSStringEncoding.Unicode);
					this.Control.AttributedText = new NSAttributedString(myHtmlData, attr, ref nsError);
					this.Control.Font = UIFont.FromName(iOS.Appearance.Instance.FontNameDefault, (nfloat)Math.Ceiling(e.NewElement.FontSize));
				}	
			}
		}

		void NewElement_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(Label.Text))
			{
				var attr = new NSAttributedStringDocumentAttributes();
				var nsError = new NSError();
				attr.DocumentType = NSDocumentType.HTML;

				var myHtmlData = NSData.FromString((this.Element as LabelHtml).HtmlText, NSStringEncoding.Unicode);
				this.Control.AttributedText = new NSAttributedString(myHtmlData, attr, ref nsError);	
			}
		}
	}
}
