using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ColonyConcierge.APIData.Data;
using ColonyConcierge.Mobile.Customer.Localization.Resx;
using Plugin.Toasts;
using Xamarin.Forms;
using System.Linq;
using Xamarin.Forms.Xaml;

namespace ColonyConcierge.Mobile.Customer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ShoppingPage : ContentPageBase
	{
		private List<Service> Services = new List<Service>();
		private ExtendedAddress mServiceAddress;
		private AddressFacade mAddressFacade = new AddressFacade();
		private List<ColonyConcierge.APIData.Data.ShoppingStore> mShoppingStores = new List<APIData.Data.ShoppingStore>();
		private ShoppingList ShoppingList = new ShoppingList();
		ScheduledShopping mScheduledShopping = new ScheduledShopping();
		ShoppingStore mShoppingStore = null;

		public ShoppingPage()
		{
			InitializeComponent();

			NavigationPage.SetBackButtonTitle(this, AppResources.Back);

			LabelProvideComment.IsVisible = string.IsNullOrEmpty(EntryProvideComment.Text);
			EntryProvideComment.TextChanged += (sender, e) =>
			{
				LabelProvideComment.IsVisible = string.IsNullOrEmpty(EntryProvideComment.Text);
			};

			GridShoppingList.SizeChanged += (sender, e) =>
			{
				if (GridShoppingList.Height > 1)
				{
					GridCheckout.IsVisible = true;
					GridCheckout.HeightRequest = GridShoppingList.Height;
				}
				//if (ImageGreenButton.Height > 1 && ImageGreenButton.Width > 1)
				//{
				//	GridShoppingList.HeightRequest = ImageGreenButton.Height;
				//	GridGreenButton.WidthRequest = ImageGreenButton.Width;

				//	Task.Run(() =>
				//	{
				//		new System.Threading.ManualResetEvent(false).WaitOne(50);
				//	}).ContinueWith(t =>
				//	{
				//		GridGreenButton.IsVisible = true;
				//	}, TaskScheduler.FromCurrentSynchronizationContext());
				//}
			};

			mServiceAddress = mAddressFacade.GetUserAddress();
			if (mServiceAddress != null)
			{
				apartmentUnitEntry.Text = mServiceAddress.BasicAddress.Line2;
				addressEntry.Text = mServiceAddress.BasicAddress.ToAddress();
			}
			else
			{
				apartmentUnitEntry.Text = string.Empty;
				addressEntry.Text = string.Empty;
			}

			addressEntry.AddressChanged += (sender, e) =>
			{
				mServiceAddress = e;
				if (mServiceAddress != null)
				{
					addressEntry.Text = mServiceAddress.BasicAddress.ToAddress();
					apartmentUnitEntry.Text = mServiceAddress.BasicAddress.Line2;
				}
				else
				{
					addressEntry.Text = string.Empty;
					apartmentUnitEntry.Text = string.Empty;
				}
				LoadData();
			};
			apartmentUnitEntry.TextChanged += (sender, e) =>
			{
				if (mServiceAddress != null)
				{
					mServiceAddress.BasicAddress.Line2 = apartmentUnitEntry.Text;
				}
			};

			GridShoppingList.IsVisible = CheckCheckout();

			DatePickerService.Date = DateTime.Now.Date.AddDays(1);
			DatePickerService.MinimumDate = DateTime.Now.Date;
			DatePickerService.Format = "MM/dd/yyyy";
			DatePickerService.DateSelected += (sender, e) =>
			{
				if (DateTime.Now.Date == e.NewDate && DateTime.Now.Hour >= 18)
				{
					Utils.ShowWarningMessage(AppResources.ShoppingErrorDateMessage, 7);
					DatePickerService.Date = DateTime.Now.Date.AddDays(1);
				}
			};

			StackLayoutShoppingPreferances.GestureRecognizers.Add(new TapGestureRecognizer
			{
				Command = new Command(() =>
				{
					UserFacade userFacade = new UserFacade();
					userFacade.RequireLogin(this, () =>
					{
						var myShoppingPrefPage = new MyShoppingPrefPage();
						myShoppingPrefPage.ShoppingPreferenceChanged += (sender, e) =>
						{
							LabelShoppingPreferancesValue.Text = AppResources.Changed;
						};
						Utils.PushAsync(Navigation, myShoppingPrefPage, true);
					});
				})
			});

			GridShoppingList.GestureRecognizers.Add(new TapGestureRecognizer
			{
				Command = new Command(async () =>
				{
					mShoppingStore = null;
					if (PickerStorePreferances.SelectedIndex > 0)
					{
						mShoppingStore = mShoppingStores[PickerStorePreferances.SelectedIndex - 1];
					}

					var serviceDate = new SimpleDate(DatePickerService.Date.Year, DatePickerService.Date.Month, DatePickerService.Date.Day);
					var isWeekly = SwitchServiceWeekly.IsToggled;
					var specialInstructions = EntryProvideComment.Text;

					var shoppingListPage = new ShoppingListPage(ShoppingList, Services, mShoppingStore, mServiceAddress, serviceDate, isWeekly, specialInstructions);
					shoppingListPage.Done += (sender, e) =>
					{
						if (sender is Page)
						{
							(sender as Page).Navigation.RemovePage(this);
						}

						//ShoppingList.Items = e.Items;
						//ShoppingList.Name = e.Name;
						//ShoppingList.Browseable = e.Browseable;
						//GridShoppingList.IsVisible = CheckCheckout();
					};
					await Utils.PushAsync(Navigation, shoppingListPage, true);
				}),
			});

			LoadData();
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			GridShoppingList.IsVisible = CheckCheckout();
		}

		public override void ReloadPage()
		{
			base.ReloadPage();
			LoadData();
		}

		public void LoadData()
		{
			this.IsBusy = true;
			PickerStorePreferances.Items.Clear();
			PickerStorePreferances.Items.Add(AppResources.NoPreference);
			PickerStorePreferances.SelectedIndex = 0;

			Task.Run(() =>
			{
				Utils.IReloadPageCurrent = this;
				try
				{
					var zipCode = mServiceAddress.BasicAddress.ZipCode;
					if (Shared.IsLoggedIn)
					{
						Services = Shared.APIs.IServices.GetAvailableServicesForUser(Shared.UserId, null, null, zipCode);
					}
					else
					{
						Services = Shared.APIs.IServices.GetAvailableServices(zipCode);
					}
					mShoppingStores = Shared.APIs.IShoppingLists.GetStores(mServiceAddress.BasicAddress.ZipCode);
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
			}).ContinueWith(t =>
			{
				if (Services != null)
				{
					Services = Services
									 .Where(s => s.ServiceType == APIData.Data.ServiceTypes.Shopping)
									 .Where(s => s.ServiceKind == ServiceKindCodes.Shopping_Grocery)
									 .ToList();

					if (mShoppingStores != null)
					{
						var storeNames = mShoppingStores.Select(s => s.DisplayName).ToList();
						foreach (var storeName in storeNames)
						{
							PickerStorePreferances.Items.Add(storeName);
						}
					}
					GridShoppingList.IsVisible = CheckCheckout();
				}
				this.IsBusy = false;
			}, TaskScheduler.FromCurrentSynchronizationContext());
		}

		public bool CheckCheckout()
		{
			bool check = true;

			var count = ShoppingList.Items.Sum(t => t.Quantity);
			if (count >= 1)
			{
				LabelItems.Text = count + " " + (count == 1 ? AppResources.Item : AppResources.ItemsUp);
			}
			else 
			{
				LabelItems.Text =  AppResources.AddItem; //AppResources.YourCartIsEmpty;
			}

			//if (count == 0)
			//{
			//	check = false;
			//}
			if (string.IsNullOrEmpty(addressEntry.Text))
			{
				check = false;
			}
			return check;
		}
	}
}
