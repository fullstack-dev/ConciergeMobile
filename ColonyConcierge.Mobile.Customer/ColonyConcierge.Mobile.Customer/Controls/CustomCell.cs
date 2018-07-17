using System;
using System.ComponentModel;
using Xamarin.Forms;

namespace ColonyConcierge.Mobile.Customer
{
	public class CustomCell : ViewCell
	{
		public bool IsNeedResize
		{
			get;
			set;
		}

		public bool TransparentHover
		{
			get;
			set;
		} = false;

		public Color ColorHover
		{
			get;
			set;
		} = Color.Transparent;

		public bool IsColorHover
		{
			get
			{
				return ColorHover != Color.Transparent;
			}
		}

		public CustomCell()
		{
			this.ForceUpdateSize();

			BindingForceUpdateSize();	
			this.BindingContextChanged += (sender, e) =>
			{
				BindingForceUpdateSize();
			};
		}

		private void BindingForceUpdateSize()
		{
			if (this.BindingContext is INotifyPropertyChanged)
			{
				(this.BindingContext as INotifyPropertyChanged).PropertyChanged += (sender2, e2) =>
				{
					if (e2.PropertyName == "ForceUpdateSize")
					{
						this.ForceUpdateSize();
					}
				};
			}
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
		}
	}
}
