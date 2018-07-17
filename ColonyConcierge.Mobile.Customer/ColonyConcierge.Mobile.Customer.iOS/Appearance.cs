using System;
using Xamarin.Forms.Platform.iOS;
using UIKit;
namespace ColonyConcierge.Mobile.Customer.iOS
{
	public class Appearance : AppearanceBase
	{
		public new static Appearance Instance
		{
			get
			{
				return AppearanceBase.Instance as Appearance;
			}
		}

		public static void Init()
		{
			if (mInstance == null)
			{
				var appearance = new Appearance();
				mInstance = appearance;
			}
		}

		private Appearance() : base()
		{
		}

		protected override void OnConfigure()
		{
			base.OnConfigure();

			var titleFontSize = 19;
			UINavigationBar.Appearance.TitleTextAttributes = new UIStringAttributes()
			{
				ForegroundColor = TextColor.ToUIColor(),
				Font = UIFont.FromName(this.FontNameDefault, titleFontSize),
			};
			UINavigationBar.Appearance.SetBackgroundImage(new UIImage(), UIBarMetrics.Default);
			UINavigationBar.Appearance.ShadowImage = new UIImage();

			UIBarButtonItem.Appearance.SetTitleTextAttributes(new UITextAttributes()
			{
				Font = UIFont.FromName(this.FontNameDefault, titleFontSize),
			}, UIControlState.Normal);


			UIBarButtonItem.Appearance.GetTitleTextAttributes(UIControlState.Normal);

			UINavigationBar.Appearance.BarTintColor = PrimaryColor.ToUIColor();
			UINavigationBar.Appearance.TintColor = TextColor.ToUIColor();


			UIProgressView.Appearance.ProgressTintColor = PrimaryColor.ToUIColor();
			
			//UITextField.Appearance.TintColor = AcentColor;

			//UIButton.Appearance.TintColor = PrimaryColor.ToUIColor();

			UIButton.Appearance.SetTitleColor(PrimaryColor.ToUIColor(), UIControlState.Normal);

		}
	}
}
