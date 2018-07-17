using System;
using CarouselView.FormsPlugin.Abstractions;

namespace ColonyConcierge.Mobile.Customer
{
	public class CarouselViewControlCustom : CarouselViewControl
	{
		private double mMarginBottom = 0;
		public double MarginBottom
		{
			get
			{
				return mMarginBottom;
			}
			set
			{
				OnPropertyChanging(nameof(MarginBottom));
				mMarginBottom = value;
				OnPropertyChanged(nameof(MarginBottom));
			}
		}
	}
}
