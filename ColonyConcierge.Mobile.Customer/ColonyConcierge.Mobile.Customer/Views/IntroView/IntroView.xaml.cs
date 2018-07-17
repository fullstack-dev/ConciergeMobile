using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColonyConcierge.Mobile.Customer.ViewModels;
using Xamarin.Forms;

namespace ColonyConcierge.Mobile.Customer
{
    public partial class IntroView : ContentView
    {
		protected override async void OnBindingContextChanged()
		{
			base.OnBindingContextChanged();
			if (this.BindingContext is IntroViewModel)
			{
				var introViewModel = this.BindingContext as IntroViewModel;
				ImageBackground.IsVisible = introViewModel.IsImageBackground;
				if (string.IsNullOrEmpty(introViewModel.Logo))
				{
					GridLogo.IsVisible = false;
					GridContent.BackgroundColor = Color.White;
					GridTitle.IsVisible = true;
					ImageImage.Source = introViewModel.Image;
				}
				else
				{
					ImageBackground.IsVisible = true;
					ImageLogo.Source = introViewModel.Logo;
					GridContent.BackgroundColor = Color.Transparent;
					//GridTitle.IsVisible = false;
					//ImageLogo.SizeChanged += (sender, e) =>
					//{
					//	ViewBackgroundColorImageLogo.HeightRequest = ImageLogo.Height;
					//};
					//if (introViewModel.IsAnimation)
					//{
					//	await Task.Delay(1000);
					//	Animation animation = new Animation((s) =>
					//	{
					//		ImageBackground.Opacity = s;
					//		introViewModel.BottomView.Opacity = s;
					//	}, 0, 1);
					//	animation.Commit(this, "OnAppearing", 40, 2000, Easing.Linear, (arg1, arg2) =>
					//	{
					//		if (introViewModel != null)
					//		{
					//			introViewModel.RaiseOnFinishAnimation();
					//		}
					//	});
					//}
					//else
					//{
					//	ImageBackground.Opacity = 1;
					//}
				}
				//ImageImage.IsVisible = !string.IsNullOrEmpty(introViewModel.Image);
			}
		}

        public IntroView()
        {
            InitializeComponent();
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
			if (Device.RuntimePlatform == Device.iOS)
			{
				Application.Current.MainPage = new SignupPage();
			}
			else 
			{
				Application.Current.MainPage = new AuthPage(true);
			}
        }
    }
}
