using ColonyConcierge.APIData.Data;
using ColonyConcierge.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ColonyConcierge.Mobile.Customer.Localization.Resx;
using Plugin.Toasts;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ColonyConcierge.Mobile.Customer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MySubscriptionPage : ContentPageBase
	{
		List<SubscriptionPlan> subscriptionPlans = new List<SubscriptionPlan>();
		Account account = null;
		private bool mFirstLoad = true;

		public MySubscriptionPage()
		{
			InitializeComponent();

			//radioButtons.CheckedChanged += (sender, e) =>
			//{
			//	ButtonSave.IsEnabled = this.SelectedIndex > -1;
			//};
			ButtonSave.IsEnabled = this.SelectedIndex > -1;

			ListViewSubscriptions.ItemTapped += (sender, e) =>
			{
				this.SelectedIndex = Plans.FindIndex(s => s == e.Item);
				if (SelectedIndex >= 0)
				{
					Plans[SelectedIndex].IsSelected = true;
				}
				ListViewSubscriptions.SelectedItem = null;
			};
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			if (mFirstLoad)
			{
				mFirstLoad = false;
				LoadData();
			}
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
					var userModel = Shared.APIs.IUsers.GetCurrentUser();
					if (userModel != null)
					{
						account = Shared.APIs.IAccounts.GetAccount(userModel.AccountID);
					}
					subscriptionPlans = Shared.APIs.ISubscriptions.GetPlans();
				}
				catch (Exception ex)
				{
					if (!this.IsErrorPage && Utils.IReloadPageCurrent == this)
					{
						Device.BeginInvokeOnMainThread(() =>
						{
							Utils.ShowErrorMessage(ex);
						});
					}
				}
			}).ContinueWith((t) =>
			{
				if (Utils.IReloadPageCurrent == this)
				{
					Utils.IReloadPageCurrent = null;
				}
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
							ButtonSave.IsEnabled = this.SelectedIndex > -1;
						};
					}

					if (account != null)
					{
						this.SelectedIndex = Plans.FindIndex(s => s.SubscriptionPlan.ID == account.SubscriptionPlanID);
						if (SelectedIndex >= 0)
						{
							Plans[SelectedIndex].IsSelected = true;
						}
					}
					this.OnPropertyChanged(nameof(SelectedIndex));
				}
				this.IsBusy = false;
			}, TaskScheduler.FromCurrentSynchronizationContext());
		}

		private List<SubscriptionPlanItemView> plans = new List<SubscriptionPlanItemView>();

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

		async private void Button_Clicked(object sender, EventArgs e)
		{
			this.IsBusy = true;
			try
			{
				var userModel = Shared.APIs.IUsers.GetCurrentUser();
				this.SelectedIndex = Plans.FindIndex(s => s.IsSelected);
				int SubscriptionPlanID = Plans[this.SelectedIndex].SubscriptionPlan.ID;
				bool IsSubscription = await Task.Run(() => { return Shared.APIs.IAccounts.RequestPlanChange(userModel.AccountID, SubscriptionPlanID); });

				if (IsSubscription)
				{
					var notificator = DependencyService.Get<IToastNotificator>();
					await notificator.Notify(ToastNotificationType.Success, AppResources.UpdateSubscription, AppResources.SubscriptionUpdateSuccess, TimeSpan.FromSeconds(2));

					await Navigation.PopAsync().ConfigureAwait(false);
				}
				else
				{
					var notificator = DependencyService.Get<IToastNotificator>();
					await notificator.Notify(ToastNotificationType.Error, AppResources.UpdateSubscription, AppResources.SubscriptionUpdateError, TimeSpan.FromSeconds(2));
				}
			}
			catch (Exception ex)
			{
				var notificator = DependencyService.Get<IToastNotificator>();
				await notificator.Notify(ToastNotificationType.Error, AppResources.UpdateSubscription, ex.Message, TimeSpan.FromSeconds(2));
			}
			finally
			{
				this.IsBusy = false;
			}
		}
	}
}
