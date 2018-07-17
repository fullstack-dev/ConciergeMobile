using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ColonyConcierge.Mobile.Customer.Localization.Resx;
using Plugin.Toasts;
using Xamarin.Forms;
using System.Linq;
using ColonyConcierge.APIData.Data;
using Xamarin.Forms.Xaml;

namespace ColonyConcierge.Mobile.Customer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PlaceOrderPage : ContentPageBase
    {
        private bool mFirstLoad = true;

        public RestaurantDetailPage RestaurantDetailPage
        {
            get;
            set;
        }
        ExtendedAddress mServiceAddress;
        private IAppServices mAppServices;

        decimal TotalPrice = 0;
        List<ColonyConcierge.APIData.Data.CreditCardData> PaymentAccountDatas = new List<APIData.Data.CreditCardData>();
        ColonyConcierge.APIData.Data.User UserModel;
        //AddressFacade mAddressFacade = new AddressFacade();
        List<Service> Services = new List<Service>();
        CreditCardData CardSelected = null;
        private bool IsPauseTrigger = false;

        decimal SubPrice;
        decimal ServiceFee;
        decimal SalesTax;
        decimal Tips;
        decimal Discount;

        private void discountButton_Tapped(object sender, EventArgs e)
        {
            if (AppResources.Remove.Equals(discountButton.Text))
            {
                discountEntry.IsEnabled = true;
                discountEntry.Text = "";
                Discount = 0;

                discountButton.Text = AppResources.Apply;
                discountValue.Text = "$0.00";
                calculateTax();
                UpdatePrice();
            }

            string discountText = discountEntry.Text;
            if (String.IsNullOrEmpty(discountText))
            {
                return;
            }


            this.IsBusy = true;
            PendingDiscount discount = null;
            Task.Run(() =>
            {
                try
                {
                    discount = Shared.APIs.ICoupons.GetAvailableDiscount(discountText, Shared.UserId, Services.FirstOrDefault().ID, "");
                }
                catch (Exception ex)
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        discountValue.Text = "$0.00";
                        Discount = 0;
                        discountEntry.Text = "";
                        await Utils.ShowWarningMessage(string.IsNullOrEmpty(ex.Message) ? AppResources.SomethingWentWrong : ex.Message, 3);
                    });
                }
            }).ContinueWith((t) =>
            {
                decimal calculatedDiscount = 0;
                if (discount != null)
                {
                    if (discount.LimitToFees)
                    {
                        if (discount.DiscountPercentageAmount != null)
                        {
                            calculatedDiscount = (ServiceFee * discount.DiscountPercentageAmount.GetValueOrDefault());
                            if (discount.DiscountFlatAmount != null && calculatedDiscount > discount.DiscountFlatAmount)
                            {
                                calculatedDiscount = discount.DiscountFlatAmount.GetValueOrDefault();
                            }
                        }
                        else
                        {
                            calculatedDiscount = Math.Min(ServiceFee, discount.DiscountFlatAmount.GetValueOrDefault());
                        }

                    }
                    else
                    {
                        if (discount.DiscountPercentageAmount != null)
                        {
                            calculatedDiscount = (SubPrice * discount.DiscountPercentageAmount.GetValueOrDefault());
                            if (discount.DiscountFlatAmount != null && calculatedDiscount > discount.DiscountFlatAmount)
                            {
                                calculatedDiscount = discount.DiscountFlatAmount.GetValueOrDefault();
                            }
                        }
                        else
                        {
                            if (discount.DiscountAsCash)
                            {
                                calculatedDiscount = discount.DiscountFlatAmount.GetValueOrDefault();
                            }
                            else
                            {
                                SubPrice -= discount.DiscountFlatAmount.GetValueOrDefault();
                                calculatedDiscount = 0;
                                calculateTax();
                            }
                        }
                    }
                    discountValue.Text = "$" + calculatedDiscount.ToString("0.00");
                    Discount = calculatedDiscount;
                    discountButton.Text = AppResources.Remove;
                    discountEntry.IsEnabled = false;
                }

                UpdatePrice();
                this.IsBusy = false;
            }, TaskScheduler.FromCurrentSynchronizationContext());


        }

        public PlaceOrderPage(string comment, RestaurantDetailPage restaurantDetailPage, ExtendedAddress serviceAddress)
        {
            RestaurantDetailPage = restaurantDetailPage;
            mServiceAddress = serviceAddress;

            InitializeComponent();

            mAppServices = DependencyService.Get<IAppServices>();
            this.Title = restaurantDetailPage.RestaurantVM.SearchResultData.RestaurantDisplayName;

            var listMenuItemViews = RestaurantDetailPage.ListMenuItemViews.Values.SelectMany(t => t);
            var listMenuGroupModifierItemSelected = listMenuItemViews.SelectMany(t => t.ListMenuGroupModifierItemSelected).ToList();
            StackLayoutChangeCard.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(async () =>
                {
                    SelectCardPage selectCardPage = new SelectCardPage(CardSelected);
                    selectCardPage.CardSelected += (sender2, e2) =>
                    {
                        CardSelected = e2.Model;
                        UpdateCard();
                        var pages = Navigation.NavigationStack.Reverse().Skip(1).ToList();
                        foreach (var page in pages)
                        {
                            if (page != this)
                            {
                                Navigation.RemovePage(page);
                            }
                            else
                            {
                                break;
                            }
                        }
                        if (pages.Count > 0)
                        {
                            Navigation.PopAsync(true).ConfigureAwait(false);
                        }
                    };
                    await Utils.PushAsync(Navigation, selectCardPage, true);
                }),
            });

            ButtonCheckout.Clicked += (sender, e) =>
            {
                if (CardSelected != null)
                {
                    IsPauseTrigger = true;
                    var id = 0;
                    IsBusy = true;
                    this.ButtonCheckout.Text = AppResources.PlacingOrder;
                    var totalPrice = LabelTotal.Text;
                    var serviceFee = LabelServiceFee.Text;
                    var salesTax = LabelSalesTax.Text;

                    var scheduledService = new ColonyConcierge.APIData.Data.ScheduledRestaurantService();

                    Task.Run(() =>
                    {
                        try
                        {
                            if (Services.FirstOrDefault() == null)
                            {
                                throw new Exception(AppResources.CanNotFoundService);
                            }

                            var paymentAccountData = CardSelected;
                            //CreditCardData creditCardData = Shared.APIs.IAccounts.BtGetPaymentMethods(UserModel.AccountID).FirstOrDefault(t => t.ID == paymentAccountData.ID);
                            //if (!creditCardData.IsPreferred)
                            //{
                            //	creditCardData.ID = paymentAccountData.ID;
                            //	creditCardData.IsPreferred = true;
                            //	bool result = Shared.APIs.IAccounts.BtUpdatePaymentMethod(UserModel.AccountID, creditCardData.ID, creditCardData);
                            //	if (!result)
                            //	{
                            //		throw new Exception(AppResources.CanNotUseThisCard);
                            //	}
                            //}
                            scheduledService.PaymentMethodID = paymentAccountData.ID;
                            scheduledService.Delivery = restaurantDetailPage.IsDeliverySelected;
                            scheduledService.FrontEndSubtotal = SubPrice;
                            scheduledService.FrontEndTaxes = SalesTax;
                            scheduledService.DiscountCodes = discountEntry.Text;

                            scheduledService.RestaurantLocationID = restaurantDetailPage.RestaurantVM.LocationId;
                            scheduledService.Tip = Tips;
                            scheduledService.SpecialInstructions = comment;
                            scheduledService.ServiceID = Services.FirstOrDefault().ID;

                            var menuIds = RestaurantDetailPage.ListMenuItemViews.Values.SelectMany(t => t)
                                                  .SelectMany(t => t.ListMenuGroupModifierItemSelected)
                                                  .Select(s => s.MenuItemView.RestaurantMenuItem.MenuId).Distinct().ToList();
                            var timeslots = restaurantDetailPage.RefreshMenuAvailables.Where((arg) => restaurantDetailPage.GetDateTimeRestaurantFromLocal(arg.StartTime.Time) == restaurantDetailPage.SelectedDate);
                            var timeslot = timeslots
                                        .Where(s =>
                                        {
                                            return !menuIds.Except(s.RelatedMenuIDs).Any();
                                        }).FirstOrDefault();

                            if (timeslot != null)
                            {
                                scheduledService.SlotID = timeslot.ID;
                            }
                            else
                            {
                                scheduledService.SlotID = restaurantDetailPage.RMenuOrderingAvailableSlotSelected.ID;
                            }

                            //scheduledService.EntryDate = restaurantDetailPage.SelectedDate;
                            scheduledService.ServiceDate = new SimpleDate(restaurantDetailPage.SelectedDate.Year, restaurantDetailPage.SelectedDate.Month, restaurantDetailPage.SelectedDate.Day);
                            scheduledService.ServiceAddress = mServiceAddress;
                            if (restaurantDetailPage.GroupedDeliveryDestination != null)
                            {
                                scheduledService.DestinationID = restaurantDetailPage.GroupedDeliveryDestination.ID;
                            }

                            foreach (var menuGroupModifierItemSelected in listMenuGroupModifierItemSelected)
                            {
                                ScheduledRMenuItem scheduledRMenuItem = new ScheduledRMenuItem();
                                scheduledRMenuItem.Quantity = menuGroupModifierItemSelected.Quantity;
                                scheduledRMenuItem.DisplayName = menuGroupModifierItemSelected.MenuItemView.RestaurantMenuItem.DisplayName;
                                scheduledRMenuItem.FrontEndPrice = menuGroupModifierItemSelected.Price;
                                scheduledRMenuItem.RelatedMenuID = menuGroupModifierItemSelected.MenuItemView.RestaurantMenuItem.MenuId;
                                scheduledRMenuItem.RelatedMenuItemID = menuGroupModifierItemSelected.MenuItemView.RestaurantMenuItem.Id;
                                scheduledRMenuItem.SpecialInstructions = menuGroupModifierItemSelected.Comment;

                                foreach (var rMenuGroupModifierVM in menuGroupModifierItemSelected.ListRMenuGroupModifierVM)
                                {
                                    var menuModifiers = rMenuGroupModifierVM.MenuModifiers.Where(t => t.IsSelected).ToList();
                                    foreach (var menuModifier in menuModifiers)
                                    {
                                        var scheduledRMenuItemModifier = new ScheduledRMenuItemModifier();
                                        scheduledRMenuItemModifier.DisplayName = menuModifier.MenuModifier.DisplayName;
                                        scheduledRMenuItemModifier.Quantity = menuModifier.Quantity;
                                        scheduledRMenuItemModifier.RelatedModiferID = menuModifier.MenuModifier.ID;
                                        scheduledRMenuItem.AppliedModifiers.Add(scheduledRMenuItemModifier);
                                        var subMenuModifiers = menuModifier.SubMenuModifiers.Where(t => t.IsSelected).ToList();
                                        foreach (var subMenuModifier in subMenuModifiers)
                                        {
                                            var scheduledRMenuSubItemModifier = new ScheduledRMenuItemModifier();
                                            scheduledRMenuSubItemModifier.DisplayName = subMenuModifier.MenuModifier.DisplayName;
                                            scheduledRMenuSubItemModifier.Quantity = subMenuModifier.Quantity;
                                            scheduledRMenuSubItemModifier.RelatedModiferID = subMenuModifier.MenuModifier.ID;
                                            scheduledRMenuItem.AppliedModifiers.Add(scheduledRMenuSubItemModifier);
                                        }
                                    }
                                }
                                scheduledService.Items.Add(scheduledRMenuItem);
                            }
                            //var json = JsonConvert.SerializeObject(scheduledService);

                            id = Shared.APIs.IUsers.AddScheduledService(Shared.UserId, scheduledService);
                        }
                        catch (Exception ex)
                        {
                            Device.BeginInvokeOnMainThread(async () =>
                            {
                                var message = string.IsNullOrEmpty(ex.Message) ? AppResources.SomethingWentWrong : ex.Message;
                                await Utils.ShowErrorMessage(message, 5);
                            });
                        }
                    }).ContinueWith(t =>
                    {
                        if (id > 0)
                        {
                            mAppServices.TrackOrder(id.ToString(),
                                                    Services.FirstOrDefault().Name + "-" + restaurantDetailPage.RestaurantVM.LocationId.ToString(),
                                                    (double)TotalPrice, (double)ServiceFee, (double)SalesTax);
                            var notificator = DependencyService.Get<IToastNotificator>();
                            notificator.Notify(ToastNotificationType.Success, AppResources.PlaceOrder, AppResources.YourOrderSuccessfully, TimeSpan.FromSeconds(2));
                            //Navigation.PopToRootAsync(true).ConfigureAwait(false);
                            //var placedSuccessfullyPage = new PlacedSuccessfullyPage(scheduledService, RestaurantDetailPage, mServiceAddress);
                            var pages = Navigation.NavigationStack.Reverse().Skip(1).ToList();
                            IServicesTabPage servicesTabPage = null;
                            foreach (var page in pages)
                            {
                                //if (page.GetType() != typeof(RestaurantsTabPage) && page.GetType() != typeof(RestaurantsListPage))
                                if (page is IServicesTabPage)
                                {
                                    servicesTabPage = page as IServicesTabPage;
                                    break;
                                }
                                else
                                {
                                    Navigation.RemovePage(page);
                                }
                            }
                            if (servicesTabPage != null)
                            {
                                servicesTabPage.SelectScheduleTab(true);
                            }
                            Navigation.PopAsync(true).ConfigureAwait(false);

                            //await Utils.PushAsync(Navigation, placedSuccessfullyPage, true);
                        }
                        else
                        {
                            IsPauseTrigger = false;
                        }
                        this.ButtonCheckout.Text = AppResources.PlaceOrder;
                        IsBusy = false;
                    }, TaskScheduler.FromCurrentSynchronizationContext());
                }
            };

            SubPrice = 0;
            foreach (var menuGroupModifierItemSelected in listMenuGroupModifierItemSelected)
            {
                menuGroupModifierItemSelected.UpdatePrice();
                SubPrice += menuGroupModifierItemSelected.Price;
            }
            LabelSubtotal.Text = "$" + SubPrice.ToString("0.00");

            calculateTax();

            PickerTips.SelectedIndexChanged += (sender, e) =>
            {
                if (PickerTips.SelectedIndex >= 0)
                {
                    if (PickerTips.Items[PickerTips.SelectedIndex] == AppResources.Other)
                    {
                        EntryServiceTips.IsEnabled = true;
                        this.EntryServiceTips.NeedShowKeyboard = true;
                        Tips = 0;
                        decimal.TryParse(this.EntryServiceTips.Text, out Tips);
                    }
                    else
                    {
                        decimal percent = 0;
                        decimal.TryParse(PickerTips.Items[PickerTips.SelectedIndex].TrimEnd(new char[] { '%', ' ' }), out percent);
                        Tips = SubPrice * percent / 100;
                        EntryServiceTips.Text = Math.Round(Tips, 2).ToString("0.00");
                        EntryServiceTips.IsEnabled = false;
                        //decimal.TryParse(this.EntryServiceTips.Text, out Tips);
                    }
                }

            };
            PickerTips.SelectedIndex = 0;

            EntryServiceTips.TextChanged += (sender, e) =>
            {
                if (PickerTips.SelectedIndex >= 0)
                {
                    if (PickerTips.Items[PickerTips.SelectedIndex] == AppResources.Other)
                    {
                        EntryServiceTips.IsEnabled = true;
                        this.EntryServiceTips.NeedShowKeyboard = true;
                        Tips = 0;
                        decimal.TryParse(this.EntryServiceTips.Text, out Tips);
                    }
                    else
                    {
                        decimal percent = 0;
                        decimal.TryParse(PickerTips.Items[PickerTips.SelectedIndex].TrimEnd(new char[] { '%', ' ' }), out percent);
                        Tips = SubPrice * percent / 100;
                        EntryServiceTips.Text = Math.Round(Tips, 2).ToString("0.00");
                        EntryServiceTips.IsEnabled = false;
                    }
                }
                UpdatePrice();
            };

            ButtonCheckout.Text = AppResources.PlaceOrder;
            LabelTotal.Text = AppResources.Calculating;
        }

        public void UpdateCard()
        {
            if (CardSelected != null)
            {
                CachedImageCard.Source = CardSelected.creditTypeImg;
                var number = string.Empty;
                var index = CardSelected.CreditCardNumber.IndexOf("*", StringComparison.OrdinalIgnoreCase);
                if (index < CardSelected.CreditCardNumber.Length - 1)
                {
                    number = CardSelected.CreditCardNumber.Substring(index + 1);
                }
                LabelCardNumber.Text = number;
                LabelCardName.Text = CardSelected.AccountNickname;
                CachedImageCard.IsVisible = true;
                ButtonCheckout.IsEnabled = true;
            }
            else
            {
                LabelCardNumber.Text = string.Empty;
                LabelCardName.Text = string.Empty;
                CachedImageCard.IsVisible = false;
            }
        }

        public void LoadCard()
        {
            GridServiceFee.IsVisible = false;
            GridMileageFee.IsVisible = false;

            UserFacade userFacade = new UserFacade();
            userFacade.RequireLogin(this, () =>
            {
                this.IsBusy = true;
                Task.Run(() =>
                            {
                                Utils.IReloadPageCurrent = this;
                                try
                                {
                                    UserModel = Shared.APIs.IUsers.GetCurrentUser();
                                    //PaymentAccountDatas = Shared.APIs.IAccounts.GetPaymentMethods(UserModel.AccountID);
                                    PaymentAccountDatas = Shared.APIs.IAccounts.BtGetPaymentMethods(UserModel.AccountID);
                                    var zipCode = mServiceAddress.BasicAddress.ZipCode;
                                    if (Shared.IsLoggedIn)
                                    {
                                        Services = Shared.APIs.IServices.GetAvailableServicesForUser(Shared.UserId, null, null, zipCode);
                                    }
                                    else
                                    {
                                        Services = Shared.APIs.IServices.GetAvailableServices(zipCode);
                                    }
                                    if (Services != null)
                                    {
                                        var serviceKind = RestaurantDetailPage.IsDeliverySelected ? APIData.Data.ServiceKindCodes.Restaurant_Delivery : APIData.Data.ServiceKindCodes.Restaurant_Pickup;
                                        if (RestaurantDetailPage.GroupedDeliveryDestination != null)
                                        {
                                            if (RestaurantDetailPage.GroupedService != null)
                                            {
                                                Services = Services.Where(s => s.ServiceType == APIData.Data.ServiceTypes.Restaurant)
                                                                   .Where(s => s.ServiceKind == ServiceKindCodes.Restaurant_GroupedDelivery)
                                                                   .Where(s => s.DisplayName == RestaurantDetailPage.GroupedService.DisplayName)
                                                                     .ToList();
                                            }
                                            else
                                            {
                                                Services = Services.Where(s => s.ServiceType == APIData.Data.ServiceTypes.Restaurant)
                                                                           .Where(s => s.ServiceKind == ServiceKindCodes.Restaurant_GroupedDelivery)
                                                                           .ToList();
                                            }
                                        }
                                        else
                                        {
                                            Services = Services.Where(s => s.ServiceType == APIData.Data.ServiceTypes.Restaurant)
                                                                 .Where(s => s.ServiceKind == serviceKind)
                                                                 .ToList();
                                        }
                                        if (Services.FirstOrDefault() != null)
                                        {
                                            var service = Shared.APIs.IServices.GetService(Services.FirstOrDefault().ID);
                                            if (service != null)
                                            {
                                                Services.FirstOrDefault().AllowedTipRates = service.AllowedTipRates;
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
                                };
                            }).ContinueWith((t) =>
                            {
                                if (Services != null)
                                {
                                    if (RestaurantDetailPage.IsDeliverySelected)
                                    {
                                        var location = RestaurantDetailPage.RestaurantVM.Location;
                                        var address = Shared.LocalAddress;

                                        Decimal serviceFee = 0;
                                        Decimal mileageFee = 0;
                                        foreach (var s in Services)
                                        {
                                            mileageFee += Shared.APIs.IServices.GetExtendedRestaurantDeliveryFee(address.Latitude, address.Longitude, s.ID, RestaurantDetailPage.RestaurantVM.Location.ID, UserModel.ID, address.BasicAddress.ZipCode);
                                            foreach (var f in s.Fees)
                                            {
                                                if (f.Name == "delivery")
                                                    serviceFee += f.Amount;
                                            }
                                        }

                                        ServiceFee = serviceFee + mileageFee;
                                        LabelServiceFee.Text = "$" + Math.Round(serviceFee, 2).ToString("0.00");
                                        LabelMileageFee.Text = "$" + Math.Round(mileageFee, 2).ToString("0.00");
                                        LabelTotalServiceFee.Text = "$" + Math.Round(ServiceFee, 2).ToString("0.00");
                                        GridServiceFee.IsVisible = true;
                                        GridMileageFee.IsVisible = true;
                                    }
                                    else
                                    {
                                        GridServiceFee.IsVisible = false;
                                        GridMileageFee.IsVisible = false;
                                        ServiceFee = Services.SelectMany(s => s.Fees).Sum(s => s.Amount);
                                        LabelTotalServiceFee.Text = "$" + Math.Round(ServiceFee, 2).ToString("0.00");
                                    }                                  

                                   

                                    PickerTips.Items.Clear();
                                    var service = Services.FirstOrDefault();
                                    if (service != null
                                        && service.AllowedTipRates != null
                                        && service.AllowedTipRates.Count > 0)
                                    {
                                        PickerTips.IsVisible = true;
                                        var allowedTipRates = service.AllowedTipRates.OrderByDescending((arg) => arg).ToList();
                                        foreach (var tip in allowedTipRates)
                                        {
                                            if (tip == 0)
                                            {
                                                PickerTips.Items.Add(AppResources.Other);
                                            }
                                            else
                                            {
                                                PickerTips.Items.Add(Math.Round(tip * 100, 2).ToString("0.00") + "%");
                                            }
                                        }
                                        if (PickerTips.Items.Count > 0)
                                        {
                                            PickerTips.SelectedIndex = 0;
                                        }
                                    }
                                    else
                                    {
                                        PickerTips.IsVisible = false;
                                        EntryServiceTips.IsEnabled = false;
                                    }

                                    var preferredIndex = PaymentAccountDatas.FindIndex(s => s.IsPreferred);
                                    if (PaymentAccountDatas.Count > 0)
                                    {
                                        ButtonCheckout.IsEnabled = true;
                                        preferredIndex = preferredIndex > 0 ? preferredIndex : 0;
                                        CardSelected = PaymentAccountDatas[preferredIndex];
                                        UpdateCard();

                                    }
                                    else
                                    {
                                        UpdateCard();
                                    }

                                    UpdatePrice();
                                }

                                this.IsBusy = false;
                            }, TaskScheduler.FromCurrentSynchronizationContext());
            });
        }

        private void calculateTax()
        {
            var taxRate = RestaurantDetailPage.RestaurantVM.Location.TaxRate;
            SalesTax = Math.Round(taxRate * SubPrice, 2);
            LabelSalesTax.Text = "$" + Math.Round(SalesTax, 2).ToString("0.00");
            LabelSalesTaxName.Text = AppResources.SalesTax + " (" + Math.Round(taxRate * 100, 2).ToString("0.00") + "%)";
        }
        public void UpdatePrice()
        {
            TotalPrice = 0;

            TotalPrice += SubPrice;
            TotalPrice += SalesTax;
            TotalPrice += ServiceFee;
            TotalPrice += Tips;
            TotalPrice -= Discount;

            ButtonCheckout.Text = AppResources.PlaceOrder + " $" + Math.Round(TotalPrice, 2);
            ButtonCheckout.ForceLayout();

            LabelTotal.Text = "$" + Math.Round(TotalPrice, 2).ToString("0.00");
        }

        bool IsAppearing = false;
        bool timeSlotTrigger = false;
        bool timeSlotTriggerLocked = false;
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            IsAppearing = false;
            timeSlotTrigger = false;
        }

        public override void ReloadPage()
        {
            base.ReloadPage();
            LoadCard();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            IsAppearing = true;

            if (mFirstLoad)
            {
                mFirstLoad = false;
                LoadCard();
            }

            if (!this.IsBusy)
            {
                UpdatePrice();
            }

            if (!timeSlotTrigger)
            {
                timeSlotTrigger = true;
                Device.StartTimer(new TimeSpan(0, 0, 0, 2), () =>
                {
                    if (!IsPauseTrigger)
                    {
                        if (!timeSlotTriggerLocked && timeSlotTrigger)
                        {
                            try
                            {
                                timeSlotTriggerLocked = true;
                                if (!OrderClosed)
                                {
                                    SearchAvailableOrderTimes();
                                }
                                timeSlotTriggerLocked = false;
                            }
                            catch (Exception)
                            {
                                timeSlotTriggerLocked = false;
                                return timeSlotTrigger;
                            }
                        }
                    }
                    return timeSlotTrigger;
                });
            }
        }

        private bool OrderClosed = false;
        public void CloseOrder()
        {
            OrderClosed = true;
            Device.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    if (this.Navigation.NavigationStack.LastOrDefault() == this && IsAppearing)
                    {
                        await DisplayAlert(AppResources.MenuIsClosed, AppResources.OrderExpired, AppResources.OK);
                        var pages = Navigation.NavigationStack.Reverse().Skip(1).ToList();
                        foreach (var page in pages)
                        {
                            if (page != RestaurantDetailPage)
                            {
                                Navigation.RemovePage(page);
                            }
                            else
                            {
                                Navigation.RemovePage(page);
                                break;
                            }
                        }
                        await Navigation.PopAsync(true).ConfigureAwait(false);
                    }
                }
                catch (Exception) { }
            });
        }

        public void SearchAvailableOrderTimes()
        {
            if (!this.IsBusy && !this.IsErrorPage)
            {
                var currentDate = RestaurantDetailPage.GetDateTimeRestaurantFromUtc(DateTime.UtcNow);
                var selectedDate = RestaurantDetailPage.SelectedDate;
                if (currentDate.Date > RestaurantDetailPage.SelectedDate)
                {
                    CloseOrder();
                }
                else
                {
                    RMenuOrderingAvailableSlot menuOrderingAvailableSlot = RestaurantDetailPage.MenuOrderingAvailables
                            .Where(s => RestaurantDetailPage.GetDateTimeRestaurantFromLocal(s.CutoffTime.Time).Date == selectedDate.Date)
                            .Where(s => RestaurantDetailPage.GetDateTimeRestaurantFromLocal(s.CutoffTime.Time) > currentDate)
                            .FirstOrDefault();
                    if (menuOrderingAvailableSlot != null)
                    {
                        var minDate = RestaurantDetailPage.GetDateTimeRestaurantFromLocal(menuOrderingAvailableSlot.StartTime.Time);
                        if (minDate > RestaurantDetailPage.SelectedDate)
                        {
                            RestaurantDetailPage.SelectedDate = minDate;
                            if (CheckCurrentPage())
                            {
                                Utils.ShowSuccessMessage((RestaurantDetailPage.IsDeliverySelected ? AppResources.Delivery : AppResources.Pickup)
                                                         + " " + AppResources.TimeChanged + " " + RestaurantDetailPage.SelectedDate.ToString("h:mm tt") + ".", AppResources.Restaurant, 5);
                            }
                        }
                    }
                    else
                    {
                        CloseOrder();
                    }
                }
                var menuIds = RestaurantDetailPage.ListMenuItemViews.Values.SelectMany(t => t)
                                                   .SelectMany(t => t.ListMenuGroupModifierItemSelected)
                                                  .Select(s => s.MenuItemView.RestaurantMenuItem.MenuId).Distinct().ToList();
                foreach (var menuId in menuIds)
                {
                    var minutes = RestaurantDetailPage.SearchAvailableOrderTimes(menuId, false);
                    if (minutes <= 0)
                    {
                        CloseOrder();
                        break;
                    }
                    else if (minutes > 0 && minutes <= 5)
                    {
                        RestaurantDetailPage.SearchAvailableOrderTimes(menuId, true);
                        break;
                    }
                }
            }
        }
    }
}
