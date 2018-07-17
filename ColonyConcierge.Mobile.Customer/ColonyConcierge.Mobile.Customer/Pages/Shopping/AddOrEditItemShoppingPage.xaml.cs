using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ColonyConcierge.APIData.Data;
using ColonyConcierge.Mobile.Customer.Localization.Resx;
using Newtonsoft.Json;
using Plugin.Toasts;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ColonyConcierge.Mobile.Customer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddOrEditItemShoppingPage : ContentPageBase
	{
		private ShoppingPreference ShoppingPreference;

		public ShoppingListItem ShoppingListItem
		{
			get;
			set;
		} = new ShoppingListItem()
		{
			Quantity = 1,
			AllowLargerSize = false,
			AllowSmallerSize = false,
			BrandSubstitution = false,
			Generic = false,
			Product = new ShoppingProduct()
			{
			},
		};

		string mQuantity = null;
		public string Quantity
		{
			get
			{
				return mQuantity;
			}
			set
			{
				OnPropertyChanging(nameof(Quantity));
				if (!string.IsNullOrEmpty(value))
				{
					try
					{
						ShoppingListItem.Quantity = int.Parse(value);
						mQuantity = value;
					}
					catch (Exception)
					{
						Quantity = mQuantity;
					}
				}
				else
				{
					ShoppingListItem.Quantity = 0;
					mQuantity = string.Empty;
				}
				OnPropertyChanged(nameof(Quantity));
			}
		}

		ColonyConcierge.APIData.Data.User UserModel;
		public event EventHandler<ShoppingListItem> Done;

		public AddOrEditItemShoppingPage(ShoppingListItem shoppingListItem = null)
		{
			InitializeComponent();

			NavigationPage.SetBackButtonTitle(this, AppResources.Back);

			LabelProvideComment.IsVisible = string.IsNullOrEmpty(EntryProvideComment.Text);
			EntryProvideComment.TextChanged += (sender, e) =>
			{
                if (!string.IsNullOrEmpty(e.NewTextValue) && e.NewTextValue.Length > 64)
                {
                    EntryProvideComment.Text = e.NewTextValue.Substring(0, 64);
                }

                LabelProvideComment.IsVisible = string.IsNullOrEmpty(EntryProvideComment.Text);
			};

			EntryBrand.TextChanged += (sender, e) => 
			{
				if (!string.IsNullOrEmpty(e.NewTextValue) && e.NewTextValue.Length > 32)
				{
					EntryBrand.Text = e.NewTextValue.Substring(0, 32);
				}
			};

			EntryProductName.TextChanged += (sender, e) => 
			{
				if (!string.IsNullOrEmpty(e.NewTextValue) && e.NewTextValue.Length > 32)
				{
					EntryProductName.Text = e.NewTextValue.Substring(0, 32);
				}
			};

			EntryProductSizeWeight.TextChanged += (sender, e) => 
			{
				if (!string.IsNullOrEmpty(e.NewTextValue) && e.NewTextValue.Length > 32)
				{
					EntryProductSizeWeight.Text = e.NewTextValue.Substring(0, 32);
				}
			};

			ButtonAddItem.Clicked += (sender, e) =>
			{
				if (Done != null)
				{
					Done(this, ShoppingListItem);
				}
				Navigation.PopAsync(true).ConfigureAwait(false);
			};

			if (shoppingListItem != null)
			{
				ButtonAddItem.Text = AppResources.Update;
				this.Title = AppResources.Update;
				var jsonShoppingListItem = JsonConvert.SerializeObject(shoppingListItem);
				ShoppingListItem = JsonConvert.DeserializeObject<ShoppingListItem>(jsonShoppingListItem);
				this.Quantity = ShoppingListItem.Quantity.ToString();
				OnPropertyChanged(nameof(ShoppingListItem));
			}
			else 
			{
				ButtonAddItem.Text = AppResources.Add;

				if (Shared.IsLoggedIn)
				{
					this.IsBusy = true;
					Task.Run(() =>
					{
						Utils.IReloadPageCurrent = this;
						try
						{
							UserModel = Shared.APIs.IUsers.GetCurrentUser();
							if (UserModel != null)
							{
								ShoppingPreference = Shared.APIs.IAccounts.GetShoppingPreference(UserModel.AccountID);
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
						if (ShoppingPreference != null)
						{
							ShoppingListItem.AllowLargerSize = ShoppingPreference.DefaultCanSubstituteLarger;
							ShoppingListItem.AllowSmallerSize = ShoppingPreference.DefaultCanSubstituteSmaller;
							ShoppingListItem.BrandSubstitution = ShoppingPreference.DefaultBrandSubstitution;
							ShoppingListItem.Generic = ShoppingPreference.DefaultGenericSubstitution;
							OnPropertyChanged(nameof(ShoppingListItem));
						}
						this.IsBusy = false;
					}, TaskScheduler.FromCurrentSynchronizationContext());
				}
			}
		}
	}
}
