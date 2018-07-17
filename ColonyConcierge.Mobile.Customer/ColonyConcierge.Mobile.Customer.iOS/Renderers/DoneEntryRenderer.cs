using System;
using System.Drawing;
using System.Threading.Tasks;
using ColonyConcierge.Mobile.Customer;
using ColonyConcierge.Mobile.Customer.iOS;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(Entry), typeof(DoneEntryRenderer))]
[assembly: ExportRenderer(typeof(NoSuggestionEntry), typeof(DoneEntryRenderer))]
[assembly: ExportRenderer(typeof(AddressEntry), typeof(DoneEntryRenderer))]
[assembly: ExportRenderer(typeof(CardEntry), typeof(DoneEntryRenderer))]
[assembly: ExportRenderer(typeof(EntryCustom), typeof(DoneEntryRenderer))]

namespace ColonyConcierge.Mobile.Customer.iOS
{
	public class DoneEntryRenderer : EntryRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
		{
			base.OnElementChanged(e);
			if (this.Element != null)
			{
				if (this.Element.FontAttributes == FontAttributes.Bold)
				{
					this.Element.FontFamily = iOS.Appearance.Instance.FontFamilyBold;
				}
				else
				{
					this.Element.FontFamily = iOS.Appearance.Instance.FontFamilyDefault;
				}

				if (this.Element.Keyboard == Keyboard.Numeric || this.Element.Keyboard == Keyboard.Telephone)
				{
					var toolbar = new UIToolbar(new RectangleF(0.0f, 0.0f, (float)Control.Frame.Size.Width, 44.0f));

					toolbar.Items = new[]
					{
						new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace),
						new UIBarButtonItem(UIBarButtonSystemItem.Done, delegate { Control.ResignFirstResponder(); })
					};

					this.Control.InputAccessoryView = toolbar;
					this.Control.KeyboardType = UIKeyboardType.NumberPad;
				}
				this.Control.AutocorrectionType = UITextAutocorrectionType.No;
			}

			if (this.Element is CardEntry)
			{
				var cardEntry = this.Element as CardEntry;
				cardEntry.PropertyChanged += (sender2, e2) =>
				{
					if (cardEntry.IsNeedClearFocus)
					{
						cardEntry.IsNeedClearFocus = false;
						if (this.Control != null)
						{
							this.Control.ResignFirstResponder();
							this.Control.EndEditing(true);
						}
					}
				};
			}
			if (this.Element is AddressEntry)
			{
				var addressEntry = this.Element as AddressEntry;
				addressEntry.PropertyChanged += (sender2, e2) =>
				{
					if (addressEntry.IsNeedClearFocus)
					{
						addressEntry.IsNeedClearFocus = false;
						if (this.Control != null)
						{
							this.Control.ResignFirstResponder();
							this.Control.EndEditing(true);
						}
					}
				};
			}
		}

		protected override async void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (this.Element is EntryCustom && this.Control != null)
			{
				EntryCustom entryCustom = this.Element as EntryCustom;
				if (entryCustom.NeedShowKeyboard)
				{
					entryCustom.NeedShowKeyboard = false;
					await Task.Delay(200);
					Device.BeginInvokeOnMainThread(() =>
					{
						if (this.Control != null)
						{
							this.Control.BecomeFirstResponder();
							if (this.Control.EndOfDocument != null)
							{
								this.Control.SelectedTextRange = this.Control.GetTextRange(this.Control.EndOfDocument, this.Control.EndOfDocument);
							}
						}
					});
				}
			}
		}
	}
}
