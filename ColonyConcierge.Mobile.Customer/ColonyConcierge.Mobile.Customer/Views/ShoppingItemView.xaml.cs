using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ColonyConcierge.Mobile.Customer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ShoppingItemView : ContentView
	{
		public event EventHandler Clicked;

		public ShoppingItemView()
		{
			InitializeComponent();
			this.GestureRecognizers.Add(new TapGestureRecognizer()
			{
				Command = new Command(() => 
				{
					if (Clicked != null)
					{
						Clicked(this, EventArgs.Empty);
					}
				})
			});
		}
	}
}
