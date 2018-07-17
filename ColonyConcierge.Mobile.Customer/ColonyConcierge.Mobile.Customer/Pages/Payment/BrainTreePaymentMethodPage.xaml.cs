using ColonyConcierge.APIData.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using ColonyConcierge.Client;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Plugin.Toasts;
using ColonyConcierge.Mobile.Customer.Localization.Resx;
using Xamarin.Forms.Xaml;

namespace ColonyConcierge.Mobile.Customer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BrainTreePaymentMethodPage : ContentPageBase
	{
		//AddressFacade mAddressFacade = new AddressFacade();
		private Func<BrainTreePaymentMethodPage, int, bool> Saved;
		private IAppServices mAppServices;
		private string FormatDate = "MM/yyyy";

		public BrainTreePaymentMethodPage(bool isRequirePreferred, Func<BrainTreePaymentMethodPage, int, bool> saved = null)
		{
			InitializeComponent();

			NavigationPage.SetBackButtonTitle(this, AppResources.Back);

			Saved = saved;

			if (isRequirePreferred)
			{
				CheckBoxIsPreferred.IsVisible = false;
				CheckBoxIsPreferred.Checked = true;
			}
			else
			{
				CheckBoxIsPreferred.IsVisible = true;
			}

			mAppServices = DependencyService.Get<IAppServices>();

			this.BindingContext = new CreditCardData();

			EntryMMYY.TextChanged += (sender, e) =>
			{
				var length = EntryMMYY.Text.Length;
				if (length == 1)
				{
					try
					{
						int month = int.Parse(EntryMMYY.Text);
						if (month < 2)
						{
						}
						else if (month >= 2)
						{
							EntryMMYY.Text = "0" + month + "/";
						}
					}
					catch (Exception)
					{
						EntryMMYY.Text = e.OldTextValue;
					}
				}
				if (length == 2)
				{
					var oldLength = e.OldTextValue.Length;
					if (oldLength == 3)
					{
						EntryMMYY.Text = EntryMMYY.Text.Substring(0, 1);
					}
					else
					{
						try
						{
							int month = int.Parse(EntryMMYY.Text);
							if (month <= 12)
							{
								EntryMMYY.Text = EntryMMYY.Text + "/";
							}
							else
							{
								EntryMMYY.Text = e.OldTextValue;
							}
						}
						catch (Exception)
						{
							EntryMMYY.Text = e.OldTextValue;
						}
					}
				}
				if (length > FormatDate.Length)
				{
					EntryMMYY.Text = e.OldTextValue;
				}

				try
				{
					if (EntryMMYY.Text.Length != FormatDate.Length)
					{
						LabelMMYY.Text = AppResources.ExpirationDateNotCorrect;
					}
					else 
					{
						var date = DateTime.ParseExact(EntryMMYY.Text, FormatDate, null);
						if (date.Date < DateTime.Now.AddDays(1 - DateTime.Now.Day).Date)
						{
							LabelMMYY.Text = AppResources.ExpirationDateIsExpired;
						}
						else
						{
							LabelMMYY.Text = " ";
						}
					}
				}
				catch (Exception)
				{
					LabelMMYY.Text = AppResources.ExpirationDateNotCorrect;
				}
			};

			creditCardNumberEntry.TextChanged += (sender, e) =>
			{
				var oldTextValueLength = e.OldTextValue != null ? e.OldTextValue.Length : 0;
				if (e.NewTextValue != null  && e.NewTextValue.Length > oldTextValueLength)
				{
					string text = creditCardNumberEntry.Text.Replace(" ", "");
					string newNumber = string.Empty;
					for (int i = 0; i < text.Length / 4; i++)
					{
						newNumber += text.Substring(i * 4, 4) + " ";
					}
					var lastNumber = text.Length / 4 * 4;
					if (lastNumber >= 0 && text.Length > lastNumber)
					{
						newNumber += text.Substring(lastNumber);
					}
					creditCardNumberEntry.Text = newNumber;
				}

				var cardNumber = creditCardNumberEntry.Text.Replace(" ", "");
				var lengthFrom = 12;
				var lengthTo = -1;
				var cardType = mAppServices.GetCreditCardType(cardNumber);
				cvvValidator.MinLength = 3;
				EntryCvv.Placeholder = AppResources.CVV;
				switch (cardType)
				{
					case CreditCardType.Amex:
						lengthFrom = 15;
						lengthTo = 15;
						EntryCvv.Placeholder = AppResources.CID;
						cvvValidator.MinLength = 4;
						break;
					case CreditCardType.Discover:
						EntryCvv.Placeholder = AppResources.CID;
						lengthFrom = 16;
						lengthTo = 16;
						break;
					case CreditCardType.Mastercard:
						EntryCvv.Placeholder = AppResources.CVC2;
						lengthFrom = 16;
						lengthTo = 16;
						break;
					case CreditCardType.Jcb:
						EntryCvv.Placeholder = AppResources.CVV;
						lengthFrom = 16;
						lengthTo = 19;
						break;
					case CreditCardType.Maestro:
						EntryCvv.Placeholder = AppResources.CVV;
						lengthFrom = 12;
						lengthTo = 19;
						break;
					case CreditCardType.Visa:
						EntryCvv.Placeholder = AppResources.CVV;
						lengthFrom = 12;
						lengthTo = 19;
						break;
				}
				cvvValidator.FieldName = EntryCvv.Placeholder;

				if (cardNumber.Length >= lengthFrom && (lengthTo < 0 || cardNumber.Length <= lengthTo))
				{
					LabelCreditCardNumberValidator.Text = " ";
				}
				else
				{
					if (lengthTo > 0)
					{
						if (lengthTo == lengthFrom)
						{
							LabelCreditCardNumberValidator.Text = string.Format(AppResources.CreditCardNumberRequire, lengthFrom, lengthTo);
						}
						else
						{
							LabelCreditCardNumberValidator.Text = string.Format(AppResources.CreditCardNumberRequireMinMax, lengthFrom, lengthTo);
						}
					}
					else
					{
						LabelCreditCardNumberValidator.Text = string.Format(AppResources.CreditCardNumberRequireMin, lengthFrom);
					}
				}
			};

			List<Entry> entries = new List<Entry>()
			{
				EntryCvv, EntryPostalCode, creditCardNumberEntry, EntryMMYY, cardNickNameEntry
			};

			foreach (var entry in entries)
			{
				entry.TextChanged += (sender, e) =>
				{
					AddButton.IsEnabled = CheckValidator();
				};
			}


		}

		public bool CheckValidator()
		{
			var cardNumber = creditCardNumberEntry.Text == null ? string.Empty : creditCardNumberEntry.Text.Replace(" ", "");
			var lengthFrom = 12;
			var lengthTo = -1;
			var cardType = mAppServices.GetCreditCardType(cardNumber);
			cvvValidator.MinLength = 3;
			EntryCvv.Placeholder = AppResources.CVV;
			switch (cardType)
			{
				case CreditCardType.Amex:
					lengthFrom = 15;
					lengthTo = 15;
					EntryCvv.Placeholder = AppResources.CID;
					cvvValidator.MinLength = 4;
					break;
				case CreditCardType.Discover:
					EntryCvv.Placeholder = AppResources.CID;
					lengthFrom = 16;
					lengthTo = 16;
					break;
				case CreditCardType.Mastercard:
					EntryCvv.Placeholder = AppResources.CVC2;
					lengthFrom = 16;
					lengthTo = 16;
					break;
				case CreditCardType.Jcb:
					EntryCvv.Placeholder = AppResources.CVV;
					lengthFrom = 16;
					lengthTo = 19;
					break;
				case CreditCardType.Maestro:
					EntryCvv.Placeholder = AppResources.CVV;
					lengthFrom = 12;
					lengthTo = 19;
					break;
				case CreditCardType.Visa:
					EntryCvv.Placeholder = AppResources.CVV;
					lengthFrom = 12;
					lengthTo = 19;
					break;
			}
			cvvValidator.FieldName = EntryCvv.Placeholder;

			if (cardNumber.Length >= lengthFrom && (lengthTo < 0 || cardNumber.Length <= lengthTo))
			{
			}
			else
			{
				return false;
			}


			if (!cvvValidator.IsValid || !postalCodeValidator.IsValid || string.IsNullOrEmpty(creditCardNumberEntry.Text) ||
			    !cardNickNameValidator.IsValid)
			{
				return false;
			}
			try
			{
				if (EntryMMYY.Text.Length != FormatDate.Length)
				{
					return false;
				}
				var date = DateTime.ParseExact(EntryMMYY.Text, FormatDate, null);
				if (date.Date < DateTime.Now.AddDays(1 - DateTime.Now.Day).Date)
				{
					return false;
				}
			}
			catch (Exception)
			{
				return false;
			}


			return true;
		}

		async private void Button_Clicked(object sender, EventArgs e)
		{
			(sender as VisualElement).IsEnabled = false;
			this.IsBusy = true;
			var handle = false;
			var userModel = await Task.Run(() => { return Shared.APIs.IUsers.GetCurrentUser(); });

			var api = new APIs(new Connector());
			api.LoginToken = Shared.LoginToken;

			try
			{
				var token = Shared.APIs.IAccounts.GetPaymentGatewayToken();
				BrainTreeCard brainTreeCard = new BrainTreeCard();
				brainTreeCard.Token = token;
				brainTreeCard.CardNumber = creditCardNumberEntry.Text.Replace(" ", "");
				brainTreeCard.ExpirationDate = DateTime.ParseExact(EntryMMYY.Text, FormatDate, null);
				brainTreeCard.PostalCode = EntryPostalCode.Text;
				brainTreeCard.Cvv = EntryCvv.Text;

				mAppServices.GetNoncenBrainTree(brainTreeCard, async (obj) =>
				{
					if (obj is string && !string.IsNullOrEmpty(obj as string))
					{
						try
						{
							var paymentNonce = obj as string;
							CreditCardData paymentAccountData = this.BindingContext as CreditCardData;
							paymentAccountData.AccountNickname = cardNickNameEntry.Text;
							paymentAccountData.IsPreferred = CheckBoxIsPreferred.Checked;
							paymentAccountData.PaymentNonce = paymentNonce;
							var result = await Shared.APIs.IAccounts.BtAddPaymentMethod_Async(userModel.AccountID, paymentAccountData);
							if (result != 0)
							{
								if (Saved != null)
								{
									handle = Saved.Invoke(this, result);
								}

								if (!handle)
								{
									await Navigation.PopAsync().ConfigureAwait(false);
								}
							}
							System.Diagnostics.Debug.WriteLine("PaymentMethodID is : " + result);
						}
						catch (Exception ex)
						{
							var notificator = DependencyService.Get<IToastNotificator>();
							await notificator.Notify(ToastNotificationType.Error, AppResources.PaymentMethodTitle, ex.Message, TimeSpan.FromSeconds(5));
							System.Diagnostics.Debug.WriteLine("Exception occured When Add Payment Method : " + ex.Message);
						}
						finally
						{
							if (!handle)
							{
								(sender as VisualElement).IsEnabled = true;
								this.IsBusy = false;
							}
						}
					}
				});
			}
			catch (Exception ex)
			{
				var notificator = DependencyService.Get<IToastNotificator>();
				await notificator.Notify(ToastNotificationType.Error, AppResources.PaymentMethodTitle, ex.Message, TimeSpan.FromSeconds(5));
				System.Diagnostics.Debug.WriteLine("Exception occured When Add Payment Method : " + ex.Message);
				if (!handle)
				{
					(sender as VisualElement).IsEnabled = true;
					this.IsBusy = false;
				}
			}
		}
	}
}
