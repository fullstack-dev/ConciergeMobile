using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColonyConcierge.Mobile.Customer.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ColonyConcierge.Mobile.Customer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class IntroView : ContentView
    {
		protected override async void OnBindingContextChanged()
		{
			base.OnBindingContextChanged();
			if (this.BindingContext is IntroViewModel)
			{
				var introViewModel = this.BindingContext as IntroViewModel;
				if (!string.IsNullOrEmpty(introViewModel.ImageBackground))
				{
					ImageBackground.IsVisible = true;
					ImageBackground.Source = introViewModel.ImageBackground;
				}
				else
				{
					ImageBackground.IsVisible = false;
				}
				GridContent.BackgroundColor = introViewModel.BackgroundColor;

				if (string.IsNullOrEmpty(introViewModel.Logo))
				{
					GridLogo.IsVisible = false;
					GridTitle.IsVisible = true;
					ImageImage.Source = introViewModel.Image;
					ImageBackground.Opacity = 1;
				}
				else
				{
					ImageLogo.Source = introViewModel.Logo;
					GridTitle.IsVisible = false;
					ImageLogo.SizeChanged += (sender, e) =>
					{
						if (ImageLogo.Width > 1)
						{
							ImageAsystant.WidthRequest = ImageLogo.Width * 0.9;
						}
						if (ImageLogo.Height > 1)
						{
							StackLayoutBGLogo.HeightRequest = ImageLogo.Height;
							RowDefinitionLogo.Height =  ImageLogo.Height;
						}
					};
					if (introViewModel.IsAnimation)
					{
						await Task.Delay(1000);
						Animation animation = new Animation((s) =>
						{
							ImageBackground.Opacity = s;
							StackLayoutYourAsystant.Opacity = s;
							introViewModel.BottomView.Opacity = Math.Max(s, introViewModel.BottomView.Opacity);
						}, 0, 1);
						animation.Commit(this, "OnAppearing", 40, 2000, Easing.Linear, (arg1, arg2) =>
						{
							if (introViewModel != null)
							{
								introViewModel.RaiseOnFinishAnimation();
							}
						});
					}
					else
					{
						ImageBackground.Opacity = 1;
						StackLayoutYourAsystant.Opacity = 1;
					}
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
