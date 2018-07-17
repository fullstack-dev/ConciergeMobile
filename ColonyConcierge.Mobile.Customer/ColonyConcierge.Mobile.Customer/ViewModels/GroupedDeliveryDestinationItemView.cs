using System;
using ColonyConcierge.APIData.Data;
using Xamarin.Forms;

namespace ColonyConcierge.Mobile.Customer
{
	public class GroupedDeliveryDestinationItemView : BindableObject
	{
		public string DisplayAddress
		{
			get
			{
				if (Model != null)
				{
					return Model.Address.BasicAddress.ToAddressLine();
				}
				return string.Empty;
			}
		}

		public GroupedDeliveryDestination Model
		{
			get;
			set;
		}

		private bool mIsSelected = false;
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
				OnPropertyChanged(nameof(BackgroundColor));
			}
		}

		public Color BackgroundColor
		{
			get
			{
				return IsSelected ? AppearanceBase.Instance.OrangeColor : AppearanceBase.Instance.PrimaryColor;
			}
		}

		public GroupedDeliveryDestinationItemView(GroupedDeliveryDestination groupedDeliveryDestination)
		{
			Model = groupedDeliveryDestination;
		}
	}
}
