using System;
namespace ColonyConcierge.Mobile.Customer
{
	public interface IReloadPage
	{
		bool IsErrorPage { get; set; }
		
		void ReloadPage();
		void ShowLoadErrorPage();
	}
}
