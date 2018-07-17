using ColonyConcierge.APIData.Data;
using ColonyConcierge.Client;
using System;
using ColonyConcierge.Mobile.Customer.Localization.Resx;
using Plugin.Toasts;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ColonyConcierge.Mobile.Customer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SignupPage : ContentPageBase
	{
		public Action<User> Done
		{
			get;
			set;
		}

		public Action Back
		{
			get;
			set;
		}

		public RegistrationEntry RegistrationEntry
		{
			get
			{
				return this.BindingContext as RegistrationEntry;
			}
		}

		public bool IsHaveSignIn
		{
			get
			{
				return StackLayoutSignIn.IsVisible;
			}
			set
			{
				StackLayoutSignIn.IsVisible = value;
			}
		}

		public bool IsFromSignUp
		{
			get;
			set;
		}

		public bool IsAnimationBack
		{
			get;
			set;
		} = false;

		public SignupPage()
		{
			// Create the viewModel for user registration it needs to propagate along
			// in all teh registartion screens
			this.BindingContext = new RegistrationEntry()
			{
				PhoneNumbers = new List<PhoneNumber>()
						{
					new PhoneNumber() { Priority = 0, Type = "Mobile"  }
						},
				ServiceAddress = new ExtendedAddress() { BasicAddress = new Address() },
				SecurityQuestions = new string[1],
				SecurityAnswers = new string[1]
			};
			try
			{
				InitializeComponent();
			}
			catch (Exception e)
			{
				System.Diagnostics.Debug.WriteLine("found resource: " + e.Message);
			}

			confirmpasswordValidator.CompareToEntry = passwordEntry;

			StackLayoutSignIn.GestureRecognizers.Add(new TapGestureRecognizer
			{
				Command = new Command((obj) =>
				{
					if (IsFromSignUp)
					{
						Navigation.PopAsync().ConfigureAwait(false);
					}
					else
					{
						Utils.PushAsync(Navigation, new SigninPage()
						{
							IsFromSignIn = false,
							Done = Done,
						});
					}
				})
			});
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

			//if (this.Width > 0 && this.Height > 0)
			//{
			//	if (IsAnimationBack)
			//	{
			//		this.IsAnimationBack = false;
			//		Animation animation = new Animation((s) =>
			//		{
			//			this.TranslationX = -this.Width * s;
			//		}, 1, 0);
			//		animation.Commit(this, "OnAppearing", 16, 500, Easing.Linear);
			//	}
			//}
		}

		//protected override void OnDisappearing()
		//{
		//	base.OnDisappearing();

		//	if (this.Width > 0 && this.Height > 0)
		//	{
		//		if (this.Width > 0 && this.Height > 0)
		//		{
		//			Animation animation = new Animation((s) =>
		//			{
		//				this.TranslationX = this.Width * s;
		//			}, 0, 1);
		//			animation.Commit(this, "OnAppearing", 16, 500, Easing.Linear);
		//		}
		//	}
		//}

		private bool mIsBack = false;
		public void ShowBack(CustomNavigationPage customNavigationPage)
		{
			this.Navigation.InsertPageBefore(new Page(), this);
			customNavigationPage.PopView = (page) =>
			{
				if (page == this)
				{
					mIsBack = true;
					Back.Invoke();
					return true;
				}
				return false;
			};
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();

			if (Device.RuntimePlatform == Device.iOS 
			    && (this.Navigation.NavigationStack == null || this.Navigation.NavigationStack.Count <= 1))
			{
				if (Back != null && !mIsBack)
				{
					Back.Invoke();
				}
			}
		}

		static readonly BindablePropertyKey IsValidPropertyKey = BindableProperty.CreateReadOnly("IsValid", typeof(bool), typeof(SignupPage), false);

		public static readonly BindableProperty IsValidProperty = IsValidPropertyKey.BindableProperty;



        async private void Button_Clicked(object sender, EventArgs e)
        {
			(sender as VisualElement).IsEnabled = false;
            this.IsBusy = true;
            RegistrationEntry registerEntry = this.RegistrationEntry;
			var api = new APIs(new Connector());
			try
			{
				 // The email provided is in proper format
				 bool isEmailTaken = await Task.Run(() => 
				 {
					return api.ILogins.IsUserEmailRegistered(registerEntry.EmailAddress); 
				 });
				 if (!isEmailTaken)
				 {
					if (!Utils.IsPullDataFailMessage)
					{
						await Utils.PushAsync(Navigation, new PersonalInfoPage() { BindingContext = this.BindingContext }, true);
						this.IsAnimationBack = true;
					}
				 }
				 else
				 {
					var notificator = DependencyService.Get<IToastNotificator>();
				 	await notificator.Notify(ToastNotificationType.Error, AppResources.Email, AppResources.EmailTaken, TimeSpan.FromSeconds(5));
				 }
		    }
		    catch (Exception ex)
		    {
				var notificator = DependencyService.Get<IToastNotificator>();
				await notificator.Notify(ToastNotificationType.Error, AppResources.Email, ex.Message, TimeSpan.FromSeconds(5));
		    }
            finally
            {
				(sender as VisualElement).IsEnabled = true;
                this.IsBusy = false;
            }
        }
    }
}
