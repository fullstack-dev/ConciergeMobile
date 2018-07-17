using System;
using Xamarin.Forms;

namespace ColonyConcierge.Mobile.Customer
{
	public class LabelHtml : Label
	{
		private string mHtmlText;
		public string HtmlText
		{
			get
			{
				return mHtmlText;
			}
			set
			{
				OnPropertyChanging(nameof(HtmlText));
				mHtmlText = value;
				OnPropertyChanged(nameof(HtmlText));
			}
		}

		public LabelHtml()
		{
			//this.FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label));
		}
	}
}
