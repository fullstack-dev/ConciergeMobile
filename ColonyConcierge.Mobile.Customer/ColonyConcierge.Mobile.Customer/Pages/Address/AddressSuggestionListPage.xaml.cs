using ColonyConcierge.APIData.Data;
using ColonyConcierge.Client;
using ColonyConcierge.Mobile.Customer.Localization.Resx;
using PCLAppConfig;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TK.CustomMap.Api;
using TK.CustomMap.Api.Google;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Plugin.Toasts;
using Xamarin.Forms.Xaml;

namespace ColonyConcierge.Mobile.Customer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddressSuggestionListPage : ContentPageBase
	{
		private bool _textChangeItemSelected;
		private IAppServices mAppServices = null;
		private string ZipCodeSelecting = string.Empty;
		private AddressFacade mAddressFacade = new AddressFacade();
		private bool mIsUseLocation = false;
		private bool mFirstLoad = true;
		private Page ParentPage;
		private Action<ExtendedAddress> Saved;

		public string FullName { get; set; }
		public string EmailAddress { get; set; }

		private ExtendedAddress mServiceAddress;
		public ExtendedAddress ServiceAddress
		{
			get
			{
				return mServiceAddress;
			}
		}

		public bool IsBusiness
		{
			get;
			set;
		}

		ToolbarItem QRCodeToolbarItem = null;

		public bool mIsShowQRCode;
		public bool IsShowQRCode
		{
			get
			{
				return mIsShowQRCode;
			}
			set
			{
				mIsShowQRCode = value;
				//if (IsShowQRCode)
				//{
				//	this.ToolbarItems.Add(QRCodeToolbarItem);
				//}
				//else
				//{
				//	this.ToolbarItems.Remove(QRCodeToolbarItem);
				//}
			}
		}

		public bool IsShowCurrentLocation
		{
			get
			{
				return ButtonUseCurrentLocation.IsVisible;
			}
			set
			{
				ButtonUseCurrentLocation.IsVisible = value;
			}
		}

		private bool mIsWaitLocation;
		public bool IsWaitLocation
		{
			get
			{
				return mIsWaitLocation;
			}
			set
			{
				OnPropertyChanging(nameof(IsWaitLocation));
				mIsWaitLocation = value;
				OnPropertyChanged(nameof(IsWaitLocation));
			}
		}

		public string ZipValid
		{
			get
			{
				if (!mAddressFacade.ValidateAddress(ServiceAddress))
				{
					// Invalid or incomplete address
					return mIsUseLocation ? AppResources.MyLocationInvaild : AppResources.InvalidAddress;
				}

				return string.IsNullOrEmpty(selectedZip) ?
							 string.Format(AppResources.ZipInvalid, ZipCodeSelecting)
							 : AppResources.ZipValid;
			}
		}

		public bool IsValid
		{
			get
			{
				if (!mAddressFacade.ValidateAddress(ServiceAddress))
				{
					// Invalid or incomplete address
					return false;
				}

				return !string.IsNullOrEmpty(selectedZip);
			}
		}

		private bool mMessageVisible;
		public bool MessageVisible
		{
			get
			{
				return mMessageVisible;
			}
			set
			{
				mMessageVisible = value;
				NotifyPropertyChanged(nameof(MessageVisible));
			}
		}

		private string selectedZip;
		public string SelectedZipCode
		{
			get
			{
				return selectedZip;
			}
			set
			{
				selectedZip = value;
				NotifyPropertyChanged(nameof(SelectedZipCode));
				NotifyPropertyChanged(nameof(ZipValid));
				NotifyPropertyChanged(nameof(IsValid));
			}
		}

		private Prediction selectedPlace;
		public Prediction SelectedPlace
		{
			get { return selectedPlace; }
			set
			{
				selectedPlace = value;
			}
		}

		private string searchText;
		public string SearchText
		{
			get 
			{
				return searchText; 
			}
			set
			{
				NotifyPropertyChanging(nameof(SearchText));
				searchText = value;
				NotifyPropertyChanged(nameof(SearchText));
			}
		}

		private ObservableCollection<Prediction> places;
		public ObservableCollection<Prediction> Places
		{
			get { return places; }
			set
			{
				NotifyPropertyChanging(nameof(Places));
				places = value;
				NotifyPropertyChanged(nameof(Places));
			}
		}

		public AddressSuggestionListPage(Page parentPage, Action<ExtendedAddress> saved = null, bool isBusiness = false)
		{
			InitializeComponent();

			ParentPage = parentPage;
			Saved = saved;
			IsBusiness = isBusiness;
			mServiceAddress = new ExtendedAddress() { BasicAddress = new Address() };
			mAppServices = DependencyService.Get<IAppServices>();

			// Initialize places
			GmsPlace.Init(ConfigurationManager.AppSettings["PlacesApiKey"]);
			this._textChangeItemSelected = false;

			QRCodeToolbarItem = new ToolbarItem()
			{
				Icon = "qrcode.png",
				Text = AppResources.QRCode,
				Command = new Command(() =>
				{
					bool lockedQR = false;
					mAppServices.ScanQRCodeContinuously((qrcode, result) =>
					{
						if (!lockedQR)
						{
							var url = result.Text;
							try
							{
								if (url.ToLower().Trim().StartsWith("asystyou://restaurant", StringComparison.OrdinalIgnoreCase))
								{
									lockedQR = true;
									Device.BeginInvokeOnMainThread(async () =>
									{
										try
										{
											qrcode.PauseAnalysis();
											Device.OpenUri(new Uri(url));
											if (Device.Android == Device.RuntimePlatform)
											{
												await Task.Delay(500);
											}
											mAppServices.Vibration(1000);
											await Task.Delay(1000);
											qrcode.Cancel();
										}
										catch (Exception)
										{
											lockedQR = false;
										}
										lockedQR = false;
									});
									return false;
								}
								else
								{
									return false;
								}
							}
							catch (Exception)
							{
								return false;
							}
						}
						return false;
					});
				})
			};

			double lastWidth = 0;
			double lastHeight = 0;
			this.SizeChanged += (sender, e) =>
			{
				if (this.Width > 1 && this.Height > 1)
				{
					if (this.Width >= 400)
					{
						LabelUseCurrentLocation.FontSize = Device.GetNamedSize(NamedSize.Medium, LabelUseCurrentLocation) * 1.0;
					}
					else if (this.Width >= 340)
					{
						LabelUseCurrentLocation.FontSize = Device.GetNamedSize(NamedSize.Medium, LabelUseCurrentLocation) * 0.9;
					}
					else
					{
						LabelUseCurrentLocation.FontSize = Device.GetNamedSize(NamedSize.Medium, LabelUseCurrentLocation) * 0.81;
					}

					if (lastWidth < this.Width || lastHeight < this.Height)
					{
						lastWidth = this.Width;
						lastHeight = this.Height;

						ImageBackgroundFront.WidthRequest = this.Width;
						ImageBackgroundFront.HeightRequest = lastHeight;

						ImageBackgroundBehind.WidthRequest = this.Width;
						ImageBackgroundBehind.HeightRequest = lastHeight;
					}
				}
			};

			ImageBackgroundBehind.SizeChanged += (sender, e) =>
			{
				if (this.ImageBackgroundBehind.Width > 1 && this.ImageBackgroundBehind.Height > 1)
				{
					ImageBackgroundFront.WidthRequest = this.ImageBackgroundBehind.Width;
					ImageBackgroundFront.HeightRequest = this.ImageBackgroundBehind.Height;
				}
			};

			var previouslyUsedAddresses = Shared.PreviouslyUsedAddresses.Take(2).ToList();
			GridPreviouslyUsedAddresses.IsVisible = previouslyUsedAddresses.Count > 0;

			CheckBoxPreviouslyUsedAddressesText1.IsVisible = previouslyUsedAddresses.Count > 0;
			CheckBoxPreviouslyUsedAddressesText2.IsVisible = previouslyUsedAddresses.Count > 1;
			GridCheckBoxPreviouslyUsedAddressesText2.IsVisible = CheckBoxPreviouslyUsedAddressesText2.IsVisible;

			var addresses1 = previouslyUsedAddresses.Count > 0 ? previouslyUsedAddresses[0].BasicAddress.ToAddressLine() : string.Empty;
			var addresses2 = previouslyUsedAddresses.Count > 1 ? previouslyUsedAddresses[1].BasicAddress.ToAddressLine() : string.Empty;

			CheckBoxPreviouslyUsedAddressesText1.Text = addresses1;
			CheckBoxPreviouslyUsedAddressesText2.Text = addresses2;
			if (Shared.LocalAddress != null && Shared.LocalAddress.BasicAddress != null)
			{
				ImageCheckBoxPreviouslyUsedAddressesText1.IsVisible = addresses1 == Shared.LocalAddress.BasicAddress.ToAddressLine();
			}

			//if (Shared.LocalAddress != null)
			//{
			//	CheckBoxPreviouslyUsedAddresses1.Checked = Shared.LocalAddress.BasicAddress.ToAddressLine() == addresses1;
			//	CheckBoxPreviouslyUsedAddresses2.Checked = Shared.LocalAddress.BasicAddress.ToAddressLine() == addresses2;
			//}
			scrollView.ForceLayout();

			ErrorViewAddress.TryAgain += (sender, e) =>
			{
				this.IsBusy = true;
				this.GridAddressError.IsVisible = false;
				addressEntry_TextChanged(addressEntry, null);
			};

			//ErrorViewSelectAddress.TryAgain += (sender, e) =>
			//{
   //             this.GridSelectdAddressError.IsVisible = false;
			//	if (this.SelectedPlace != null)
			//	{
			//		HandleItemSelected(this.SelectedPlace);
			//	}
			//};

			GridCheckBoxPreviouslyUsedAddressesText1.GestureRecognizers.Add(new TapGestureRecognizer()
			{
				Command = new Command(() => 
				{
					//CheckBoxPreviouslyUsedAddresses1.Checked = !CheckBoxPreviouslyUsedAddresses1.Checked;
					addressEntry.Unfocus();
					Save(previouslyUsedAddresses[0]);
				})
			});
			//CheckBoxPreviouslyUsedAddresses1.CheckedChanged += (sender, e) =>
			//{
			//	if (e.Value)
			//	{
			//		Save(previouslyUsedAddresses[0]);
			//	}
			//};

			CheckBoxPreviouslyUsedAddressesText2.GestureRecognizers.Add(new TapGestureRecognizer()
			{
				Command = new Command(() =>
				{
					//CheckBoxPreviouslyUsedAddresses2.Checked = !CheckBoxPreviouslyUsedAddresses2.Checked;
					addressEntry.Unfocus();
					Save(previouslyUsedAddresses[1]);

				})
			});
			//CheckBoxPreviouslyUsedAddresses2.CheckedChanged += (sender, e) =>
			//{
			//	if (e.Value)
			//	{
			//		Save(previouslyUsedAddresses[1]);
			//	}
			//};

			ButtonUseCurrentLocation.GestureRecognizers.Add(new TapGestureRecognizer
			{
				Command = new Command(() =>
				{
					try
					{
						this.IsWaitLocation = true;
						mAppServices.CheckLocationPermission(t =>
						{
							if (t)
							{
								mAppServices.GetLocation((isLocation) =>
								{
									if (isLocation)
									{
										Task.Run(async () =>
										{
											Device.BeginInvokeOnMainThread(() =>
											{
												this.IsBusy = true;
											});
											try
											{
												var gmsDetails = await mAddressFacade.GetGmsDetails(mAppServices.LastLatitude, mAppServices.LastLongitude);
												if (gmsDetails != null)
												{
													ExtendedAddress extendedAddress = new ExtendedAddress 
													{
														BasicAddress = new Address()
													};
													mAddressFacade.FillExtendedAddress(extendedAddress, gmsDetails);
													this.SearchText = extendedAddress.BasicAddress.ToAddressWithoutZip().Replace("\n", " ");
													Device.BeginInvokeOnMainThread(() =>
													{
														this.ButtonUseCurrentLocation.IsVisible = false;
													});
													//mIsUseLocation = true;
													//await UpdateLocation(gmsDetails);
												}
											}
											catch (Exception) { }
											Device.BeginInvokeOnMainThread(() =>
											{
												this.IsWaitLocation = false;
												this.IsBusy = false;
												mAppServices.SetNetworkBar(false);
											});
										});
									}
									else
									{
										this.IsWaitLocation = false;
									}
								});
							}
							else
							{
								this.IsWaitLocation = false;
							}
						});
					}
					catch (Exception)
					{
						Device.BeginInvokeOnMainThread(async () =>
						{
							await Utils.ShowErrorMessage(AppResources.SomethingWentWrong, 2);
						});
					}
				}),
			});

			ImageClose.GestureRecognizers.Add(new TapGestureRecognizer()
			{
				Command = new Command((t) =>
				{
					this.SearchText = string.Empty;
				})
			});

			AddressMessageNoMatches.GestureRecognizers.Add(new TapGestureRecognizer()
			{
				Command = new Command((sender) =>
				{
					AddAddressPage AddAddressPage = new AddAddressPage(ParentPage, Saved);
					AddAddressPage.IsBusiness = IsBusiness;
					AddAddressPage.FullName = FullName;
					AddAddressPage.EmailAddress = EmailAddress;
					Utils.PushAsync(Navigation, AddAddressPage, true);
				})
			});

			this.BindingContextChanged += AddressSuggestionListPage_BindingContextChanged;
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			if (mFirstLoad && CheckCurrentPage())
			{
				mFirstLoad = false;
				addressEntry.NeedShowKeyboard = true;
			}
		}

		public override void ShowLoadErrorPage()
		{
			//base.ShowLoadErrorPage();
			this.GridAddressError.IsVisible = true;
			this.MessageVisible = false;
		}

		private void AddressSuggestionListPage_BindingContextChanged(object sender, EventArgs e)
		{
			RegistrationEntry userInfo = (RegistrationEntry)this.BindingContext;
			this._textChangeItemSelected = true;
			this.SearchText = userInfo.ServiceAddress.BasicAddress.ToCannonicalString();
			this._textChangeItemSelected = false;
		}

		private bool lockedSearchPlaces = false;
		private async void addressEntry_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (this._textChangeItemSelected)
			{
				this._textChangeItemSelected = false;
				return;
			}

			if (!lockedSearchPlaces)
			{
				lockedSearchPlaces = true;
				try
				{
					var searchingText = this.SearchText;
					if (string.IsNullOrEmpty(searchingText))
					{
						var previouslyUsedAddresses = Shared.PreviouslyUsedAddresses.Take(2).ToList();
						GridPreviouslyUsedAddresses.IsVisible = previouslyUsedAddresses.Count > 0;
						ButtonUseCurrentLocation.IsVisible = true;
						this.MessageVisible = false;
						this.AddressMessageNoMatches.IsVisible = false;
						this.Places = new ObservableCollection<Prediction>();
                        this.GridAddressSearchResults.IsVisible = false;
						this.GridAddressError.IsVisible = false;
					}
					else
					{
						GridPreviouslyUsedAddresses.IsVisible = false;
						ButtonUseCurrentLocation.IsVisible = false;
						var searchPlaces = await mAddressFacade.SearchPlaces(searchingText);
						if (searchPlaces == null)
						{
							this.GridAddressError.IsVisible = true;
							MessageVisible = false;
							this.AddressMessageNoMatches.IsVisible = false;
							this.GridAddressSearchResults.IsVisible = false;
							this.Places = new ObservableCollection<Prediction>();
						}
						else if (searchPlaces.Count == 0)
						{
							this.Places = searchPlaces;
							this.GridAddressError.IsVisible = false;
							this.AddressMessageNoMatches.IsVisible = true;
                            this.GridAddressSearchResults.IsVisible = false;
						}
						else
						{
                            this.Places = searchPlaces;
							this.GridAddressError.IsVisible = false;
							this.AddressMessageNoMatches.IsVisible = false;
                            this.GridAddressSearchResults.IsVisible = true;
						}
					}
					lockedSearchPlaces = false;
					this.IsBusy = false;
					if (searchingText != this.SearchText)
					{
						addressEntry_TextChanged(addressEntry, null);
					}
				}
				catch (Exception)
				{
                    this.IsBusy = false;
					lockedSearchPlaces = false;
				}
			}
		}

		private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			if (e.SelectedItem == null) return;
			var prediction = (Prediction)e.SelectedItem;

			this.HandleItemSelected(prediction);
		}

		private async void FindPostalCode()
		{
			if (selectedPlace.Description != AppResources.UseCurrentLocation)
			{
				this.IsBusy = true;
				Utils.IReloadPageCurrent = this;
				try
				{
					var details = await mAddressFacade.GetGmsDetails(selectedPlace.Description);
					int line1 = 0;
					if (!int.TryParse(mAddressFacade.GetStreetAddress(details).Trim(), out line1))
					{
						var placeDetails = await GmsPlace.Instance.GetDetails(selectedPlace.PlaceId);
						if (placeDetails != null && placeDetails.Item != null)
						{
							if (int.TryParse(mAddressFacade.GetStreetAddress(placeDetails.Item).Trim(), out line1))
							{
								details = placeDetails.Item;
							}
						}
					}

					this.SelectedZipCode = string.Empty;
					// Check if the place has a postal code
					//if (details.Status == GmsDetailsResultStatus.Ok)
					{
						mIsUseLocation = false;
						await UpdateLocation(details);
					}
				}
				catch(Exception) 
				{
					this.GridAddressError.IsVisible = true;
					MessageVisible = false;
				}
				if (Utils.IReloadPageCurrent == this)
				{
					Utils.IReloadPageCurrent = null;
				}
				this.IsBusy = false;
			}
		}

		public async Task UpdateLocation(GmsDetailsResultItem msDetailsResult)
		{
			Device.BeginInvokeOnMainThread(() =>
			{
				this.MessageVisible = false;
			});
			var messageVisible = true;
			try
			{
				// Check to see if service is provided in this area
				mAddressFacade.FillExtendedAddress(ServiceAddress, msDetailsResult);
				ZipCodeSelecting = ServiceAddress.BasicAddress.ZipCode;
				List<Service> serviceList = null;
				var serivesService = new SerivesFacade();
				var zipCode = await Task.Run(() =>
				{
					serviceList = serivesService.GetAvailableServices(ServiceAddress);
					return serivesService.CheckAvailableServices(ServiceAddress, serviceList);
				});

				if (serviceList != null)
				{
					if(mAddressFacade.ValidateAddress(ServiceAddress))
					{
						if (string.IsNullOrEmpty(zipCode))
						{
							messageVisible = false;
							mAppServices.TrackEvent("LP Address - outside service area", "click", "address");
							var serviceAddress = new ExtendedAddress { BasicAddress = new Address() };
							mAddressFacade.FillExtendedAddress(serviceAddress, msDetailsResult);
							var serviceNotAvailablePage = new ServiceNotAvailablePage(serviceAddress, FullName, EmailAddress);
							await Navigation.PushAsync(serviceNotAvailablePage);
                            this.SearchText = string.Empty;
							//if (Shared.IsLoggedIn)
							//{

							//	var userModel = await Shared.APIs.IUsers.GetCurrentUser_Async();
							//	mAppServices.AddServiceNotAvaible(userModel.ID.ToString(), userModel.FirstName + " " + userModel.LastName, userModel.EmailAddress, serviceAddress);
							//}
						}
						else
						{
							mAppServices.TrackEvent("LP Address", "click", "address");
						}
					}
				}
				else
				{
                    this.GridAddressError.IsVisible = true;
					this.MessageVisible = false;
					return;
				}

				this.SelectedZipCode = zipCode;
			}
			catch (Exception) { }
			finally
			{
				Device.BeginInvokeOnMainThread(() =>
				{
					this.MessageVisible = messageVisible && !this.GridAddressError.IsVisible;
					if (IsValid)
					{
						Save(ServiceAddress);
					}
				});
			}
		}

		private void Save(ExtendedAddress serviceAddress)
		{
			if (!IsBusiness)
			{
				Shared.LocalAddress = serviceAddress;
			}

			if (Saved != null)
			{
				Saved.Invoke(serviceAddress);
			}

			this._textChangeItemSelected = true;
			this.SearchText = serviceAddress.BasicAddress.ToCannonicalString();
			var pages = Navigation.NavigationStack.Reverse().Skip(1).ToList();
			if (ParentPage != null)
			{
				foreach (var page in pages)
				{
					if (page != ParentPage)
					{
						Navigation.RemovePage(page);
					}
					else
					{
						break;
					}
				}
			}
			if (pages.Count > 0)
			{
				Navigation.PopAsync(true).ConfigureAwait(false);
			}
		}

		private void NotifyPropertyChanging(string propertyName)
		{
			OnPropertyChanging(propertyName);
		}

		private void NotifyPropertyChanged(string propertyName)
		{
			OnPropertyChanged(propertyName);
		}

		private void HandleItemSelected(Prediction prediction)
		{
			this._textChangeItemSelected = true;
			GridAddressError.IsVisible = false;
			MessageVisible = false;
			this.SearchText = prediction.Description;
			this.SelectedZipCode = string.Empty;
			this.SelectedPlace = prediction;
			this.FindPostalCode();

			this.Reset();
		}

		private void Reset()
		{
			this.addressEntry.Unfocus();
			this.GridAddressSearchResults.IsVisible = false;
		}

	}
}
