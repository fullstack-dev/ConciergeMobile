using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ColonyConcierge.Mobile.Customer.ViewModels
{
    public class MasterPageItemViewModel : BindableObject
    {
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
				return IsSelected ? AppearanceBase.Instance.OrangeColor : Color.Transparent;
			}
		}

		public string Title { get; set; }
        public string IconSource { get; set; }

        public Type TargetType { get; set; }
		public object Parameter { get; set; }
		public string Name { get; internal set; }
	}
}
