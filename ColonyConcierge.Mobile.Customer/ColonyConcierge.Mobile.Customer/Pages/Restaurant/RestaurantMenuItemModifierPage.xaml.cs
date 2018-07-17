using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ColonyConcierge.Mobile.Customer.Localization.Resx;
using Xamarin.Forms;
using System.Linq;
using Plugin.Toasts;
using Xamarin.Forms.Xaml;

namespace ColonyConcierge.Mobile.Customer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RestaurantMenuItemModifierPage : ContentPageBase
	{
		private bool mFirstLoad = true;
		private MenuGroupModifierItem MenuGroupModifierItem = null;
		private MenuItemViewModel MenuItemView;
		private RestaurantFacade RestaurantFacade = new RestaurantFacade();
		private List<RMenuGroupModifierVM> MenuItemModifiers = new List<RMenuGroupModifierVM>();
		private List<ModifierItemViewModel> ModifierItemViews = new List<ModifierItemViewModel>();
		private List<ModifierItemViewModel> ModifierGroupItemViews = new List<ModifierItemViewModel>();

		private bool IsModifierItemViewChanging = false;

		public RestaurantVM RestaurantVM { get; set; }
		public MenuItemVM MenuItemVM { get; set; }
		public Action<MenuGroupModifierItem> Saved { get; set; }

		public decimal? mPrice;
		public decimal? Price
		{
			get
			{
				return mPrice;
			}
			set
			{
				if (mPrice != value)
				{
					OnPropertyChanging(nameof(Price));
					mPrice = value;
					OnPropertyChanged(nameof(Price));
					OnPropertyChanged(nameof(DisplayPrice));
					OnPropertyChanged(nameof(CartPriceText));
				}
			}
		}

		public string CartPriceText
		{
			get
			{
				return AppResources.AddToCart + " " + DisplayPrice;
			}
		}

		public string DisplayPrice
		{
			get
			{
				if (Price.HasValue && Price.Value > 0) return "$" + Price.Value.ToString("0.00");
				return string.Empty;
			}
		}

		private string mFeedback = string.Empty;
		public string Feedback
		{
			get
			{
				return mFeedback;
			}
			set
			{
				OnPropertyChanging(nameof(Feedback));
				mFeedback = value;
				OnPropertyChanged(nameof(Feedback));
			}
		}

		//public Command mFeedbackCommand;
		//public Command FeedbackCommand
		//{
		//	get
		//	{
		//		return mFeedbackCommand = mFeedbackCommand ?? new Command(() =>
		//		{
		//			GridFeedback.IsVisible = true;
		//			EditorFeedback.Focus();
		//		});
		//	}
		//}

		private int mQuantity;
		public int Quantity
		{
			get
			{
				//int quantity = 1;
				//int.TryParse(this.NumberEntryViewQuantity.Entry.Text, out quantity);
				//return quantity;
				return mQuantity;
			}
			set
			{
				mQuantity = value;
				OnPropertyChanged(nameof(Quantity));
				Device.BeginInvokeOnMainThread(() =>
				{
					Price = UpdatePrice(this.MenuItemVM, this.MenuItemModifiers, this.Quantity);
				});
			}
		}

		public async void SaveOrAddToCart()
		{
			if (this.IsBusy)
			{
				return;
			}

			string error = string.Empty;
			foreach (var menuItemModifier in MenuItemModifiers)
			{
				var minApplied = menuItemModifier.MenuModifierGroup.MinApplied;
				var itemsSelected = menuItemModifier.MenuModifiers.Where(s => s.IsSelected).ToList();
				var countApplied = itemsSelected.Sum(t => t.Quantity);
				if (countApplied < minApplied)
				{
					if (!string.IsNullOrEmpty(error))
					{
						error += "\n";
					}
					error += string.Format(AppResources.ChooseRequired, minApplied + " " + menuItemModifier.MenuModifierGroup.DisplayName + " -");
				}

				foreach (var itemSelected in itemsSelected)
				{
					foreach (var subMenuModifierGroup in itemSelected.SubMenuModifierGroups)
					{
						minApplied = subMenuModifierGroup.MinApplied;
						itemsSelected = itemSelected.SubMenuModifiers
							.Where((t) => subMenuModifierGroup.ModifierIDs.Contains(t.MenuModifier.ID))
							.Where(s => s.IsSelected).ToList();
						countApplied = itemsSelected.Sum(t => t.Quantity);

						if (countApplied < minApplied)
						{
							if (!string.IsNullOrEmpty(error))
							{
								error += "\n";
							}
							error += string.Format(AppResources.ChooseRequired, minApplied + " " + subMenuModifierGroup.DisplayName + " -");
						}
					}
				}
			}

			if (string.IsNullOrEmpty(error))
			{
				var menuGroupModifierItem = new MenuGroupModifierItem(MenuItemView, MenuItemModifiers, Quantity, this.Feedback);
				if (MenuGroupModifierItem == null)
				{
					MenuGroupModifierItem = menuGroupModifierItem;
					MenuItemView.ListMenuGroupModifierItemSelected.Add(MenuGroupModifierItem);
					MenuItemView.IsSelected = true;
				}
				else 
				{
					MenuGroupModifierItem.Comment = menuGroupModifierItem.Comment;
					MenuGroupModifierItem.ListRMenuGroupModifierVM = menuGroupModifierItem.ListRMenuGroupModifierVM.ToList();
					MenuGroupModifierItem.Quantity = menuGroupModifierItem.Quantity;
					MenuGroupModifierItem.MenuItemView = menuGroupModifierItem.MenuItemView;
				}
				if (Saved != null)
				{
					Saved.Invoke(MenuGroupModifierItem);
				}

				await Navigation.PopAsync().ConfigureAwait(false);
			}
			else
			{
				Device.BeginInvokeOnMainThread(async () =>
				{
					var notificator = DependencyService.Get<IToastNotificator>();
					await notificator.Notify(ToastNotificationType.Error, AppResources.Error, error, TimeSpan.FromSeconds(5));
				});
			}
		}

		public RestaurantMenuItemModifierPage(RestaurantVM restaurantVM, MenuItemViewModel menuItemView, MenuGroupModifierItem menuGroupModifierItem = null)
		{
			RestaurantVM = restaurantVM;
			MenuItemView = menuItemView;
			MenuItemVM = menuItemView.RestaurantMenuItem.MenuItemVM;
			MenuGroupModifierItem = menuGroupModifierItem;

			Price = MenuItemVM.MenuItem.BasePrice;

			InitializeComponent();

			NavigationPage.SetBackButtonTitle(this, AppResources.Back);

			//this.NumberEntryViewQuantity.Entry.Text = "1";
			this.Quantity = 1;

			//this.ToolbarItems.Add(new ToolbarItem()
			//{
			//	Icon = "check_white.png",
			//	Command = new Command(() => 
			//	{
			//		SaveOrAddToCart();
			//	}),
			//});

			if (MenuGroupModifierItem != null)
			{
				//this.NumberEntryViewQuantity.Entry.Text = Convert.ToString(MenuGroupModifierItem.Quantity);
                this.Quantity = MenuGroupModifierItem.Quantity;
				Feedback = MenuGroupModifierItem.Comment;
				ButtonAddToCart.Text = AppResources.Save;
			}

			ButtonAddToCart.Clicked += (sender, e) =>
			{
				SaveOrAddToCart();
			};

			//ButtonFeedbackDone.Clicked += (sender, e) =>
			//{
			//	EditorFeedback.Unfocus();
			//	GridFeedback.IsVisible = false;
			//};

			//this.NumberEntryViewQuantity.Entry.TextChanged += (sender, e) =>
			//{
			//	Device.BeginInvokeOnMainThread(() =>
			//	{
			//		Price = UpdatePrice(this.MenuItemVM, this.MenuItemModifiers, this.Quantity);
			//	});
			//};

			ListViewModifiers.ItemSelected += (sender, e) =>
			{
				if (e.SelectedItem != null)
				{
					ListViewModifiers.SelectedItem = null;
					ModifierItemViewModel modifierItemView = e.SelectedItem as ModifierItemViewModel;
					if (modifierItemView.IsItem && modifierItemView.IsEnabled)
					{
						if (modifierItemView.MenuModifierVM.SubMenuModifiers.Count > 0 && !modifierItemView.IsSelected)
						{
							RestaurantMenuItemSubModifierPage restaurantMenuItemSubModifierPage = new RestaurantMenuItemSubModifierPage(this, modifierItemView);
							Utils.PushAsync(Navigation, restaurantMenuItemSubModifierPage, true);
						}
						else
						{
							modifierItemView.IsSelected = !modifierItemView.IsSelected;
							modifierItemView.Save();
						}
					}
				}
			};
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			if (mFirstLoad)
			{
				mFirstLoad = false;
				LoadData();
			}
		}

		public override void ReloadPage()
		{
			base.ReloadPage();

            LoadData();
		}

		public void LoadData()
		{
			this.IsBusy = true;

			Task.Run(() =>
			{
				Utils.IReloadPageCurrent = this;
				try
				{
					var RMenuItemVM = new RMenuItemVM { LocationId = RestaurantVM.LocationId, MenuId = MenuItemView.RestaurantMenuItem.MenuId, MenuItem = MenuItemVM.MenuItem };
					MenuItemModifiers = RestaurantFacade.GetMenuItemModifiers(RMenuItemVM);
					if (MenuGroupModifierItem != null)
					{
						var menuItemModifiersList = MenuItemModifiers.SelectMany(t => t.MenuModifiers);
						foreach (var rMenuGroupModifierVM in MenuGroupModifierItem.ListRMenuGroupModifierVM)
						{
							var menuModifiers = rMenuGroupModifierVM.MenuModifiers.ToList();
							foreach (var menuModifier in menuModifiers)
							{
								var menuItemModifier = menuItemModifiersList.FirstOrDefault(t => t.MenuModifier.ID == menuModifier.MenuModifier.ID);
								if (menuItemModifier != null)
								{
									menuItemModifier.IsSelected = menuModifier.IsSelected;
									menuItemModifier.Quantity = menuModifier.Quantity;
									menuItemModifier.MenuModifier.ApplyByDefault = false;
									foreach (var subMenuModifier in menuModifier.SubMenuModifiers)
									{
										var subMenuItemModifier = menuItemModifier.SubMenuModifiers.FirstOrDefault(t => t.MenuModifier.ID == subMenuModifier.MenuModifier.ID);
										if (subMenuItemModifier != null)
										{
											subMenuItemModifier.IsSelected = subMenuModifier.IsSelected;
											subMenuItemModifier.Quantity = subMenuModifier.Quantity;
											subMenuItemModifier.MenuModifier.ApplyByDefault = false;
										}
									}
								}
							}
						}
					}

					ModifierItemViews = new List<ModifierItemViewModel>();
					ModifierGroupItemViews = new List<ModifierItemViewModel>();
					foreach (var menuItemModifier in MenuItemModifiers)
					{
						var menuItemModifiers = menuItemModifier.MenuModifiers.Select(k => new ModifierItemViewModel(menuItemModifier, k)).ToList();
						ModifierGroupItemViews.Add(new ModifierItemViewModel(menuItemModifier));
						ModifierGroupItemViews.AddRange(menuItemModifiers);
						ModifierItemViews.AddRange(menuItemModifiers);
					}

					foreach (var modifierItemView in ModifierItemViews)
					{
						modifierItemView.SaveChanged += (sender, e) =>
						{
							Device.BeginInvokeOnMainThread(() =>
							{
								Price = UpdatePrice(this.MenuItemVM, this.MenuItemModifiers, this.Quantity);
							});
						};
						modifierItemView.PropertyChanged += (sender, e) =>
						{
							if (e.PropertyName == nameof(modifierItemView.Quantity))
							{
								Device.BeginInvokeOnMainThread(() =>
								{
									Price = UpdatePrice(this.MenuItemVM, this.MenuItemModifiers, this.Quantity);
								});
								var menuItemModifier = modifierItemView.MenuGroupModifierVM;
								var modifierItemViewsGroup = ModifierItemViews.Where(t => menuItemModifier.MenuModifiers.Contains(t.MenuModifierVM)).ToList();
								foreach (var modifierItemViewGroup in modifierItemViewsGroup)
								{
									modifierItemViewGroup.Refresh();
								}
								Task.Run(() =>
								{
									new System.Threading.ManualResetEvent(false).WaitOne(100);
								}).ContinueWith((s) =>
								{
									var itemsSelected = modifierItemView.MenuGroupModifierVM.MenuModifiers.Where(t => t.IsSelected).ToList();
									var countSelected = itemsSelected.Sum(t => t.Quantity);
									var maxApplied = modifierItemView.MenuGroupModifierVM.MenuModifierGroup.MaxApplied;
									if (maxApplied < countSelected && modifierItemView.Quantity != modifierItemView.QuantityPrevious)
									{
										modifierItemView.Quantity = modifierItemView.QuantityPrevious;
									}
								}, TaskScheduler.FromCurrentSynchronizationContext());
							}
							if (!IsModifierItemViewChanging && e.PropertyName == nameof(modifierItemView.IsSelected))
							{
								IsModifierItemViewChanging = true;
								var menuItemModifier = modifierItemView.MenuGroupModifierVM;
								var modifierItemViewsGroup = ModifierItemViews.Where(t => menuItemModifier.MenuModifiers.Contains(t.MenuModifierVM)).ToList();
								var minApplied = menuItemModifier.MenuModifierGroup.MinApplied;
								var maxApplied = menuItemModifier.MenuModifierGroup.MaxApplied;
								if (minApplied == 1 && modifierItemViewsGroup.Where(t => t.IsSelected).LongCount() == 0)
								{
									modifierItemView.IsSelected = true;
								}
								if (modifierItemView.IsSelected)
								{
									if (maxApplied == 1)
									{
										foreach (var modifierItemViewGroup in modifierItemViewsGroup)
										{
											modifierItemViewGroup.IsSelected = (modifierItemViewGroup == modifierItemView);
										}
									}
								}
								foreach (var modifierItemViewGroup in modifierItemViewsGroup)
								{
									modifierItemViewGroup.Refresh();
								}

								Device.BeginInvokeOnMainThread(() =>
								{
									Price = UpdatePrice(this.MenuItemVM, this.MenuItemModifiers, this.Quantity);
								});
								IsModifierItemViewChanging = false;
							}
						};
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
			}).ContinueWith(t =>
			{
				if (ModifierGroupItemViews == null)
				{
					ModifierGroupItemViews = new List<ModifierItemViewModel>();
				}
				ListViewModifiers.ItemsSource = ModifierGroupItemViews;
				ListViewModifiers.Footer = this;
				Device.BeginInvokeOnMainThread(() =>
				{
					Price = UpdatePrice(this.MenuItemVM, this.MenuItemModifiers, this.Quantity);
				});
				this.IsBusy = false;
			}, TaskScheduler.FromCurrentSynchronizationContext());
		}

		public static decimal? UpdatePrice(MenuItemVM menuItemVM, List<RMenuGroupModifierVM> menuItemModifiers, int quantityItems)
		{
			decimal? price = null;
			var menuModifiers = menuItemModifiers.SelectMany(s => s.MenuModifiers);

			if (menuItemVM.MenuItem.BasePrice.HasValue)
			{
				price = menuItemVM.MenuItem.BasePrice;
			}

			var menuModifierOverride = menuModifiers.FirstOrDefault(t => !t.Additive && t.IsSelected);
			if (menuModifierOverride != null)
			{
				if (menuModifierOverride.MenuModifier.AllowMultipleInstances)
				{
					var quantity = menuModifierOverride.Quantity;
					if (quantity > 0 && menuModifierOverride.MenuModifier.ApplyByDefault)
					{
						quantity--;
					}
					price = (menuModifierOverride.ModifierPrice * quantity);
				}
				else
				{
					price = menuModifierOverride.ModifierPrice;
				}
				foreach (var subMenuModifiers in menuModifierOverride.SubMenuModifiers)
				{
					if (subMenuModifiers.IsSelected)
					{
						if (subMenuModifiers.MenuModifier.AllowMultipleInstances)
						{
							if (subMenuModifiers.MenuModifier.Price.HasValue)
							{
								if (!price.HasValue)
								{
									price = 0;
								}
								var quantity = subMenuModifiers.Quantity;
								if (quantity > 0 && subMenuModifiers.MenuModifier.ApplyByDefault)
								{
									quantity--;
								}
								price += (subMenuModifiers.MenuModifier.Price * quantity);
							}
						}
						else
						{
							if (subMenuModifiers.MenuModifier.Price.HasValue)
							{
								if (!price.HasValue)
								{
									price = 0;
								}
								price += subMenuModifiers.MenuModifier.Price;
							}
						}
					}
				}
			}

			var menuModifiersAdditive = menuModifiers.Where(t => t.Additive && t.IsSelected);
			foreach (var menuModifierAdditive in menuModifiersAdditive)
			{
				if (menuModifierAdditive.MenuModifier.Price.HasValue)
				{
					if (menuModifierAdditive.MenuModifier.AllowMultipleInstances)
					{
						if (menuModifierAdditive.ModifierPrice.HasValue)
						{
							if (!price.HasValue)
							{
								price = 0;
							}
							var quantity = menuModifierAdditive.Quantity;
							if (quantity > 0 && menuModifierAdditive.MenuModifier.ApplyByDefault)
							{
								quantity--;
							}
							price += (menuModifierAdditive.ModifierPrice * quantity);
						}
					}
					else
					{
						if (menuModifierAdditive.ModifierPrice.HasValue)
						{
							if (!price.HasValue)
							{
								price = 0;
							}
							price += menuModifierAdditive.ModifierPrice;
						}
					}
				}
				foreach (var subMenuModifiers in menuModifierAdditive.SubMenuModifiers)
				{

					if (subMenuModifiers.IsSelected)
					{
						if (subMenuModifiers.MenuModifier.AllowMultipleInstances)
						{
							if (subMenuModifiers.MenuModifier.Price.HasValue)
							{
								if (!price.HasValue)
								{
									price = 0;
								}
								var quantity = subMenuModifiers.Quantity;
								if (quantity > 0 && subMenuModifiers.MenuModifier.ApplyByDefault)
								{
									quantity--;
								}
								price += (subMenuModifiers.MenuModifier.Price * quantity);
							}
						}
						else
						{
							if (subMenuModifiers.MenuModifier.Price.HasValue)
							{
								if (!price.HasValue)
								{
									price = 0;
								}
								price += subMenuModifiers.MenuModifier.Price;
							}
						}
					}
				}
			}

			if (!price.HasValue)
			{
				if (!menuItemVM.MenuItem.BasePrice.HasValue && menuModifiers.LongCount() > 0)
				{
					price = menuModifiers.Min(s => s.ModifierPrice);
				}
			}

			if (price.HasValue)
			{
				price = price * quantityItems;
			}

			return price;
		}


		public decimal? UpdatePriceUnSave(ModifierItemViewModel modifierItemView)
		{
			decimal? price = null;

			if (MenuItemVM.MenuItem.BasePrice.HasValue)
			{
				price = MenuItemVM.MenuItem.BasePrice;
			}

			var menuModifierOverride = ModifierItemViews.FirstOrDefault(t => !t.MenuModifierVM.Additive && (t.IsSelected ||modifierItemView == t));
			if (menuModifierOverride != null)
			{
				if (menuModifierOverride.MenuModifierVM.MenuModifier.AllowMultipleInstances)
				{
					var quantity = menuModifierOverride.Quantity;
					if (quantity > 0 && menuModifierOverride.MenuModifierVM.MenuModifier.ApplyByDefault)
					{
						quantity--;
					}
					price = (menuModifierOverride.MenuModifierVM.ModifierPrice * quantity);
				}
				else
				{
					price = menuModifierOverride.MenuModifierVM.ModifierPrice;
				}
				foreach (var subMenuModifiers in menuModifierOverride.SubMenuModifierItemViews)
				{
					if (subMenuModifiers.IsSelected)
					{
						if (subMenuModifiers.SubMenuModifierVM.MenuModifier.AllowMultipleInstances)
						{
							if (subMenuModifiers.SubMenuModifierVM.MenuModifier.Price.HasValue)
							{
								if (!price.HasValue)
								{
									price = 0;
								}
								var quantity = subMenuModifiers.Quantity;
								if (quantity > 0 && subMenuModifiers.SubMenuModifierVM.MenuModifier.ApplyByDefault)
								{
									quantity--;
								}
								price += (subMenuModifiers.SubMenuModifierVM.MenuModifier.Price * quantity);
							}
						}
						else
						{
							if (subMenuModifiers.SubMenuModifierVM.MenuModifier.Price.HasValue)
							{
								if (!price.HasValue)
								{
									price = 0;
								}
								price += subMenuModifiers.SubMenuModifierVM.MenuModifier.Price;
							}
						}
					}
				}
			}

			var menuModifiersAdditive = ModifierItemViews.Where(t => t.MenuModifierVM.Additive && (t.IsSelected || modifierItemView == t));
			foreach (var menuModifierAdditive in menuModifiersAdditive)
			{
				if (menuModifierAdditive.MenuModifierVM.MenuModifier.Price.HasValue)
				{
					if (menuModifierAdditive.MenuModifierVM.MenuModifier.AllowMultipleInstances)
					{
						if (menuModifierAdditive.MenuModifierVM.ModifierPrice.HasValue)
						{
							if (!price.HasValue)
							{
								price = 0;
							}
							var quantity = menuModifierAdditive.Quantity;
							if (quantity > 0 && menuModifierAdditive.MenuModifierVM.MenuModifier.ApplyByDefault)
							{
								quantity--;
							}
							price += (menuModifierAdditive.MenuModifierVM.ModifierPrice * quantity);
						}
					}
					else
					{
						if (menuModifierAdditive.MenuModifierVM.ModifierPrice.HasValue)
						{
							if (!price.HasValue)
							{
								price = 0;
							}
							price += menuModifierAdditive.MenuModifierVM.ModifierPrice;
						}
					}
				}
				foreach (var subMenuModifiers in menuModifierAdditive.SubMenuModifierItemViews)
				{

					if (subMenuModifiers.IsSelected)
					{
						if (subMenuModifiers.SubMenuModifierVM.MenuModifier.AllowMultipleInstances)
						{
							if (subMenuModifiers.SubMenuModifierVM.MenuModifier.Price.HasValue)
							{
								if (!price.HasValue)
								{
									price = 0;
								}
								var quantity = subMenuModifiers.Quantity;
								if (quantity > 0 && subMenuModifiers.SubMenuModifierVM.MenuModifier.ApplyByDefault)
								{
									quantity--;
								}
								price += (subMenuModifiers.SubMenuModifierVM.MenuModifier.Price * quantity);
							}
						}
						else
						{
							if (subMenuModifiers.SubMenuModifierVM.MenuModifier.Price.HasValue)
							{
								if (!price.HasValue)
								{
									price = 0;
								}
								price += subMenuModifiers.SubMenuModifierVM.MenuModifier.Price;
							}
						}
					}
				}
			}

			if (!price.HasValue)
			{
				if (!MenuItemVM.MenuItem.BasePrice.HasValue && ModifierItemViews.LongCount() > 0)
				{
					price = ModifierItemViews.Min(s => s.MenuModifierVM.ModifierPrice);
				}
			}

			if (price.HasValue)
			{
				price = price * this.Quantity;
			}

			return price;
		}
	}
}
