using System;
using System.Collections.Generic;
using Xamarin.Forms.Maps;

namespace ColonyConcierge.Mobile.Customer
{
	public class AddressMap : Map
	{
		public event EventHandler<Position> RegionChanged;
		public CustomPin AddressPin { get; set; }
		public CustomPin CurrentPin { get; set; }
		public Position Center { get; set; }

		public AddressMap() : base()
		{
			this.IsShowingUser = false;
		}

		public void RaiseOnRegionChanged(Position position)
		{
			if (RegionChanged != null)
			{
				RegionChanged(this, position);
			}
		}
	}

	public class CustomPin
	{
		public Pin Pin { get; set; }
	}
}
