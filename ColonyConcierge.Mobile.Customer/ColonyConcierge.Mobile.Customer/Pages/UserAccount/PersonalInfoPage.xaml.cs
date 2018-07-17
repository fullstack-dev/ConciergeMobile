using ColonyConcierge.APIData.Data;
using ColonyConcierge.Client;
using ColonyConcierge.Mobile.Customer.Localization.Resx;
using PCLAppConfig;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TK.CustomMap.Api;
using TK.CustomMap.Api.Google;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace ColonyConcierge.Mobile.Customer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PersonalInfoPage : ContentPageBase, INotifyPropertyChanged
    {
		public bool IsBackAnimation
		{
			get;
			set;
		} = false;

        public PersonalInfoPage()
        {
            InitializeComponent();
        }

		protected override void OnAppearing()
		{
			base.OnAppearing();

			//if (this.Width > 0 && this.Height > 0)
			//{
			//	if (IsBackAnimation)
			//	{
			//		IsBackAnimation = false;
   //                 this.TranslationX = -this.Width;
			//		Animation animation = new Animation((s) =>
			//		{
			//			this.TranslationX = -this.Width * s;
			//		}, 1, 0);
			//		animation.Commit(this, "OnAppearing", 16, 500, Easing.Linear);
			//	}
			//	else
			//	{
			//		if (mFirstLoad)
			//		{
			//			mFirstLoad = false;
   //                     this.TranslationX = this.Width;
			//			Animation animation = new Animation((s) =>
			//			{
			//				this.TranslationX = this.Width * s;
			//			}, 1, 0);
			//			animation.Commit(this, "OnDisappearing", 16, 500, Easing.Linear);
			//		}

			//	}
			//}
		}

        async private void ButtonNext_Clicked(object sender, EventArgs e)
        {
			await Utils.PushAsync(Navigation, new AccountAddressTypePage(false) { BindingContext = this.BindingContext }, true);
			IsBackAnimation = true;
        }

        async private void ButtonBack_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync().ConfigureAwait(false);
        }
    }
}
