using System;
using System.Collections.Generic;
using System.Linq;
using ColonyConcierge.APIData.Data;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace ColonyConcierge.Mobile.Customer
{
	public class CardEntry : Entry
	{
		public Page ParenntPage { get; set; }
		public event EventHandler<CreditCardData> CardChanged;
		public bool IsNeedClearFocus
		{
			get;
			set;
		}

		public CreditCardData mCardSelected = null;
		public CreditCardData CardSelected
		{
			get
			{
				return mCardSelected;
			}
			set
			{
				mCardSelected = value;
				Device.BeginInvokeOnMainThread(() =>
				{
					if (mCardSelected != null)
					{
						var number = string.Empty;
						var index = mCardSelected.CreditCardNumber.IndexOf("*", StringComparison.OrdinalIgnoreCase);
						if (index < mCardSelected.CreditCardNumber.Length - 1)
						{
							number = mCardSelected.CreditCardNumber.Substring(index + 1);
						}
						this.Text = mCardSelected.AccountNickname + " - " + number;
					}
					else
					{
						this.Text = string.Empty;
					}
				});
			}
		}

		public CardEntry()
		{
			this.Focused += async (sender, e) =>
			{
				if (e.IsFocused)
				{
					if (Device.RuntimePlatform == Device.iOS)
					{
						Device.BeginInvokeOnMainThread(() =>
						{
                            this.IsNeedClearFocus = true;
							OnPropertyChanged(nameof(IsNeedClearFocus));
						});
					}
					else
					{
						//Device.BeginInvokeOnMainThread(() =>
						//{
                            this.IsEnabled = false;
							this.Unfocus();
						//});
					}
					SelectCardPage selectCardPage = new SelectCardPage(CardSelected);
					selectCardPage.CardSelected += (sender2, e2) =>
					{
						CardSelected = e2.Model;
						if (CardChanged != null)
						{
							CardChanged(this, CardSelected);
						}
						var pages = Navigation.NavigationStack.Reverse().Skip(1).ToList();
						if (ParenntPage != null)
						{
							foreach (var page in pages)
							{
								if (page != ParenntPage)
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
					};
					await Utils.PushAsync(Navigation, selectCardPage, true);
					//if (Device.RuntimePlatform == Device.iOS)
					//{
					//	this.IsNeedClearFocus = true;
					//	OnPropertyChanged(nameof(IsNeedClearFocus));
					//}
				}
			};
			this.Unfocused += (sender, e) =>
			{
				this.IsEnabled = true;
                this.IsVisible = true;
			};
		}
	}
}
