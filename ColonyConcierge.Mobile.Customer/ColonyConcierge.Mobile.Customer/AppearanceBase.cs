using System;
using Xamarin.Forms;

namespace ColonyConcierge.Mobile.Customer
{
	public class AppearanceBase
	{
		protected static AppearanceBase mInstance;
		public static AppearanceBase Instance
		{
			get
			{
				return mInstance;
			}
		}

		//for Android, please set color on Resources/values/styles.xml too
		public Color PrimaryColor
		{
			get
			{
				return Color.FromHex("#43b02a");
			}
		}

		//for Android, please set color on Resources/values/styles.xml too
		public Color PrimaryDarkColor
		{
			get
			{
				return Color.FromHex("#41AF49");
			}
		}

		public Color OrangeColor
		{
			get
			{
				return Color.FromHex("#ff8200");
			}
		}

		public Color LightGray
		{
			get
			{
				return Color.FromHex("#E6E6E6");
			}
		}

		public Color LightGray2
		{
			get
			{
				return Color.FromHex("#C3C3C3");
			}
		}

		public Color DrakGray
		{
			get
			{
				return Color.FromHex("#a9a9a9");
			}
		}

		public Color GroupSeparatorColor
		{
			get
			{
				return Color.FromHex("#F1F1F1");
			}
		}

		public Color LineSeparatorColor
		{
			get
			{
				return Color.FromHex("#ECF1F4");
			}
		}

		public string FontNameDefault
		{
			get
			{
				return "Montserrat-Regular";
			}
		}

		public string FontFileNameDefault
		{
			get
			{
				return FontNameDefault + ".ttf";
			}
		}

		public string FontNameBold
		{
			get
			{
				return "Montserrat-SemiBold";
			}
		}

		public string FontFileNameBold
		{
			get
			{
				return FontNameBold + ".ttf";
			}
		}

		public string FontFamilyDefault
		{
			get
			{
				if (Device.RuntimePlatform == Device.Android)
				{
					return FontFileNameDefault + "#" + FontNameDefault;
				}
				else
				{
					return FontNameDefault;
				}
			}
		}

		public string FontFamilyBold
		{
			get
			{
				if (Device.RuntimePlatform == Device.Android)
				{
					return FontFileNameBold + "#" + FontNameBold;
				}
				else
				{
					return FontNameBold;
				}
			}
		}

		public Color TextColor
		{
			get
			{
				return Color.White;
			}
		}

		public double FontMediumL1
		{
			get
			{
				return Device.GetNamedSize(NamedSize.Medium, typeof(Label)) * 0.9;
			}
		}

		protected AppearanceBase()
		{
			this.Configure();
		}

		protected void Configure()
		{
			OnConfigure();
		}

		protected virtual void OnConfigure()
		{
		}
	}
}
