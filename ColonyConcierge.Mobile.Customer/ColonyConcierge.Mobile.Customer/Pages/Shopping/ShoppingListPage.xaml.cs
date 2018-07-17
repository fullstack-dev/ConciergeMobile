using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ColonyConcierge.APIData.Data;
using ColonyConcierge.Mobile.Customer.Localization.Resx;
using Xamarin.Forms;
using System.Linq;
using System.Threading.Tasks;
using Plugin.Toasts;
using Xamarin.Forms.Xaml;

namespace ColonyConcierge.Mobile.Customer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ShoppingListPage : ContentPageBase
	{
		private ShoppingList ShoppingList = new ShoppingList();
		private List<Service> Services = new List<Service>();
		SimpleDate mServiceDate;
		private ExtendedAddress mServiceAddress;
		ColonyConcierge.APIData.Data.User UserModel;
		List<ColonyConcierge.APIData.Data.CreditCardData> mPaymentAccountDatas = new List<APIData.Data.CreditCardData>();
		ScheduledShopping mScheduledShopping = new ScheduledShopping();
		ShoppingStore mShoppingStore = null;
		bool mServiceWeekly;
		string mSpecialInstructions;

		ObservableCollection<ShoppingListItemViewModel> ShoppingListItemViewModels = new ObservableCollection<ShoppingListItemViewModel>();

		public event EventHandler<ShoppingList> Done;

		public ShoppingListPage(ShoppingList shoppingList, List<Service> services, ShoppingStore shoppingStore, ExtendedAddress serviceAddress, SimpleDate serviceDate
		                       , bool serviceWeekly, string specialInstructions)
		{
			InitializeComponent();

			Services = services;
			mServiceAddress = serviceAddress;
			ShoppingList = shoppingList;
			mShoppingStore = shoppingStore;
			mServiceDate = serviceDate;
			mServiceWeekly = serviceWeekly;
			mSpecialInstructions = specialInstructions;

			NavigationPage.SetBackButtonTitle(this, AppResources.Back);

			SwitchSaveShoppingList.Toggled += (sender, e) =>
			{
				ShoppingList.Browseable = SwitchSaveShoppingList.IsToggled;
				EntryShoppingListName.IsVisible = SwitchSaveShoppingList.IsToggled;
				(EntryShoppingListName.Parent as Layout).ForceLayout();
				ButtonCheckout.IsEnabled = CheckCheckout();
			};

			EntryShoppingListName.TextChanged += (sender, e) =>
			{
				ShoppingList.Name = EntryShoppingListName.Text;
				ButtonCheckout.IsEnabled = CheckCheckout();
			};

			SwitchSaveShoppingList.IsToggled = shoppingList.Browseable;
			EntryShoppingListName.Text = shoppingList.Name;
			EntryShoppingListName.IsVisible = SwitchSaveShoppingList.IsToggled;
			(EntryShoppingListName.Parent as Layout).ForceLayout();
			ButtonCheckout.IsEnabled = CheckCheckout();

			ShoppingListItemViewModels.CollectionChanged += (sender, e) =>
			{
				ImageEmpty.IsVisible = ShoppingListItemViewModels.Count == 0;
			};
			ImageEmpty.IsVisible = ShoppingListItemViewModels.Count == 0;

			if (shoppingList.Items != null)
			{
				foreach (var shoppingListItem in shoppingList.Items)
				{
					ShoppingListItemViewModels.Add(new ShoppingListItemViewModel(shoppingListItem, (obj) =>
					{
						ShoppingListItemViewModels.Remove(obj);
						var shoppingListItemsModel = ShoppingListItemViewModels.Select(t => t.Model).ToList();
						ShoppingList.Items = shoppingListItemsModel;
						ButtonCheckout.IsEnabled = CheckCheckout();
					}));
				}
			}

			ListViewShoppingListItem.ItemsSource = ShoppingListItemViewModels;
			ListViewShoppingListItem.ItemTapped += (sender, e) =>
			{
				if (e.Item is ShoppingListItemViewModel)
				{
					var shoppingListItemViewModel = e.Item as ShoppingListItemViewModel;
					var addItemShoppingPage = new AddOrEditItemShoppingPage(shoppingListItemViewModel.Model);
					addItemShoppingPage.Done += (sender2, e2) =>
					{                      
                        shoppingListItemViewModel.Model = e2;
						shoppingListItemViewModel.UpdateModel();

                        var shoppingListItemsModel = ShoppingListItemViewModels.Select(t => t.Model).ToList();
                        ShoppingList.Items = shoppingListItemsModel;
                    };
					Utils.PushAsync(Navigation, addItemShoppingPage, true);
				}

				ListViewShoppingListItem.SelectedItem = null;
			};

			LabelAddItems.GestureRecognizers.Add(new TapGestureRecognizer()
			{
				Command = new Command(() => 
				{
					var addItemShoppingPage = new AddOrEditItemShoppingPage();
					addItemShoppingPage.Done += (sender, e) =>
					{
						var shoppingListItemViewModel = ShoppingListItemViewModels.FirstOrDefault((arg) =>
						{
							return arg.Model.Product.Brand == e.Product.Brand
									  && arg.Model.Product.Name == e.Product.Name;
						});
						if (shoppingListItemViewModel == null)
						{
							ShoppingListItemViewModels.Insert(0, new ShoppingListItemViewModel(e, (obj) =>
							{
								ShoppingListItemViewModels.Remove(obj);
							}));
						}
						else
						{
							shoppingListItemViewModel.Model.Quantity += e.Quantity;
							shoppingListItemViewModel.UpdateModel();
						}
						var shoppingListItemsModel = ShoppingListItemViewModels.Select(t => t.Model).ToList();
						ShoppingList.Items = shoppingListItemsModel;
						ButtonCheckout.IsEnabled = CheckCheckout();
					};
					Utils.PushAsync(Navigation, addItemShoppingPage, true);
				})
			});

			ButtonCheckout.Clicked += (sender, e) =>
			{
				Checkout();
				//if (Done != null)
				//{
				//	Done(this, ShoppingList);
				//}
				//Navigation.PopAsync(true).ConfigureAwait(false);
			};

			StackLayoutFromMySavedLists.GestureRecognizers.Add(new TapGestureRecognizer()
			{
				Command = new Command(() =>
				{
					UserFacade userFacade = new UserFacade();
					userFacade.RequireLogin(this, () =>
					{
						var myShoppingListPage = new MyShoppingListPage();
						myShoppingListPage.Added += (sender, e) =>
						{
							foreach (ShoppingListItem shoppingListItem in e)
							{
								shoppingListItem.Product.ID = 0;
								var shoppingListItemViewModel = ShoppingListItemViewModels.FirstOrDefault((arg) =>
								{
									return arg.Model.Product.Brand == shoppingListItem.Product.Brand
											  && arg.Model.Product.Name == shoppingListItem.Product.Name;
								});
								if (shoppingListItemViewModel == null)
								{
									ShoppingListItemViewModels.Insert(0, new ShoppingListItemViewModel(shoppingListItem, (obj) =>
									{
										ShoppingListItemViewModels.Remove(obj);
									}));
								}
								else
								{
									shoppingListItemViewModel.Model.Quantity += shoppingListItem.Quantity;
									shoppingListItemViewModel.UpdateModel();
								}
							}
							var shoppingListItemsModel = ShoppingListItemViewModels.Select(t => t.Model).ToList();
							ShoppingList.Items = shoppingListItemsModel;
							ButtonCheckout.IsEnabled = CheckCheckout();
						};
						Utils.PushAsync(Navigation, myShoppingListPage, true);
					});
				})
			});
		}

		public void Checkout()
		{
			UserFacade userFacade = new UserFacade();
			userFacade.RequireLogin(this, () =>
			{
				this.IsBusy = true;
				bool isSucceed = false;
				mScheduledShopping = new ScheduledShopping();
				Task.Run(() =>
				{
					try
					{
						if (Services.FirstOrDefault() == null)
						{
							throw new Exception(AppResources.CanNotFoundService);
						}

						UserModel = Shared.APIs.IUsers.GetCurrentUser();
						var shoppingPreference = Shared.APIs.IAccounts.GetShoppingPreference(UserModel.AccountID);
						mScheduledShopping.Delivery = true;
						mScheduledShopping.ShoppingPreference = shoppingPreference;
						mScheduledShopping.Status = "";
						mScheduledShopping.ShoppingList = ShoppingList;
						mScheduledShopping.Type = ScheduledShopping.ShoppingType.Grocery;
						mScheduledShopping.IsWeekly = mServiceWeekly;
						mScheduledShopping.ServiceAddress = mServiceAddress;
						mScheduledShopping.ServiceID = Services.FirstOrDefault().ID;
						mScheduledShopping.ServiceDate = mServiceDate;
						mScheduledShopping.SpecialInstructions = mSpecialInstructions;
						if (mShoppingStore != null)
						{
							mScheduledShopping.ShoppingStoreID = mShoppingStore.ID;
						}
						//mPaymentAccountDatas = Shared.APIs.IAccounts.GetPaymentMethods(UserModel.AccountID);
						mPaymentAccountDatas = Shared.APIs.IAccounts.BtGetPaymentMethods(UserModel.AccountID);

						isSucceed = true;
					}
					catch (Exception ex)
					{
						Device.BeginInvokeOnMainThread(async () =>
						{
							var message = string.IsNullOrEmpty(ex.Message) ? AppResources.SomethingWentWrong : ex.Message;
							await Utils.ShowErrorMessage(message, 5);
						});
					}
				}).ContinueWith(async t =>
				{
					if (isSucceed)
					{
						if (mPaymentAccountDatas != null && mPaymentAccountDatas.Count > 0)
						{
							await GoSelectPaymentMethod();
						}
						else
						{
							var paymentMethodUpdatePage = new BrainTreePaymentMethodPage(true, (page, cardId) =>
							{
								Device.BeginInvokeOnMainThread(async () =>
								{
									var selectPaymentMethod = await GoSelectPaymentMethod();
									selectPaymentMethod.Navigation.RemovePage(page);
								});

								return true;
							});
							await Utils.PushAsync(Navigation, paymentMethodUpdatePage, true);
						}
					}
					this.IsBusy = false;
				}, TaskScheduler.FromCurrentSynchronizationContext());
			});
		}

		public async Task<SelectPaymentMethod> GoSelectPaymentMethod()
		{
			var shoppingSelectPaymentMethod = new SelectPaymentMethod(Services.FirstOrDefault(), mScheduledShopping);
			await Utils.PushAsync(Navigation, shoppingSelectPaymentMethod, true);
			return shoppingSelectPaymentMethod;
		}

		public bool CheckCheckout()
		{
			bool check = true;

			var count = ShoppingList.Items.Sum(t => t.Quantity);

			if (count == 0)
			{
				check = false;
			}

			if (SwitchSaveShoppingList.IsToggled && string.IsNullOrEmpty(EntryShoppingListName.Text))
			{
				check = false;
			}

			return check;
		}
	}
}
