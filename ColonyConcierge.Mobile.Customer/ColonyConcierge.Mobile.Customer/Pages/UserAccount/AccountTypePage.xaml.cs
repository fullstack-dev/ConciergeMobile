using ColonyConcierge.APIData.Data;
using ColonyConcierge.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using ColonyConcierge.Mobile.Customer.Localization.Resx;
using Xamarin.Forms.Xaml;

namespace ColonyConcierge.Mobile.Customer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AccountTypePage : ContentPageBase
    {
		private bool mFirstLoad = true;

		public bool IsBackAnimation
		{
			get;
			set;
		} = false;

		List<ColonyConcierge.APIData.Data.SubscriptionPlan> subscriptionPlans = new List<SubscriptionPlan>();

        public AccountTypePage()
        {
            InitializeComponent();

			ListViewSubscriptions.ItemTapped += (sender, e) =>
			{
				this.SelectedIndex = Plans.FindIndex(s => s == e.Item);
				if (SelectedIndex >= 0)
				{
					Plans[SelectedIndex].IsSelected = true;
				}
				ListViewSubscriptions.SelectedItem = null;
			};

			//radioButtons.CheckedChanged += (sender, e) =>
			//{
			//	ButtonNext.IsEnabled = radioButtons.SelectedIndex > -1;
			//};
			//ButtonNext.IsEnabled = radioButtons.SelectedIndex > -1;
        }

		public override void ReloadPage()
		{
			base.ReloadPage();

			LoadData();
		}

		public void LoadData()
		{
			this.IsBusy = true;
			Task.Run(() =>
			{
				Utils.IReloadPageCurrent = this;
				try
				{
					// Fetch account plans from API
					subscriptionPlans = Shared.APIs.ISubscriptions.GetPlans();
				}
				catch (Exception ex)
				{
					if (!this.IsErrorPage && Utils.IReloadPageCurrent == this)
					{
						Device.BeginInvokeOnMainThread(async () =>
						{
							await Utils.ShowErrorMessage(string.IsNullOrEmpty(ex.Message) ? AppResources.SomethingWentWrong : ex.Message);
						});
					}
				}
				if (Utils.IReloadPageCurrent == this)
				{
					Utils.IReloadPageCurrent = null;
				}
			}).ContinueWith((t) =>
			{
				this.IsBusy = false;
				if (subscriptionPlans != null)
				{
					Plans = subscriptionPlans.Select(s =>
					{
						return new SubscriptionPlanItemView(s);
					}).ToList();
					foreach (var plan in Plans)
					{
						plan.PropertyChanged += (sender, e) =>
						{
							if (e.PropertyName == nameof(plan.IsSelected) && plan.IsSelected)
							{
								foreach (var plan2 in Plans)
								{
									if (plan2 != plan)
									{
										plan2.IsSelected = false;
									}
								}
							}
							this.SelectedIndex = Plans.FindIndex(s => s.IsSelected);
							ButtonNext.IsEnabled = this.SelectedIndex > -1;
						};
					}
					this.OnPropertyChanged(nameof(SelectedIndex));
				}
			}, TaskScheduler.FromCurrentSynchronizationContext());
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

			if (mFirstLoad)
			{
				mFirstLoad = false;
				LoadData();
			}

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

        private List<SubscriptionPlanItemView> plans;

        public List<SubscriptionPlanItemView> Plans
        {
            get { return plans; }
            set
            {
                plans = value;
                this.OnPropertyChanged(nameof(Plans));
				// Add the plans as custom radio buttons

            }
        }

		public int SelectedIndex { get; set; } = -1;

        async private void ButtonNext_Clicked(object sender, EventArgs e)
        {
			if (Plans[this.SelectedIndex].SubscriptionPlan.Name.ToLower().Contains("annual"))
			{
				RegistrationEntry userInfo = (RegistrationEntry)this.BindingContext;
				userInfo.SubscriptionPlanID = Plans[this.SelectedIndex].SubscriptionPlan.ID;
				await Utils.PushAsync(Navigation, new AccountBrainTreePaymentMethodPage(true) { BindingContext = this.BindingContext }, true);
				IsBackAnimation = true;
			}
			else
			{
				RegistrationEntry userInfo = (RegistrationEntry)this.BindingContext;
				userInfo.SubscriptionPlanID = Plans[this.SelectedIndex].SubscriptionPlan.ID;
				await Utils.PushAsync(Navigation, new SecurityQuestionPage() { BindingContext = this.BindingContext }, true);
				IsBackAnimation = true;
			}

        }

        async private void ButtonBack_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync().ConfigureAwait(false);
        }
    }
}
