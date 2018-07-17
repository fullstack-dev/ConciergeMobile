using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ColonyConcierge.Mobile.Customer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OrangeButton : ContentView
	{
		public event EventHandler Clicked;

		public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(ActivityIndicatorView), default(string), BindingMode.OneWay, null, (bindable, oldValue, newValue) =>
		{
			if (bindable is OrangeButton)
			{
				var orangeButton = bindable as OrangeButton;
				Device.BeginInvokeOnMainThread(() =>
				{
					orangeButton.LabelText.Text = Convert.ToString(newValue);
				});
			}
		});

		public string Text
		{
			get
			{

				return (string)GetValue(TextProperty);
			}
			set
			{
				SetValue(TextProperty, value);
			}
		}

		private Color mTextColor;
		public Color TextColor
		{
			get
			{
				return mTextColor;
			}
			set
			{
				mTextColor = value;
			}
		}

		public double FontSize
		{
			get
			{
				return LabelText.FontSize;
			}
			set
			{
				LabelText.FontSize = value;
			}
		}

		public OrangeButton()
		{
			InitializeComponent();

			mTextColor = LabelText.TextColor;

			this.PropertyChanged += (sender, e) =>
			{
				if (nameof(IsEnabled) == e.PropertyName)
				{
					GridButton.IsEnabled = IsEnabled;
					if (IsEnabled)
					{
						LabelText.TextColor = mTextColor;
					}
					else 
					{
						LabelText.TextColor = AppearanceBase.Instance.LightGray2;
					}
				}
			};

			ImageButton.SizeChanged += (sender, e) =>
			{
				if (ImageButton.Height > 1 && ImageButton.Width > 1)
				{
					GridButton.HeightRequest = ImageButton.Height;
                    this.HeightRequest = ImageButton.Height;
					Task.Run(() =>
					{
						new System.Threading.ManualResetEvent(false).WaitOne(50);
					}).ContinueWith(t =>
					{
						LabelText.IsVisible = true;
					}, TaskScheduler.FromCurrentSynchronizationContext());
				}
			};

			var lockTouch = false;
			ImageButton.Touch += async (sender, e) =>
			{
				if (IsEnabled)
				{
					if (!lockTouch)
					{
						lockTouch = true;
						ImageButtonClick.HeightRequest = ImageButton.Height;
						if (e == true)
						{
							ImageButtonClick.IsVisible = true;
							lockTouch = false;
						}
						else
						{
							try
							{
								await Task.Delay(100);
								lockTouch = false;
								ImageButtonClick.IsVisible = false;
								if (Clicked != null)
								{

									Clicked(this, EventArgs.Empty);
								}
								//Task.Run(() =>
								//{
								//	new System.Threading.ManualResetEvent(false).WaitOne(100);
								//}).ContinueWith(t =>
								//{
								//	lockTouch = false;
								//	ImageButtonClick.IsVisible = false;
								//	Clicked(this, EventArgs.Empty);
								//}, TaskScheduler.FromCurrentSynchronizationContext());
							}
							catch (Exception)
							{
                                Clicked(this, EventArgs.Empty);
							}
						}
					}
				}
			};
		}
	}
}
