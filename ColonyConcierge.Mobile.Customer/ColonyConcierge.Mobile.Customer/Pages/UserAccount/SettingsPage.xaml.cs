using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ColonyConcierge.Mobile.Customer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPageBase
	{
		public SettingsPage()
		{
			InitializeComponent();

			EntryToken.Text = Shared.NotificationToken;
		}

		private void Save_button_clicked(object sender, EventArgs e)
		{
			
		}
	}
}
