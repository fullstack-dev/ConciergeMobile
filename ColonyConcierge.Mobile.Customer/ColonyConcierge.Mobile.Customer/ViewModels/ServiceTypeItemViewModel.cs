using ColonyConcierge.APIData.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ColonyConcierge.Mobile.Customer
{
    public class ServiceTypeItemViewModel : BindableObject
    {        
		public Service Model  { get; set; }

        public ServiceTypes TypeName { get; set; }
		public ServiceKindCodes ServiceKind { get; set; }

		public string DisplayType { get; set; }


		public string PriceValue
		{
			get
			{
				if (Price.HasValue)
				{
					return Price.Value.ToString("0.00");
				}
				return string.Empty;
			}
		}

		public decimal? Price { get; set; }
		public bool HasPrice
		{
			get
			{
				return Price.HasValue;
			}
		}
		public string PriceDescription { get; set; }
        public string TypeDescription { get; set; }

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
				return IsSelected ? AppearanceBase.Instance.OrangeColor: AppearanceBase.Instance.PrimaryColor;
			}
		}


		public ServiceTypeItemViewModel(Service service)
		{
			Model = service;
		}
    }
}
