using ColonyConcierge.Client;
using ColonyConcierge.Mobile.Customer.Localization.Resx;
using Plugin.Toasts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using ColonyConcierge.Client.PlatformServices;
using ColonyConcierge.APIData.Data;
using Xamarin.Forms.Xaml;

namespace ColonyConcierge.Mobile.Customer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SigninPage : ContentPageBase
    {
		IAppServices mAppServices;

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

		public User UserModel
		{
			get;
			set;
		}

		public ExtendedAddress ServiceAddress
		{
			get;
			set;
		}

		public bool IsHaveSignIn
		{
			get
			{
				return StackLayoutSignUp.IsVisible;
			}
			set
			{
				StackLayoutSignUp.IsVisible = value;
			}
		}

		public bool IsFromSignIn
		{
			get;
			set;
		}



		public SigninPage()
		{
			InitializeComponent();

			mAppServices = DependencyService.Get<IAppServices>();

			LabelForgotYourPassword.GestureRecognizers.Add(new TapGestureRecognizer()
			{
				Command = new Command(() =>
				{
					//Device.OpenUri(new Uri(PCLAppConfig.ConfigurationManager.AppSettings["ForgotPasswordUrl"]));
                    AlertManager.Prompt("Please enter your username or email.")
                            .ContinueWith((response) =>
                            {
                                //ConsoleOutput.PrintLine(response.Result);
                                var email = response.Result;
                                if (Validator.IsStringAsEmail(email))
                                {
                                    sendForgotPasswordForMail(email);
                                }
                                else
                                {
                                    AlertManager.ShowAlert("Error", "Invalid email.", "OK", this);
                                }
                            });
				})
			});

			StackLayoutSignUp.GestureRecognizers.Add(new TapGestureRecognizer
			{
				Command = new Command((obj) =>
				{
					if (IsFromSignIn)
					{
						Navigation.PopAsync().ConfigureAwait(false);
					}
					else
					{
						var signupPage = new SignupPage()
						{
							IsFromSignUp = true,
						};
						signupPage.RegistrationEntry.ServiceAddress = ServiceAddress;
						signupPage.Done = Done;
						Utils.PushAsync(Navigation, signupPage);
					}
				})
			});
        }

        void sendForgotPasswordForMail(string email)
        {
            AlertManager.ShowFullScreenSpinner();

            var isEmailAvailable = Shared.APIs.ILogins.IsUserEmailRegistered(email);

            if (isEmailAvailable)
            {
                var SecretQuestionID = Shared.APIs.ILogins.GetQuestionIDs(email);

                var SecretQuestion = Shared.APIs.ILogins.GetQuestion(email, SecretQuestionID);

                var questionID = SecretQuestion.ID;
                var Question = SecretQuestion.Question;

                AlertManager.HideFullScreenSpinner();

                AlertManager.Prompt(Question)
                            .ContinueWith((response) =>
                            {
                                var answer = response.Result;
                                
                                AlertManager.ShowFullScreenSpinner();

                                var secret_question =new LostPasswordData();

                                secret_question.NewPassword = "";
                                secret_question.QuestionID = questionID;
                                secret_question.Answer = answer;

                                var checkSecretAnswer = Shared.APIs.ILogins.IsAnswerCorrect(email, secret_question);

                                var SecretFlag = checkSecretAnswer.IsOkToLogin;
                                if(SecretFlag){
                                    AlertManager.HideFullScreenSpinner();

                                    AlertManager.Prompt("New Password", true)
                                                   .ContinueWith((response1) =>
                                                   {
                                                       var new_password = response1.Result;

                                                       AlertManager.ShowFullScreenSpinner();

                                                       var secret_question1 = new LostPasswordData();

                                                       secret_question1.NewPassword = new_password;
                                                       secret_question1.QuestionID = questionID;
                                                       secret_question1.Answer = answer;

                                                       var result = Shared.APIs.ILogins.SetLostPassword(email, secret_question1);

                                                       var token = result.Token;

                                                       if (token != null)
                                                       {
                                                           Shared.LoginToken = token;
                                                           AlertManager.HideFullScreenSpinner();
                                                           AlertManager.ShowAlert("Alert", "Your password has been updated.", "OK", this);
                                                        }else {
                                                           AlertManager.HideFullScreenSpinner();
                                                           AlertManager.ShowAlert("Error", "Token Error.", "OK", this);
                                                        }
                                                   });
                                } else {
                                    AlertManager.HideFullScreenSpinner();
                                    AlertManager.ShowAlert("Error", "The answer is not correct.", "OK", this);
                                }
                                
                            });
                                           
            } else {
                AlertManager.HideFullScreenSpinner();
                AlertManager.ShowAlert("Error", "The email you have choosen is not in use.", "OK", this);
            }

        }

		public void ShowBack(CustomNavigationPage customNavigationPage)
		{
			this.Navigation.InsertPageBefore(new Page(), this);
			customNavigationPage.PopView = (page) =>
			{
				if (page == this)
				{
					Back.Invoke();
					return true;
				}
				return false;
			};
		}

        private void Button_Clicked(object sender, EventArgs e)
        {
			var isSuccessed = false;
            this.IsBusy = true;
			(sender as VisualElement).IsEnabled = false;

			Task.Run(() =>
			{
				try
				{
					var loginResult = Shared.APIs.ILogins.GetLoginToken(emailEntry.Text, passwordEntry.Text);
					if (loginResult != null)
					{
						if (loginResult.Token != null && loginResult.OK)
						{
							Shared.LoginToken = loginResult.Token;
							Shared.LocalAddress = ServiceAddress;

							var userModel = Shared.APIs.IUsers.GetCurrentUser();
							UserModel = userModel;
							Shared.UserId = userModel.ID;

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
									bool isDeviceTokenSet = Shared.APIs.IUsers.AddDeviceToken(userModel.ID, platform, notificationId, uniqueDeviceId);
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

							isSuccessed = true;
							System.Diagnostics.Debug.WriteLine("UserID : " + UserModel.ID + " UserName : " + UserModel.Username);
						}
						else
						{
							Device.BeginInvokeOnMainThread(() =>
							{
								(sender as VisualElement).IsEnabled = true;
								Utils.ShowErrorMessage(loginResult.Message);
							});
						}
					}
				}
				catch (Exception ex)
				{
					Device.BeginInvokeOnMainThread(() =>
					{
						(sender as VisualElement).IsEnabled = true;
						Utils.ShowErrorMessage(ex);
					});
				}
			}).ContinueWith(t =>
			{
				if (isSuccessed)
				{
					Device.BeginInvokeOnMainThread(() =>
					{
                        this.IsBusy = false;
						if (this.Done != null)
						{
							this.Done(UserModel);
						}
						else
						{
							AddressSuggestionListPage addressSuggestionListPage = new AddressSuggestionListPage(null, (obj) =>
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
						}
					});
				}
				else 
				{
					(sender as VisualElement).IsEnabled = true;
					this.IsBusy = false;
				}
			}, TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}
