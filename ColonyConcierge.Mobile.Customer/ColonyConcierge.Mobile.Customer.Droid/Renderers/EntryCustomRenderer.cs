using System;
using System.ComponentModel;
using Android.OS;
using Android.Text;
using Android.Text.Method;
using Android.Views.InputMethods;
using ColonyConcierge.Mobile.Customer;
using ColonyConcierge.Mobile.Customer.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Entry), typeof(EntryCustomRenderer))]
[assembly: ExportRenderer(typeof(NoSuggestionEntry), typeof(EntryCustomRenderer))]
[assembly: ExportRenderer(typeof(CardEntry), typeof(EntryCustomRenderer))]
[assembly: ExportRenderer(typeof(AddressEntry), typeof(EntryCustomRenderer))]
[assembly: ExportRenderer(typeof(EntryCustom), typeof(EntryCustomRenderer))]

namespace ColonyConcierge.Mobile.Customer.Droid
{
	public class EntryCustomRenderer : EntryRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Entry> e)
		{
			try
			{
				base.OnElementChanged(e);
				if (this.Element is NoSuggestionEntry)
				{
					this.Control.InputType = InputTypes.TextVariationVisiblePassword;
				}

				if (this.Element != null)
				{
					if (this.Element.FontAttributes == FontAttributes.Bold)
					{
						this.Element.FontFamily = Appearance.Instance.FontFamilyBold;
					}
					else
					{
						this.Element.FontFamily = Appearance.Instance.FontFamilyDefault;
					}
				}
			}
			catch (Exception)
			{
				return;
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			try
			{
				base.OnElementPropertyChanged(sender, e);

				if (this.Element is EntryCustom && this.Control != null)
				{
					EntryCustom entryCustom = this.Element as EntryCustom;
					if (entryCustom.NeedShowKeyboard)
					{
						entryCustom.NeedShowKeyboard = false;
						this.Control.PostDelayed(() =>
						{
							if (this.Control != null)
							{
								this.Control.RequestFocus();
								this.Control.SetSelection(this.Control.Text.Length);
								InputMethodManager inputMethodManager = this.Control.Context.GetSystemService(Android.Content.Context.InputMethodService) as InputMethodManager;
								inputMethodManager.ShowSoftInput(this.Control, ShowFlags.Implicit);
							}
						}, 200);
					}
				}

			}
			catch (Exception)
			{
				return;
			}
		}
	}
}
