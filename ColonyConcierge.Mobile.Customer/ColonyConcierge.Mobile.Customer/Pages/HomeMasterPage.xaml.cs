using ColonyConcierge.Mobile.Customer.Localization.Resx;
using ColonyConcierge.Mobile.Customer.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Version.Plugin;
using Xamarin.Forms.Xaml;

namespace ColonyConcierge.Mobile.Customer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomeMasterPage : ContentPageBase
	{
		bool IsLoggedIn = true;
		ColonyConcierge.APIData.Data.User userModel = null;
		IAppServices mAppServices;

		public HomeMasterPage()
		{
			InitializeComponent();
			IsLoggedIn = Shared.IsLoggedIn;
			mAppServices = DependencyService.Get<IAppServices>();

			LabelAppVersion.Text = "v" + mAppServices.AppVersion + " " + AppResources.AppVersion;

			this.SizeChanged += (sender, e) =>
			{
				ImageLogo.WidthRequest = this.Width * 0.6;
				if (this.Width >= 350)
				{
					LabelTermsConditions.FontSize = Device.GetNamedSize(NamedSize.Small, LabelTermsConditions);
					LabelSeparate.FontSize = Device.GetNamedSize(NamedSize.Small, LabelTermsConditions);
					LabelPrivacyPolicy.FontSize = Device.GetNamedSize(NamedSize.Small, LabelTermsConditions);
					LabelAppVersion.FontSize = Device.GetNamedSize(NamedSize.Small, LabelAppVersion);
				}
				else if (this.Width >= 290)
				{
					LabelTermsConditions.FontSize = Device.GetNamedSize(NamedSize.Small, LabelTermsConditions) * 0.9;
					LabelSeparate.FontSize = Device.GetNamedSize(NamedSize.Small, LabelTermsConditions) * 0.9;
					LabelPrivacyPolicy.FontSize = Device.GetNamedSize(NamedSize.Small, LabelTermsConditions) * 0.9;
					LabelAppVersion.FontSize = Device.GetNamedSize(NamedSize.Small, LabelTermsConditions) * 0.9;
				}
				else if (this.Width >= 250)
				{
					LabelTermsConditions.FontSize = Device.GetNamedSize(NamedSize.Small, LabelTermsConditions) * 0.8;
					LabelSeparate.FontSize = Device.GetNamedSize(NamedSize.Small, LabelTermsConditions) * 0.8;
					LabelPrivacyPolicy.FontSize = Device.GetNamedSize(NamedSize.Small, LabelTermsConditions) * 0.8;
					LabelAppVersion.FontSize = Device.GetNamedSize(NamedSize.Small, LabelTermsConditions) * 0.8;
				}
				else
				{
					LabelTermsConditions.FontSize = Device.GetNamedSize(NamedSize.Small, LabelTermsConditions) * 0.7;
					LabelSeparate.FontSize = Device.GetNamedSize(NamedSize.Small, LabelTermsConditions) * 0.7;
					LabelPrivacyPolicy.FontSize = Device.GetNamedSize(NamedSize.Small, LabelTermsConditions) * 0.7;
					LabelAppVersion.FontSize = Device.GetNamedSize(NamedSize.Small, LabelTermsConditions) * 0.7;
				}
			};

            LoadMenu();
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

           	ReloadData();
		}

		public void ReloadData()
		{
			if (IsLoggedIn != Shared.IsLoggedIn)
			{
				LoadData();
			}
		}

		public void LoadData()
		{
			IsLoggedIn = Shared.IsLoggedIn;

			if (!this.IsBusy)
			{
				LoadMenu();
				LoadUser();
			}
		}

		public void LoadUser()
		{
			if (Shared.IsLoggedIn)
			{
           		this.IsBusy = true;
				Task.Run(() =>
				{
					try
					{
						userModel = Shared.APIs.IUsers.GetCurrentUser();
					}
					catch (Exception)
					{
						userModel = null;
					}
				}).ContinueWith(t =>
				{
					this.IsBusy = false;
					if (userModel != null)
					{
						lbName.Text = userModel.FirstName + " " + userModel.LastName;
					}
					else
					{
						lbName.Text = string.Empty;
					}
				}, TaskScheduler.FromCurrentSynchronizationContext());
			}
		}

		public void LoadMenu()
		{
			IsLoggedIn = Shared.IsLoggedIn;

			this.Pages.Clear();

			var homeitem = new MasterPageItemViewModel
			{
				Title = AppResources.HomePageTitle,
				IconSource = "home_white.png",
				TargetType = (Device.RuntimePlatform == Device.iOS) ? typeof(ServicesNonTabPage) : typeof(ServicesTabPage)
			};
			this.Pages.Add(homeitem);

			if (Shared.IsLoggedIn)
			{
				this.Pages.Add(new MasterPageItemViewModel
				{
					Title = AppResources.OrderHistory,
					IconSource = "my_orders.png",
					TargetType = typeof(MyOrderPage)
				});
				this.Pages.Add(new MasterPageItemViewModel
				{
					Title = AppResources.ProfilePageTitle,
					IconSource = "my_profile.png",
					TargetType = typeof(ProfilePage)
				});
			}

			this.Pages.Add(new MasterPageItemViewModel
			{
				Title = AppResources.ServicesAndFeesPageTitle,
				IconSource = "services_fees.png",
				TargetType = typeof(WebViewPage),
				Parameter = PCLAppConfig.ConfigurationManager.AppSettings["ServciesUrl"]
			});
			this.Pages.Add(new MasterPageItemViewModel
			{
				Title = AppResources.SupportPageTitle,
				IconSource = "support.png",
				TargetType = typeof(WebViewPage),
				Parameter = PCLAppConfig.ConfigurationManager.AppSettings["ContactUrl"]
			});
			this.Pages.Add(new MasterPageItemViewModel
			{
				Title = AppResources.FAQPageTitle,
				IconSource = "faq.png",
				TargetType = typeof(WebViewPage),
				Parameter = PCLAppConfig.ConfigurationManager.AppSettings["FaqUrl"]
			});

			if (Shared.IsLoggedIn)
			{
				this.Pages.Add(new MasterPageItemViewModel
				{
					Title = AppResources.LogoutTitle,
					IconSource = "log_out.png",
					TargetType = typeof(AuthPage)
				});
			}
			else
			{
				this.Pages.Add(new MasterPageItemViewModel
				{
					Title = AppResources.SigninSignup,
					IconSource = "log_out.png",
					TargetType = typeof(AuthPage),
					Parameter = Shared.LocalAddress
				});
			}

			Device.BeginInvokeOnMainThread(() =>
			{
				this.ListView.SelectedItem = homeitem;
				homeitem.IsSelected = true;
			});
		}

		//private object mSelectedItem;
		//public object SelectedItem
		//{
		//	get { return mSelectedItem; }
		//	set
		//	{
		//		mSelectedItem = value;

		//		this.OnPropertyChanged(nameof(SelectedItem));
		//	}
		//}

        private ObservableCollection<MasterPageItemViewModel> pages = new ObservableCollection<MasterPageItemViewModel>();
        public ObservableCollection<MasterPageItemViewModel> Pages
        {
            get { return pages; }
            set
            {
                pages = value;

                this.OnPropertyChanged(nameof(Pages));
            }
        }
        public ListView ListView { get { return listView; } }

		public StackLayout Legal { get { return StackLayoutLegal; } }


		public Label PrivacyPolicy { get { return LabelPrivacyPolicy; } }
		public Label TermsConditions { get { return LabelTermsConditions; } }

    }
}
