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
    public partial class AccountBrainTreePaymentMethodPage : ContentPageBase
	{
		public bool IsBackAnimation
		{
			get;
			set;
		} = false;

		//AddressFacade mAddressFacade = new AddressFacade();
		private IAppServices mAppServices;
		private string FormatDate = "MM/yyyy";
		public CreditCardData CreditCardData = new CreditCardData();

		public AccountBrainTreePaymentMethodPage(bool isRequirePreferred)
		{
			InitializeComponent();

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
							LabelCreditCardNumberValidator.Text = string.Format("Credit card number should be {0} numbers", lengthFrom, lengthTo);
						}
						else
						{
							LabelCreditCardNumberValidator.Text = string.Format("Credit card number should be from {0} to {1} numbers", lengthFrom, lengthTo);
						}
					}
					else
					{
						LabelCreditCardNumberValidator.Text = string.Format("Credit card number should be at least {0} numbers", lengthFrom);
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

		protected override void OnAppearing()
		{
			base.OnAppearing();

			//if (this.Width > 0 && this.Height > 0)
			//{
			//	if (IsBackAnimation)
			//	{
			//		IsBackAnimation = false;
			//		this.TranslationX = -this.Width;
			//		Animation animation = new Animation((s) =>
			//		{
			//			this.TranslationX = -this.Width * s;
			//		}, 1, 0);
			//		animation.Commit(this, "OnAppearing", 16, 500, Easing.Linear);
			//	}
			//	else
			//	{
			//		if (mFirstLoad)
			//		{
			//			mFirstLoad = false;
			//			this.TranslationX = this.Width;
			//			Animation animation = new Animation((s) =>
			//			{
			//				this.TranslationX = this.Width * s;
			//			}, 1, 0);
			//			animation.Commit(this, "OnDisappearing", 16, 500, Easing.Linear);
			//		}

			//	}
			//}
		}

		public bool CheckValidator()
		{
			var cardNumber = creditCardNumberEntry.Text == null? string.Empty : creditCardNumberEntry.Text.Replace(" ", "");
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
			//var userModel = await Task.Run(() => { return Shared.APIs.IUsers.GetCurrentUser(); });

			//var api = new APIs(new Connector());
			//api.LoginToken = Shared.LoginToken;

			try
			{
				//var token = Shared.APIs.IAccounts.GetToken(userModel.AccountID);
				BrainTreeCard brainTreeCard = new BrainTreeCard();
				//brainTreeCard.Token = token;
				brainTreeCard.CardNumber = creditCardNumberEntry.Text.Replace(" ", "");
				brainTreeCard.ExpirationDate = DateTime.ParseExact(EntryMMYY.Text, FormatDate, null);
				brainTreeCard.PostalCode = EntryPostalCode.Text;
				brainTreeCard.Cvv = EntryCvv.Text;
				brainTreeCard.IsPreferred = CheckBoxIsPreferred.Checked;
				brainTreeCard.AccountNickname = cardNickNameEntry.Text;
				await Utils.PushAsync(Navigation, new SecurityQuestionPage()
				{
					BindingContext = this.BindingContext,
					BrainTreeCard = brainTreeCard,
				}, true);

				//mAppServices.GetNoncenBrainTree(brainTreeCard, async (obj) =>
				//{
				//	if (obj is string && !string.IsNullOrEmpty(obj as string))
				//	{
				//		try
				//		{
				//			await Utils.PushAsync(Navigation, new SecurityQuestionPage() 
				//			{
				//				BindingContext = this.BindingContext,
				//				BrainTreeCard = brainTreeCard,
				//			});
				//		}
				//		catch (Exception ex)
				//		{
				//			var notificator = DependencyService.Get<IToastNotificator>();
				//			await notificator.Notify(ToastNotificationType.Error, AppResources.PaymentMethodTitle, ex.Message, TimeSpan.FromSeconds(5));
				//			System.Diagnostics.Debug.WriteLine("Exception occured When Add Payment Method : " + ex.Message);
				//		}
				//		finally
				//		{
				//			if (!handle)
				//			{
				//				(sender as VisualElement).IsEnabled = true;
				//				this.IsBusy = false;
				//			}
				//		}
				//	}
				//});
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
