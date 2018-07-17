using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ColonyConcierge.Mobile.Customer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ServicesPage : ContentPage
    {
        public ServicesPage()
        {
            InitializeComponent();

            this.ListViewServices.ItemTapped += ListViewServices_ItemTapped;
        }

        private void ListViewServices_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            ItemTapped(sender, e);
        }

        public IEnumerable<ServiceTypeItemViewModel> ItemsSource
        {
            set
            {
                this.ListViewServices.ItemsSource = value;
            }
        }

        public Func<object, ItemTappedEventArgs, Task> ItemTapped { get; internal set; }
        public object SelectedItem
        {
            set
            {
                this.ListViewServices.SelectedItem = value;
            }
        }

        public bool LoadingScheduled
        {
            set
            {
                _activityIndicator.IsEnabled = value;
                _activityIndicator.IsVisible = value;
            }
        }
    }
}