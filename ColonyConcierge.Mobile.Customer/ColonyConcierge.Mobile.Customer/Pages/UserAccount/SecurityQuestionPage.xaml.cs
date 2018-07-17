using ColonyConcierge.APIData.Data;
using ColonyConcierge.Client;
using ColonyConcierge.Mobile.Customer.Localization.Resx;
using Plugin.Toasts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ColonyConcierge.Mobile.Customer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SecurityQuestionPage : ContentPageBase
    {
		public bool IsBackAnimation
		{
			get;
			set;
		} = false;

		private IAppServices mAppServices;

		public User UserModel
		{
			get;
			set;
		}

		public BrainTreeCard BrainTreeCard
		{
			get;
			set;
		}

        public SecurityQuestionPage()
        {
            InitializeComponent();

			mAppServices = DependencyService.Get<IAppServices>();

			var url = string.Format("<a href=\"{0}\">{1}</a>", PCLAppConfig.ConfigurationManager.AppSettings["PrivacyPolicyUrl"], AppResources.PolicyTermsConditions);
			TermsButton.HtmlText = "<html>" + string.Format(AppResources.PolicySignupMessage, url) + "</html>";
        }

		async private void ButtonBack_Clicked(object sender, EventArgs e)
		{
			await Navigation.PopAsync().ConfigureAwait(false);
		}

		void OnTapGestureRecognizerTapped(object sender, EventArgs args) { 
			try
			{
				Uri uri = new Uri(PCLAppConfig.ConfigurationManager.AppSettings["LegalUrl"]);
				Device.OpenUri(uri);
			}
			catch (Exception ex)
			{	
				System.Diagnostics.Debug.WriteLine("Exceptions Occured when open the Terms and Conditions" + ex.Message);
			}
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

			//if (this.Width > 0 && this.Height > 0)
			//{
			//	if (IsBackAnimation)
			//	{
			//		IsBackAnimation = false;
			//		this.TranslationX = -this.Width;
			//		Animation animation = new Animation((s) =>
			//		{
			//			this.TranslationX = -this.Width * s;
			//		}, 1, 0);
			//		animation.Commit(this, "OnAppearing", 16, 500, Easing.Linear);
			//	}
			//	else
			//	{
			//		if (mFirstLoad)
			//		{
			//			mFirstLoad = false;
			//			this.TranslationX = this.Width;
			//			Animation animation = new Animation((s) =>
			//			{
			//				this.TranslationX = this.Width * s;
			//			}, 1, 0);
			//			animation.Commit(this, "OnDisappearing", 16, 500, Easing.Linear);
			//		}

			//	}
			//}
		}

        private void ButtonCreate_Clicked(object sender, EventArgs e)
        {
            (sender as VisualElement).IsEnabled = false;
            this.IsBusy = true;
            // Create the account
            RegistrationEntry userInfo = (RegistrationEntry)this.BindingContext;
			userInfo.UserName = userInfo.EmailAddress;
			userInfo.ServiceAddress.IsPreferred = true;
            var api = new APIs(new Connector());
			var isSuccess = true;
			Task.Run(() =>
			{
				try
				{
					LoginResult result = api.IAccounts.RegisterNewAccount(userInfo);
					if (result.Token != null && result.OK)
					{
						api.LoginToken = result.Token;
						//var userModel = api.IUsers.GetCurrentUser();
						//System.Diagnostics.Debug.WriteLine("UserID : " + userModel.ID + " UserName : " + userModel.Username);

						Shared.LoginToken = result.Token;
						Shared.LocalAddress = userInfo.ServiceAddress;
						UserModel = api.IUsers.GetCurrentUser();
						Shared.UserId = UserModel.ID;

						if (BrainTreeCard != null)
						{
							bool creatingCard = true;
							var token = Shared.APIs.IAccounts.GetPaymentGatewayToken();
							BrainTreeCard.Token = token;
							mAppServices.GetNoncenBrainTree(BrainTreeCard, async (obj) =>
							{
								if (obj is string && !string.IsNullOrEmpty(obj as string))
								{
									try
									{
										var paymentNonce = obj as string;
										CreditCardData paymentAccountData = new CreditCardData();
										paymentAccountData.AccountNickname = BrainTreeCard.AccountNickname;
										paymentAccountData.IsPreferred = BrainTreeCard.IsPreferred;
										paymentAccountData.PaymentNonce = paymentNonce;
										await Shared.APIs.IAccounts.BtAddPaymentMethod_Async(UserModel.AccountID, paymentAccountData);
									}
									catch (Exception ex)
									{
										var notificator = DependencyService.Get<IToastNotificator>();
										await notificator.Notify(ToastNotificationType.Error, AppResources.PaymentMethodTitle, ex.Message, TimeSpan.FromSeconds(5));
										System.Diagnostics.Debug.WriteLine("Exception occured When Add Payment Method : " + ex.Message);
									}
								}
								this.IsBusy = false;
								creatingCard = false;
							});

							// wait create BT card
							while (creatingCard)
							{
								new System.Threading.ManualResetEvent(false).WaitOne(50);
							}
						}

						try
						{
							string notificationId = mAppServices.GetRegistrationNotificationId();
							string uniqueDeviceId = mAppServices.UniqueDeviceId;

							if (!string.IsNullOrEmpty(notificationId))
							{
								var platform = "android";
								if (Device.RuntimePlatform == Device.iOS)
								{
									platform = "ios";
								}
								bool isDeviceTokenSet = Shared.APIs.IUsers.AddDeviceToken(UserModel.ID, platform, notificationId, uniqueDeviceId);
								if (!isDeviceTokenSet)
								{
									throw new Exception(AppResources.RegisterNotificationFail);
								}
								Shared.NotificationToken = notificationId;
							}
						}
						catch (Exception ex)
						{
							Device.BeginInvokeOnMainThread(() =>
							{
								(sender as VisualElement).IsEnabled = true;
								Utils.ShowErrorMessage(new CustomException(ex.Message, ex));
							});
						}

						isSuccess = true;
					}
					else
					{
						Device.BeginInvokeOnMainThread(async () =>
						{
							// Show the validation error
							await DisplayAlert(AppResources.SignUp, result.Message, AppResources.OK);
							await Navigation.PopToRootAsync().ConfigureAwait(false);
						});
					}
				}
				catch (Exception ex)
				{
					Device.BeginInvokeOnMainThread(async () =>
					{
						var notificator = DependencyService.Get<IToastNotificator>();
						await notificator.Notify(ToastNotificationType.Error, AppResources.Email, ex.Message, TimeSpan.FromSeconds(5));
					});
				}
			}).ContinueWith(t =>
			{
				if (isSuccess)
				{
					var pages = Navigation.NavigationStack.Reverse().ToList();
					var page = pages.FirstOrDefault(s => s is SignupPage || s is AuthPage);
					Action<User> done = null;

					if (page is SignupPage)
					{
						done = (page as SignupPage).Done;
					}
					else if (page is AuthPage)
					{
						done = (page as AuthPage).Done;
					}

					if (done != null)
					{
						done(UserModel);
					}
					else
					{
						Application.Current.MainPage = new HomePage();
					}
				}

				(sender as VisualElement).IsEnabled = true;
				this.IsBusy = false;
			}, TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}
