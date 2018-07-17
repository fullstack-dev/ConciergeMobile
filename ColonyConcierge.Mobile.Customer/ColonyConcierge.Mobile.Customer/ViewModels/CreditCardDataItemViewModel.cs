using System;
using ColonyConcierge.APIData.Data;
using ColonyConcierge.Mobile.Customer.Localization.Resx;
using Xamarin.Forms;

namespace ColonyConcierge.Mobile.Customer
{
	public class CreditCardDataItemViewModel : BindableObject
	{
		public CreditCardData Model
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
			}
		}

		//public CreditCardType CardType
		//{
		//	get;
		//	set;
		//}

		public string CreditCardNumber
		{
			get
			{
				if (Model == null || string.IsNullOrEmpty(Model.CreditCardNumber))
				{
					return string.Empty;
				}
				var index = Model.CreditCardNumber.IndexOf("*", StringComparison.OrdinalIgnoreCase);
				if (index < Model.CreditCardNumber.Length - 1)
				{
					return Model.CreditCardNumber.Substring(index + 1);
				}
				return string.Empty;
			}
		}

		public string Preferred
		{
			get
			{
				return Model.IsPreferred ? AppResources.Preferred : string.Empty;
			}
		}


		public CreditCardDataItemViewModel(CreditCardData creditCardData)
		{
			Model = creditCardData;
			//CardType = DependencyService.Get<IAppServices>().GetCreditCardType(creditCardData.CreditCardNumber.Replace("*", "").Replace(" ", ""));
		}
	}
}
