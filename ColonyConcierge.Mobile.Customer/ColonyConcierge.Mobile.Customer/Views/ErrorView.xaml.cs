using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ColonyConcierge.Mobile.Customer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ErrorView : ContentView
	{
		public event EventHandler TryAgain;

		public ErrorView()
		{
			InitializeComponent();

			GridTryAgain.GestureRecognizers.Add(new TapGestureRecognizer
			{
				Command = new Command(() =>
				{
					if (TryAgain != null)
					{
						TryAgain(this, EventArgs.Empty);
					}
				})
			});
		}
	}
}
