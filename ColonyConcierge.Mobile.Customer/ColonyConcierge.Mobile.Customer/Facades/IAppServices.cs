using System;
using System.Threading.Tasks;
using ColonyConcierge.APIData.Data;
using ZXing.Mobile;

namespace ColonyConcierge.Mobile.Customer
{
	public interface IAppServices
	{	
		string AppVersion
		{
			get;
		}

		string UniqueDeviceId
		{
			get;
		}

		double LastLatitude
		{
			get;
			set;
		}

		double LastLongitude
		{
			get;
			set;
		}

		void Vibration(int milliseconds);
		string GetRegistrationNotificationId();

		void SetNetworkBar(bool isVisible);
		void SetShowStatus(bool isVisible);

		void GetLocation(Action<bool> action, bool isRefresh = false);
		void CheckLocationPermission(Action<bool> locationRequestAction);
		void RaiseAppOnLocationChanged(bool isLocation);

		void GetNoncenBrainTree(BrainTreeCard brainTreeCard, Action<object> action);
		CreditCardType GetCreditCardType(string cardNumber);

		TimeZoneInfo GetTimeZoneById(string id);

		Task<ZXing.Result> ScanQRCode();
		void ScanQRCodeContinuously(Func<IMobileBarcodeScanner, ZXing.Result, bool> action);

		void TrackPage(string page);
		void TrackEvent(string eventCategory, string eventAction, string eventLabel);
		void TrackOrder(string id, string affiliation);
		void TrackOrder(string id, string affiliation, double revenue, double shipping, double tax);

		void AddServiceNotAvaible(string id, string fullname, string email, ExtendedAddress address);
	}

	public class BrainTreeCard
	{
		public string Token
		{
			get;
			set;
		}

		public string CardNumber
		{
			get;
			set;
		}

		public string Cvv
		{
			get;
			set;
		}

		public string PostalCode
		{
			get;
			set;
		}

		public DateTime ExpirationDate
		{
			get;
			set;
		}

		public bool IsPreferred 
		{
			get; 
			set; 
		}

		/// <summary>
		/// User-visible name on the account payment method.
		/// </summary>
		public string AccountNickname { get; set; }

	}



	public enum CreditCardType
	{
		Amex,
		DinersClub,
		Discover,
		Jcb,
		Maestro,
		Mastercard,
		UnionPay,
		Unknown,
		Visa
	}
}
