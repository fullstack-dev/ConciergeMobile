using System;
using ColonyConcierge.APIData.Data;
using Xamarin.Forms;

namespace ColonyConcierge.Mobile.Customer
{
	public class OrderItemViewModel : BindableObject
	{
		public ScheduledService ScheduledService
		{
			get;
			set;
		}

		public string ServiceDate
		{
			get;
			set;
		}

		public string ServiceType
		{
			get;
			set;
		}

		public string Status
		{
			get;
			set;
		}

		public string SpecialDescription
		{
			get;
			set;
		}

		public bool SpecialDescriptionVisible
		{
			get
			{
				return !string.IsNullOrEmpty(SpecialDescription);
			}
		}

		public OrderItemViewModel()
		{
		}
	}
}
