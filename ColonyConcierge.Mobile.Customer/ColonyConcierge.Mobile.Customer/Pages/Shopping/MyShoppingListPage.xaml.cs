using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ColonyConcierge.APIData.Data;
using Xamarin.Forms;
using System.Linq;
using Plugin.Toasts;
using ColonyConcierge.Mobile.Customer.Localization.Resx;
using Xamarin.Forms.Xaml;

namespace ColonyConcierge.Mobile.Customer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MyShoppingListPage : ContentPageBase
	{
		public EventHandler<List<ShoppingListItem>> Added;
		List<ShoppingList> ShoppingList = new List<APIData.Data.ShoppingList>();
		List<ShoppingListViewModel> ShoppingListItemViewModelList = new List<ShoppingListViewModel>();

		public MyShoppingListPage()
		{
			InitializeComponent();

			NavigationPage.SetBackButtonTitle(this, AppResources.Back);

			ListViewMyShopping.ItemTapped += (sender, e) =>
			{
				if (e.Item is ShoppingListViewModel)
				{
					(e.Item as ShoppingListViewModel).IsSelected = !(e.Item as ShoppingListViewModel).IsSelected;
				}
				ListViewMyShopping.SelectedItem = null;
				StackLayoutAddItem.IsVisible = ShoppingListItemViewModelList.Count(t => t.IsSelected) > 0;
			};

			ButtonAddItem.Clicked += (sender, e) =>
			{
				if (Added != null)
				{
					List<ShoppingListItem> shoppingListItems = new List<ShoppingListItem>();
					var shoppingListItemViewModelListSelected = ShoppingListItemViewModelList.Where(t => t.IsSelected);
					foreach (var shoppingListItemViewModel in shoppingListItemViewModelListSelected)
					{
						if (shoppingListItemViewModel.Model.Items != null)
						{
							shoppingListItems.AddRange(shoppingListItemViewModel.Model.Items);
						}
					}
					Added(this, shoppingListItems);
				}
				Navigation.PopAsync(true).ConfigureAwait(false);
			};

			LoadData();
		}

		public override void ReloadPage()
		{
			base.ReloadPage();
            LoadData();
		}

		public void LoadData()
		{
			UserFacade userFacade = new UserFacade();
			userFacade.RequireLogin(this, () =>
			{
				this.IsBusy = true;
				Task.Run(() =>
				{
					Utils.IReloadPageCurrent = this;
					try
					{
						var shoppingListIds = Shared.APIs.IUsers.GetShoppingListsIds(Shared.UserId);
						if (shoppingListIds != null)
						{
							foreach (var shoppingListId in shoppingListIds)
							{
								var shoppingList = Shared.APIs.IShoppingLists.GetShoppingList(shoppingListId);
								if (shoppingList != null)
								{
									if (shoppingList.Browseable)
									{
										ShoppingList.Add((shoppingList));
									}
								}
							}
						}
					}
					catch (Exception ex)
					{
						if (!this.IsErrorPage && Utils.IReloadPageCurrent == this)
						{
							Device.BeginInvokeOnMainThread(async () =>
							{
								await Utils.ShowErrorMessage(string.IsNullOrEmpty(ex.Message) ? AppResources.SomethingWentWrong : ex.Message);
							});
						}
					}
					if (Utils.IReloadPageCurrent == this)
					{
						Utils.IReloadPageCurrent = null;
					}
				}).ContinueWith((t) =>
				{
					if (ShoppingList != null)
					{
						ShoppingListItemViewModelList = ShoppingList.Select(s => new ShoppingListViewModel(s)).ToList();
						ListViewMyShopping.ItemsSource = ShoppingListItemViewModelList;
						LabelNoItems.IsVisible = ShoppingListItemViewModelList.Count == 0;
					}
					this.IsBusy = false;
				}, TaskScheduler.FromCurrentSynchronizationContext());
			});
		}
	}
}
