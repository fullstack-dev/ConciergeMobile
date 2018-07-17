using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ColonyConcierge.Mobile.Customer.ViewModels
{
	public class IntroViewModel
	{
		public event EventHandler FinishAnimation;

		public string Title { get; set; }
		public string SubTitle { get; set; }
		public string Description { get; set; }
		public string Logo { get; set; }
		public string Image { get; set; }
		public string ImageBackground { get; set; }
		public Color BackgroundColor { get; set; } = Color.Transparent;

		public bool IsAnimation { get; set; } = true;
		public bool ShowLogin { get; set; }
		public View BottomView { get; set; }

		public IntroViewModel()
		{
		}

		public void RaiseOnFinishAnimation()
		{
			if (FinishAnimation != null)
			{
				IsAnimation = false;
				FinishAnimation(this, EventArgs.Empty);
			}
		}
	}
}
