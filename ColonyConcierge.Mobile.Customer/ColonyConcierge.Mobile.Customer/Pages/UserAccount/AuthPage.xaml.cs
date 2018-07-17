using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColonyConcierge.APIData.Data;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ColonyConcierge.Mobile.Customer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AuthPage : TabbedPage
    {
		private Action<User> mDone;
		public Action<User> Done
		{
			get
			{
				return mDone;
			}
			set
			{
				mDone = value;
				SigninPage.Done = Done;
				SignupPage.Done = Done;
			}
		}

		public Action Back
		{
			get;
			set;
		}

		public AuthPage()
		{
			InitializeComponent();
		}

        public AuthPage(bool isSignup)
        {
            InitializeComponent();
			if (isSignup)
			{
				this.CurrentPage = SignUpNavigationPage;
			}
        }

		public AuthPage(ExtendedAddress serviceAddress)
		{
			InitializeComponent();

			this.SignupPage.RegistrationEntry.ServiceAddress = serviceAddress;
			this.SigninPage.ServiceAddress = serviceAddress;
		}

		public void ShowBack()
		{
			SignupPage.Navigation.InsertPageBefore(new Page(), SignupPage);
			SigninPage.Navigation.InsertPageBefore(new Page(), SigninPage);

			SignInNavigationPage.PopView = (page) =>
			{
				if(page == SigninPage)
				{
                    Back.Invoke();
					return true;
				}
				return false;
			};

			SignUpNavigationPage.PopView = (page) =>
			{
				if(page == SignupPage)
				{
                    Back.Invoke();
					return true;
				}
				return false;
			};
		}

		public void SetHasNavigationBarTab(bool isShowed)
		{
			NavigationPage.SetHasBackButton(SignupPage, isShowed);
			NavigationPage.SetHasBackButton(SigninPage, isShowed);
			NavigationPage.SetHasNavigationBar(SignupPage, isShowed);
			NavigationPage.SetHasNavigationBar(SigninPage, isShowed);
		}
	}
}
