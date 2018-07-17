using System;
namespace ColonyConcierge.Mobile.Customer
{
	public interface IServicesTabPage
	{
		void SelectScheduleTab(bool isRefresh = false);
		void SelectServicesTab(bool isRefresh = false);
	}
}
