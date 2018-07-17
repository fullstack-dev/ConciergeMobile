using ColonyConcierge.APIData.Data;
using ColonyConcierge.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using ColonyConcierge.Mobile.Customer.Localization.Resx;
using Xamarin.Forms.Xaml;

namespace ColonyConcierge.Mobile.Customer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ServiceKindList : ContentPageBase
    {
        public ServiceKindList(List<Service> services = null)
        {
            InitializeComponent();

			NavigationPage.SetBackButtonTitle(this, AppResources.Back);

			//StackLayoutBack.GestureRecognizers.Add(new TapGestureRecognizer()
			//{
			//    Command = new Command(() =>
			//    {
			//        OnBackButtonPressed();
			//    })
			//});

			ListViewServices.ItemTapped += (sender, e) =>
			{
				ServiceKindItemViewModel serviceKindItemViewModel = e.Item as ServiceKindItemViewModel;
				if (serviceKindItemViewModel.Model.ServiceType == ServiceTypes.PersonalServices)
				{
					var personalPage = new PersonalPage(serviceKindItemViewModel.Model.ServiceKind);
					Utils.PushAsync(Navigation, personalPage, true);
				}

				ListViewServices.SelectedItem = null;
			};

            LoadData(services);
        }

        public void LoadData(List<Service> services)
        {
			ListViewServices.ItemsSource = services
				.Select(t => new ServiceKindItemViewModel(t))
				.ToList();
        }
    }
}
