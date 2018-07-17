using System;
using ColonyConcierge.APIData.Data;
using ColonyConcierge.Mobile.Customer.Localization.Resx;
using Xamarin.Forms;

namespace ColonyConcierge.Mobile.Customer
{
	public class SubscriptionPlanItemView : BindableObject
	{
		private bool mIsSelected;
		public bool IsSelected
		{
			get
			{
				return mIsSelected;
			}
			set
			{
				OnPropertyChanging(nameof(IsSelected));
				mIsSelected = value;
				OnPropertyChanged(nameof(IsSelected));
			}
		}

		public string DisplayName
		{
			get
			{
				string text = SubscriptionPlan.Name + " ("
			   + (SubscriptionPlan.AnnualPrice.HasValue ? ("$" + Math.Round(SubscriptionPlan.AnnualPrice.Value) + " / year") : AppResources.Variable)
											  + ")";
				return text;
			}
		}

		public string Description
		{
			get
			{
				string text = SubscriptionPlan.Description;
				return text;
			}
		}

		public SubscriptionPlan SubscriptionPlan
		{
			get;
			set;
		}

		public SubscriptionPlanItemView(SubscriptionPlan subscriptionPlan)
		{
			SubscriptionPlan = subscriptionPlan;
		}
	}
}
