
using Android.Graphics;

namespace ColonyConcierge.Mobile.Customer.Droid
{
	public class Appearance : AppearanceBase
	{
		public new static Appearance Instance
		{
			get
			{
				return AppearanceBase.Instance as Appearance;
			}
		}

		public static void Init()
		{
			if (mInstance == null)
			{
				var appearance = new Appearance();
				mInstance = appearance;
			}
		}

		Typeface mTypeface;
		public Typeface TypefaceDefault
		{
			get
			{
				if (mTypeface == null)
				{
					mTypeface = Typeface.CreateFromAsset(Android.App.Application.Context.Assets, FontFileNameDefault);
				}
				return mTypeface;
			}
		}

		Typeface mTypefaceBold;
		public Typeface TypefaceBold
		{
			get
			{
				if (mTypefaceBold == null)
				{
					mTypefaceBold = Typeface.CreateFromAsset(Android.App.Application.Context.Assets, FontFileNameBold);
				}
				return mTypefaceBold;
			}
		}

		private Appearance() : base()
		{
		}

		protected override void OnConfigure()
		{
			base.OnConfigure();
		}

	}
}
