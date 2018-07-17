using ColonyConcierge.Mobile.Customer.Localization.Resx;
using ColonyConcierge.Mobile.Customer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using ColonyConcierge.APIData.Data;
using Xamarin.Forms.Xaml;

namespace ColonyConcierge.Mobile.Customer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPageBase
    {
		IAppServices mAppServices;

        public MainPage(bool animate = false)
        {
            InitializeComponent();

			mAppServices = DependencyService.Get<IAppServices>();
			mAppServices.SetShowStatus(false);
			mAppServices.TrackPage(this.GetType().Name);

            var intros = new List<IntroViewModel>();

			IntroViewModel introViewModel= new IntroViewModel()
			{
				Logo = "asystyou_logo.png",
				ImageBackground = "shutterstock.png",
				ShowLogin = false,
				IsAnimation= animate,
				BottomView = GridDoubleButton
			};

			if (animate)
			{
				introViewModel.FinishAnimation += (sender, e) =>
				{
					Carousel.ShowIndicators = true;
				};
				Carousel.ShowIndicators = false;
				GridDoubleButton.Opacity = 0;
			}

			intros.Add(introViewModel);
            intros.Add(new IntroViewModel() { Title = AppResources.Howitworks,
                                    SubTitle = AppResources.Request,
                                    Description = AppResources.RequestDescription,
                                    Image = "intro_request.png",
									ImageBackground = "intro_bgrequest.png",
									BackgroundColor= Color.FromHex("#c0ffffff"),
                                    ShowLogin = false });
            intros.Add(new IntroViewModel() { Title = "",
                                    SubTitle = AppResources.Relax,
                                    Description = AppResources.RelaxDescription,
                                    Image = "intro_relax.png",
									ImageBackground = "intro_bgrelax.png",
									BackgroundColor= Color.FromHex("#c0ffffff"),
                                    ShowLogin = false });
            intros.Add(new IntroViewModel() { Title = "",
                                    SubTitle = AppResources.Receive,
                                    Description = AppResources.ReceiveDescription,
                                    Image = "intro_receive.png",
									ImageBackground = "intro_bgreceive.png",
									BackgroundColor= Color.FromHex("#c0ffffff"),
                                    ShowLogin = true });
            this.Carousel.ItemsSource = intros;
			ButtonSignUp.GestureRecognizers.Add(new TapGestureRecognizer()
			{
				Command = new Command(() => 
				{
					if (Device.RuntimePlatform == Device.iOS)
					{
						var signupPage = new SignupPage();
						var customNavigationPage = new CustomNavigationPage(signupPage);
						Application.Current.MainPage = customNavigationPage;
						signupPage.ShowBack(customNavigationPage);
						signupPage.Back += () =>
						{
							var mainPage = new ColonyConcierge.Mobile.Customer.MainPage();
							Application.Current.MainPage = mainPage;
						};
						mAppServices.TrackPage(signupPage.GetType().Name);
					}
					else
					{
						var authPage = new AuthPage(true);
						authPage.ShowBack();
						authPage.Back += () =>
						{
							Application.Current.MainPage = this;
							mAppServices.SetShowStatus(false);
							mAppServices.TrackPage(this.GetType().Name);
						};
						Application.Current.MainPage = authPage;
						mAppServices.TrackPage(authPage.GetType().Name);
					}
					mAppServices.SetShowStatus(true);
				})
			});

			ButtonGetStarted.GestureRecognizers.Add(new TapGestureRecognizer()
			{
				Command = new Command(() =>
				{
					AddressSuggestionListPage addressSuggestionListPage = new AddressSuggestionListPage(this, (obj) =>
					{
						if (obj != null)
						{
							Application.Current.MainPage = new HomePage();
						}
					})
					{
						IsShowQRCode = true
					};
					Application.Current.MainPage = new NavigationPage(addressSuggestionListPage);
					mAppServices.SetShowStatus(true);
				})
			});

			this.SizeChanged += (sender, e) =>
			{
				if (this.Height > 1)
				{
					ButtonGetStarted.Padding = new Thickness(0, this.Height / 44);
					ButtonSignUp.Padding = new Thickness(0, this.Height / 44);
				}
			};

			GridDoubleButton.SizeChanged += (sender, e) =>
			{
				if (GridDoubleButton.Width > 1 && GridDoubleButton.Height > 1)
				{
					Carousel.MarginBottom = GridDoubleButton.Height;
					GridDouble.HeightRequest = GridDoubleButton.Height;
					//RowDefinitionImageDoubleButton.Height = ImageDoubleButton.Height;
					//GridDoubleButton.WidthRequest = ImageDoubleButton.Width;
					//GridDoubleButton.IsVisible = true;
				}
			};
        }

		protected override void OnAppearing()
		{
			base.OnAppearing();

			//Load background first for Android to remove flicker
			//ImageBackground.IsVisible = Device.RuntimePlatform == Device.Android;
		}
    }
}
