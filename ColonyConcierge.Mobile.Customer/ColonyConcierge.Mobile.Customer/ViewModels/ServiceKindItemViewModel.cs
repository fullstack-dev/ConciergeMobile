using ColonyConcierge.APIData.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ColonyConcierge.Mobile.Customer
{
    public class ServiceKindItemViewModel : BindableObject
    {        
		public Service Model
		{
			get;
			set;
		}

		public decimal? Fee
		{
			get
			{
				decimal? price = null;
				var fee = Model.Fees.FirstOrDefault();
				if (fee != null)
				{
					price = Math.Round(fee.Amount, 2);
				}
				return price;
			}
		}

		public bool HasFee
		{
			get
			{
				return Fee.HasValue;
			}
		}

		public string FeeDescription
		{
			get
			{
				string feeDescription = string.Empty;
				var fee = Model.Fees.FirstOrDefault();
				if (fee != null)
				{
					feeDescription = fee.Description;
				}
				return feeDescription;
			}
		}

		public ServiceKindItemViewModel(Service service)
		{
			Model = service;
		}
    }
}
