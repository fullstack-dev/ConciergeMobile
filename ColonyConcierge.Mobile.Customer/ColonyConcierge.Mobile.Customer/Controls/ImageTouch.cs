using System;
using Xamarin.Forms;

namespace ColonyConcierge.Mobile.Customer
{
	public class ImageTouch : Image
	{
		public event EventHandler<bool> Touch;

		public void RaiseOnTouch(bool isTouch)
		{
			if (Touch != null)
			{
				Touch(this, isTouch);
			}
		}
	}
}
