using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ColonyConcierge.Mobile.Customer.Localization.Resx;
using Plugin.Toasts;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ColonyConcierge.Mobile.Customer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RestaurantMenuItemSubModifierPage : ContentPageBase
	{
		private bool IsModifierItemViewChanging = false;
		List<SubMenuModifierItemViewModel> SubMenuModifierItemViews = new List<SubMenuModifierItemViewModel>();

		public ModifierItemViewModel ModifierItemView
		{
			get;
			set;
		}

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
					this.ToolbarItems.Clear();
					this.ToolbarItems.Add(new ToolbarItem()
					{
						Text = DisplayPrice,
					});
				}
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

		public RestaurantMenuItemSubModifierPage(RestaurantMenuItemModifierPage restaurantMenuItemModifierPage, ModifierItemViewModel modifierItemView)
		{
			ModifierItemView = modifierItemView;

			InitializeComponent();

			NavigationPage.SetBackButtonTitle(this, AppResources.Back);
			var subMenuModifierItemViewsSource = new List<SubMenuModifierItemViewModel>();

			foreach (var subMenuModifierGroup in modifierItemView.MenuModifierVM.SubMenuModifierGroups)
			{
				subMenuModifierItemViewsSource.Add(new SubMenuModifierItemViewModel(modifierItemView, subMenuModifierGroup));
				var subMenuModifierItemViews = modifierItemView.MenuModifierVM.SubMenuModifiers
				                                           .Where((t) => subMenuModifierGroup.ModifierIDs.Contains(t.MenuModifier.ID))
				                                           .Select(t => new SubMenuModifierItemViewModel(modifierItemView, subMenuModifierGroup, t)).ToList();
				subMenuModifierItemViewsSource.AddRange(subMenuModifierItemViews);
				SubMenuModifierItemViews.AddRange(subMenuModifierItemViews);
			}

			ModifierItemView.SubMenuModifierItemViews = SubMenuModifierItemViews;
			foreach (var subMenuModifierItemView in SubMenuModifierItemViews)
			{
				var subMenuModifierItemViews = SubMenuModifierItemViews
					.Where((t) => subMenuModifierItemView.SubMenuModifierGroup.ModifierIDs.Contains(t.SubMenuModifierVM.MenuModifier.ID))
					.ToList();

				subMenuModifierItemView.PropertyChanged += (sender, e) =>
				{
					if (e.PropertyName == nameof(subMenuModifierItemView.Quantity))
					{
						Price = restaurantMenuItemModifierPage.UpdatePriceUnSave(ModifierItemView);
						foreach (var modifierItemViewGroup in subMenuModifierItemViews)
						{
							modifierItemViewGroup.RefreshEnabled();
						}
						Task.Run(() => 
						{
							new System.Threading.ManualResetEvent(false).WaitOne(100);
						}).ContinueWith((s) =>
						{
							var itemsSelected = subMenuModifierItemViews.Where(t => t.IsSelected).ToList();
							var countSelected = itemsSelected.Sum(t => t.Quantity);
							var maxApplied = subMenuModifierItemView.SubMenuModifierGroup.MaxApplied;
							if (maxApplied < countSelected && subMenuModifierItemView.Quantity != subMenuModifierItemView.QuantityPrevious)
							{
								subMenuModifierItemView.Quantity = subMenuModifierItemView.QuantityPrevious;
							}
						}, TaskScheduler.FromCurrentSynchronizationContext());
					}
					if (!IsModifierItemViewChanging && e.PropertyName == nameof(subMenuModifierItemView.IsSelected))
					{
						IsModifierItemViewChanging = true;
						var minApplied = subMenuModifierItemView.SubMenuModifierGroup.MinApplied;
						var maxApplied = subMenuModifierItemView.SubMenuModifierGroup.MaxApplied;
						if (minApplied == 1 && subMenuModifierItemViews.Where(t => t.IsSelected).LongCount() == 0)
						{
							subMenuModifierItemView.IsSelected = (true);
						}
						if (subMenuModifierItemView.IsSelected)
						{
							if (maxApplied == 1)
							{
								foreach (var modifierItemViewGroup in subMenuModifierItemViews)
								{
									modifierItemViewGroup.IsSelected = (modifierItemViewGroup == subMenuModifierItemView);
								}
							}
						}
						foreach (var modifierItemViewGroup in subMenuModifierItemViews)
						{
							modifierItemViewGroup.RefreshEnabled();
						}

						Price = restaurantMenuItemModifierPage.UpdatePriceUnSave(ModifierItemView);

						IsModifierItemViewChanging = false;
					}
				};
			}

			ListViewSubModifiers.ItemsSource = subMenuModifierItemViewsSource;
			ListViewSubModifiers.ItemTapped += (sender, e) =>
			{
				if (e.Item != null)
				{
					ListViewSubModifiers.SelectedItem = null;
					SubMenuModifierItemViewModel subMenuModifierItemView = e.Item as SubMenuModifierItemViewModel;
					if (subMenuModifierItemView.IsEnabled)
					{
						subMenuModifierItemView.IsSelected = !subMenuModifierItemView.IsSelected;
					}
				}
			};

			Price = restaurantMenuItemModifierPage.UpdatePriceUnSave(ModifierItemView);

			ButtonDone.Clicked += async (sender, e) =>
			{
				string error = string.Empty;

				foreach (var subMenuModifierGroup in ModifierItemView.MenuModifierVM.SubMenuModifierGroups)
				{
					var minApplied = subMenuModifierGroup.MinApplied;
					var itemsSelected = SubMenuModifierItemViews
						.Where((t) => subMenuModifierGroup.ModifierIDs.Contains(t.SubMenuModifierVM.MenuModifier.ID))
						.Where(s => s.IsSelected).ToList();
					var countApplied = itemsSelected.Sum(t => t.Quantity);

					if (countApplied < minApplied)
					{
						if (!string.IsNullOrEmpty(error))
						{
							error += "\n";
						}
						error += string.Format(AppResources.ChooseRequired, minApplied + " " + subMenuModifierGroup.DisplayName + " -");
					}
				}

				if (string.IsNullOrEmpty(error))
				{
					ModifierItemView.IsSelected = true;
					ModifierItemView.Save();
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
			};
		}
	}
}
