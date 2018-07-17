using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ColonyConcierge.Mobile.Customer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NumberEntryView : ContentView
	{
		public Entry Entry
		{
			get
			{
				return EntryNumber;
			}
		}

		public NumberEntryView()
		{
			InitializeComponent();

			//LabelLess.FontSize = Device.GetNamedSize(NamedSize.Large, LabelLess) * 1.4;
			//LabelAdd.FontSize = Device.GetNamedSize(NamedSize.Large, LabelAdd) * 1.4;

			//ImageButton.SizeChanged += (sender, e) =>
			//{
			//	if (ImageButton.Width > 1 && ImageButton.Height > 1)
			//	{
			//		GridButton.WidthRequest = ImageButton.Width;
			//		GridButton.HeightRequest = ImageButton.Height;
			//		GridButton.IsVisible = true;
			//	}
			//};

			Entry.TextChanged += (sender, e) =>
			{
				LabelNumber.Text = Entry.Text;
			};

			//ButtonAdd.Clicked += (sender, e) =>
			//{
			//	try
			//	{
			//		EntryNumber.Text = Convert.ToString(Convert.ToInt32(EntryNumber.Text) + 1);
			//	}
			//	catch (Exception)
			//	{
			//		EntryNumber.Text = "1";
			//	}
			//};

			//ButtonLess.Clicked += (sender, e) =>
			//{
			//	try
			//	{
			//		EntryNumber.Text = Convert.ToString(Math.Max(1, Convert.ToInt32(EntryNumber.Text) - 1));
			//	}
			//	catch (Exception)
			//	{
			//		EntryNumber.Text = "1";
			//	}
			//};

			ButtonAdd.GestureRecognizers.Add(new TapGestureRecognizer()
			{
				Command = new Command(() =>
				{
					if (StackLayoutEnabledAdd.IsEnabled)
					{
						try
						{
							EntryNumber.Text = Convert.ToString(Convert.ToInt32(EntryNumber.Text) + 1);
						}
						catch (Exception)
						{
							EntryNumber.Text = "1";
						}
					}
				}),
			});

			ButtonLess.GestureRecognizers.Add(new TapGestureRecognizer()
			{
				Command = new Command(() =>
				{
					if (StackLayoutEnabled.IsEnabled)
					{
						try
						{
							EntryNumber.Text = Convert.ToString(Math.Max(1, Convert.ToInt32(EntryNumber.Text) - 1));
						}
						catch (Exception)
						{
							EntryNumber.Text = "1";
						}
					}
				}),
			});
		}
	}
}
