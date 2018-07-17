using ColonyConcierge.APIData.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using ColonyConcierge.Mobile.Customer.Localization.Resx;

namespace ColonyConcierge.Mobile.Customer
{
	public class ScheduledServiceItemViewModel : BindableObject
	{
		public ScheduledService Model
		{
			get;
			set;
		}

		public string Name
		{
			get
			{
				if (Model != null)
				{
					if (Model is ScheduledRestaurantService)
					{
						return Model.Name;
						//return (Model as ScheduledRestaurantService).Delivery?
      //                              AppResources.RestaurantDelivery : AppResources.RestaurantPickup;
					}
					return Model.Name;
				}
				return string.Empty;
			}
		}

		public string Time
		{
			get
			{
				if (Model != null)
				{
					if (Model is ScheduledRestaurantService)
					{
						var scheduledRestaurantService = (Model as ScheduledRestaurantService);
						var serviceStartTime = TimeZoneInfo.ConvertTime(DateTime.Parse(scheduledRestaurantService.ServiceStartTime.Time), TimeZoneInfo.Local);
						var serviceEndTime = TimeZoneInfo.ConvertTime(DateTime.Parse(scheduledRestaurantService.ServiceEndTime.Time), TimeZoneInfo.Local);
						return serviceStartTime.ToString("h:mm tt") + " - "
											   + serviceEndTime.ToString("h:mm tt");
					}
				}
				return string.Empty;
			}
		}

		public bool IsTimeShowed
		{
			get
			{
				return !string.IsNullOrEmpty(Time);
			}
		}

		public string Status
		{
			get
			{
				if (Model != null)
				{
					if (Model is ScheduledRestaurantService)
					{
						if (Model is ScheduledRestaurantService)
						{
							var scheduledRestaurantService = (Model as ScheduledRestaurantService);
							if (scheduledRestaurantService.Delivery)
							{
								if (RestaurantFacade.DeliveryStatus.ContainsKey(scheduledRestaurantService.Status))
								{
									return RestaurantFacade.DeliveryStatus[scheduledRestaurantService.Status];
								}
							}
						}
					}
				}
				return string.Empty;
			}
		}

		public bool IsStatusShowed
		{
			get
			{
				return !string.IsNullOrEmpty(Status);
			}
		}

		public ScheduledServiceItemViewModel(ScheduledService scheduledService)
		{
			Model = scheduledService;
		}

	}
}
