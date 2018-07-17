using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ColonyConcierge.APIData.Data;
using ColonyConcierge.Mobile.Customer.Localization.Resx;
using Plugin.Toasts;
using TK.CustomMap.Api.Google;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ColonyConcierge.Mobile.Customer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddAddressPage : ContentPageBase
	{
		AddressFacade mAddressFacade = new AddressFacade();
		Dictionary<string, string> mStates = new Dictionary<string, string>();
		private Page ParentPage;
		private Action<ExtendedAddress> Saved;
		private IAppServices mAppServices;

		public bool IsBusiness
		{
			get;
			set;
		}
		public string FullName { get; set; }
		public string EmailAddress { get; set; }

		public AddAddressPage(Page parentPage, Action<ExtendedAddress> saved = null)
		{
			InitializeComponent();

			mAppServices = DependencyService.Get<IAppServices>();
			ParentPage = parentPage;
			Saved = saved;

			mStates = mAddressFacade.GetStates();
			stateEntry.Items.Clear();
			foreach (var item in mStates.Keys)
			{
				stateEntry.Items.Add(item);
			}
		}

		async void ButtonNext_Clicked(object sender, System.EventArgs e)
		{
			(sender as VisualElement).IsEnabled = false;
			this.IsBusy = true;
			try
			{
				var state = string.Empty;
				if (stateEntry.SelectedIndex >= 0)
				{
					state = stateEntry.Items[stateEntry.SelectedIndex];
					state = mStates[state];
				}

				string Address = zipcodeEntry.Text + " US";

				GmsDetailsResultItem gmsDetailsResult = await Task.Run(() => { return mAddressFacade.GetGmsDetails(Address); });
				if (gmsDetailsResult != null)
				{	
					ExtendedAddress serviceAddress = new ExtendedAddress() { BasicAddress = new Address() };
					serviceAddress.BasicAddress.ZipCode = zipcodeEntry.Text;
					serviceAddress.BasicAddress.City = cityEntry.Text;
					serviceAddress.BasicAddress.StateProv = state;
					serviceAddress.BasicAddress.Line1 = addressEntry.Text;
					serviceAddress.BasicAddress.Line2 = apartmentEntry.Text;
					serviceAddress.Latitude = (decimal)gmsDetailsResult.Geometry.Location.Latitude;
					serviceAddress.Longitude = (decimal)gmsDetailsResult.Geometry.Location.Longitude;

					SerivesFacade serivesService = new SerivesFacade();
					var zipCode = await Task.Run(() => serivesService.CheckAvailableServices(serviceAddress));
					if (!string.IsNullOrEmpty(zipCode))
					{
						var latitude = mAppServices.LastLatitude;
						var longitude = mAppServices.LastLongitude;
						if (!latitude.Equals(0) || !longitude.Equals(0))
						{
							var currentAddressDetails = await mAddressFacade.GetGmsDetails(latitude, longitude);
							ExtendedAddress currentExtendedAddress = mAddressFacade.GetExtendedAddress(currentAddressDetails);
							if (currentAddressDetails != null && currentExtendedAddress.BasicAddress.ZipCode == zipCode)
							{
								serviceAddress.Latitude = (decimal)latitude;
								serviceAddress.Longitude = (decimal)longitude;
							}
							SaveAddressPage SaveAddressPage = new SaveAddressPage(ParentPage, serviceAddress, Saved);
							SaveAddressPage.IsBusiness = IsBusiness;
							SaveAddressPage.FullName = FullName;
							SaveAddressPage.EmailAddress = EmailAddress;

							await Utils.PushAsync(Navigation, SaveAddressPage, true);
						}
						else
						{
							mAppServices.CheckLocationPermission((isChecked) =>
							{
								Task.Run(() =>
								{
									if (isChecked)
									{
										IsBusy = true;
										Device.BeginInvokeOnMainThread(() =>
										{
											try
											{
												mAppServices.GetLocation((isLocation) =>
												{
													Task.Run(() =>
													{
														try
														{
															if (isLocation)
															{
																latitude = mAppServices.LastLatitude;
																longitude = mAppServices.LastLongitude;
																if (!latitude.Equals(0) || !longitude.Equals(0))
																{
																	var currentAddressDetails = mAddressFacade.GetGmsDetails(latitude, longitude).Result;
																	ExtendedAddress currentExtendedAddress = mAddressFacade.GetExtendedAddress(currentAddressDetails);
																	if (currentAddressDetails != null && currentExtendedAddress.BasicAddress.ZipCode == zipCode)
																	{
																		serviceAddress.Latitude = (decimal)latitude;
																		serviceAddress.Longitude = (decimal)longitude;
																	}
																}
															}
															else
															{
																Device.BeginInvokeOnMainThread(() =>
																{
																	IsBusy = false;
																	SaveAddressPage SaveAddressPage = new SaveAddressPage(ParentPage, serviceAddress, Saved);
																	SaveAddressPage.IsBusiness = IsBusiness;
																	SaveAddressPage.FullName = FullName;
																	SaveAddressPage.EmailAddress = EmailAddress;
																	Utils.PushAsync(Navigation, SaveAddressPage, true);
																});
															}
														}
														catch (Exception) 
														{
															Device.BeginInvokeOnMainThread(() =>
															{
																IsBusy = false;
																SaveAddressPage SaveAddressPage = new SaveAddressPage(ParentPage, serviceAddress, Saved);
																SaveAddressPage.IsBusiness = IsBusiness;
																SaveAddressPage.FullName = FullName;
																SaveAddressPage.EmailAddress = EmailAddress;
																Utils.PushAsync(Navigation, SaveAddressPage, true);
															});
														}
														new System.Threading.ManualResetEvent(false).WaitOne(100);
													}).ContinueWith(t =>
													{
														Device.BeginInvokeOnMainThread(() =>
														{
															IsBusy = false;
															var saveAddressPage = new SaveAddressPage(ParentPage, serviceAddress, Saved);
															saveAddressPage.FullName = FullName;
															saveAddressPage.EmailAddress = EmailAddress;
															Utils.PushAsync(Navigation, saveAddressPage, true);
														});
													}, TaskScheduler.FromCurrentSynchronizationContext());
												});
											}
											catch (Exception)
											{
												IsBusy = false;
												SaveAddressPage SaveAddressPage = new SaveAddressPage(ParentPage, serviceAddress, Saved);
												SaveAddressPage.IsBusiness = IsBusiness;
												SaveAddressPage.FullName = FullName;
												SaveAddressPage.EmailAddress = EmailAddress;
												Utils.PushAsync(Navigation, SaveAddressPage, true);
											}
										});
									}
									else
									{
										new System.Threading.ManualResetEvent(false).WaitOne(100);
										Device.BeginInvokeOnMainThread(() =>
										{
											IsBusy = false;
											SaveAddressPage SaveAddressPage = new SaveAddressPage(ParentPage, serviceAddress, Saved);
											SaveAddressPage.IsBusiness = IsBusiness;
											SaveAddressPage.FullName = FullName;
											SaveAddressPage.EmailAddress = EmailAddress;
											Utils.PushAsync(Navigation, SaveAddressPage, true);
										});
									}
								});
							});
						}
					}
					else
					{
						mAppServices.TrackEvent("LP Address - outside service area", "click", "address");
						ExtendedAddress newServiceAddress = new ExtendedAddress() { BasicAddress = new Address() };
						newServiceAddress.BasicAddress.ZipCode = zipcodeEntry.Text;
						newServiceAddress.BasicAddress.City = cityEntry.Text;
						newServiceAddress.BasicAddress.StateProv = state;
						newServiceAddress.BasicAddress.Line1 = addressEntry.Text;
						newServiceAddress.BasicAddress.Line2 = apartmentEntry.Text;
						newServiceAddress.Latitude = (decimal)gmsDetailsResult.Geometry.Location.Latitude;
						newServiceAddress.Longitude = (decimal)gmsDetailsResult.Geometry.Location.Longitude;
						var serviceNotAvailablePage = new ServiceNotAvailablePage(newServiceAddress, FullName, EmailAddress);
						await Navigation.PushAsync(serviceNotAvailablePage);
						//var notificator = DependencyService.Get<IToastNotificator>();
						//await notificator.Notify(ToastNotificationType.Error, "", AppResources.AreaInvalid, TimeSpan.FromSeconds(2));
					}
				}
				else
				{
					var notificator = DependencyService.Get<IToastNotificator>();
					await notificator.Notify(ToastNotificationType.Error, "", AppResources.PullDataFailMessage, TimeSpan.FromSeconds(2));
				}
			}
			catch (Exception ex)
			{
				var notificator = DependencyService.Get<IToastNotificator>();
				await notificator.Notify(ToastNotificationType.Error, "Error", ex.Message, TimeSpan.FromSeconds(2));
			}
			finally
			{
				(sender as VisualElement).IsEnabled = true;
				this.IsBusy = false;
			}
			//throw new NotImplementedException();
		}

		void ButtonCancel_Clicked(object sender, System.EventArgs e)
		{
			Navigation.PopAsync().ConfigureAwait(false);
			//throw new NotImplementedException();
		}
	}
}
