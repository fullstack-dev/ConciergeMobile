using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ColonyConcierge.Mobile.Customer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RestaurantMenuItemView : ContentView
	{
		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged();
			if (this.BindingContext is MenuItemViewModel)
			{
				var menuItemViewModel = this.BindingContext as MenuItemViewModel;
				this.LabelDescription.Text = menuItemViewModel.Description;
				this.LabelDescription.IsVisible = menuItemViewModel.IsDescription;
				//this.GridName.IsVisible = true;
				menuItemViewModel.IsSelected = menuItemViewModel.IsSelected;
			}
		}

		public RestaurantMenuItemView()
		{
			InitializeComponent();

            this.LabelDisplayPrice.HeightRequest = Device.GetNamedSize(NamedSize.Small, this.LabelDisplayPrice);
		}
	}
}
