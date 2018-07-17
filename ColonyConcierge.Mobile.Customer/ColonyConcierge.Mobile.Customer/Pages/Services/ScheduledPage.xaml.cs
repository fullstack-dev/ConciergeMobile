using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ColonyConcierge.Mobile.Customer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ScheduledPage : ContentPage
    {
        public ScheduledPage()
        {
            InitializeComponent();
            this.ListViewScheduled.ItemTapped += ListViewScheduled_ItemTapped;
            this.ListViewScheduled.RefreshCommand = new Command(() =>
            {
                RefreshCommand.Execute(null);
            });
        }

        public Func<object, ItemTappedEventArgs, Task> ItemTapped { get; internal set; }
        private void ListViewScheduled_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            ItemTapped(sender, e);
        }

        public object SelectedItem
        {
            set
            {
                this.ListViewScheduled.SelectedItem = value;
            }
        }

        public IEnumerable<ScheduledServiceItemViewModel> ItemsSource
        {
            set
            {
                ListViewScheduled.ItemsSource = value;
                ImageEmpty.IsVisible = value.Count() == 0;
            }
        }
        
        
        public Command RefreshCommand { get; internal set; }
        public bool IsRefreshing
        {
            set
            {
                this.ListViewScheduled.IsRefreshing = value;
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