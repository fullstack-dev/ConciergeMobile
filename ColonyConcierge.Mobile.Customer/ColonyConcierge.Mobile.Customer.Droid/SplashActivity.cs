using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Util;
using Android.Support.V7.App;
using Android.Preferences;
using Android.Content.PM;
using Xamarin.Forms;
using Newtonsoft.Json;
using ColonyConcierge.APIData.Data.Logistics.NotificationData;

namespace ColonyConcierge.Mobile.Customer.Droid
{
	[Activity(Theme = "@style/MainTheme.Splash", MainLauncher = true, ScreenOrientation = ScreenOrientation.Portrait, NoHistory = false)]
	//[IntentFilter(new[] { Intent.ActionView },
	//		  AutoVerify = true,
	//		  Categories = new[] { Intent.CategoryBrowsable, Intent.CategoryDefault },
	//		  DataScheme = "asystyou",
	//		  DataHost = "restaurant")]
	//[IntentFilter(new[] { Intent.ActionView },
	//		  AutoVerify = true,
	//		  Categories = new[] { Intent.CategoryBrowsable, Intent.CategoryDefault },
	//		  DataScheme = "asystyou",
	//		  DataHost = "Restaurant/Index/fitz_restaurant_school_lunch/?destination_id=1")]
	public class SplashActivity : AppCompatActivity
	{
		private Handler UIHandler;
		private int SplashTime = 500;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.ActivitySplash);

			UIHandler = new Handler();

			if (Intent.Data != null)
			{
				AppServices appServices = null;
				var isAppOpen = false;
				try
				{
					appServices = DependencyService.Get<IAppServices>() as AppServices;
					isAppOpen = appServices != null && appServices.MainActivity != null && !appServices.MainActivity.IsDestroyed && !appServices.MainActivity.IsFinishing;
				}
				catch (Exception)
				{
					isAppOpen = false;
				}

				if (!isAppOpen)
				{
					Intent intent = new Intent(this, this.GetType());
					intent.AddFlags(ActivityFlags.MultipleTask | ActivityFlags.NewTask | ActivityFlags.ClearTask | ActivityFlags.NoAnimation);
					intent.PutExtra("isNewTask", false);
					intent.PutExtra("DataUri", Intent.Data);
					StartActivity(intent);
					this.OverridePendingTransition(0, 0);
					if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
					{
						this.FinishAndRemoveTask();
					}
					else
					{
						this.Finish();
					}
				}
				else
				{
					appServices.MainActivity.MoveTaskToBack(true);
					Intent intent = new Intent(this, typeof(MainActivity));
					intent.AddFlags( ActivityFlags.ClearTop | ActivityFlags.SingleTop | ActivityFlags.NoAnimation);
					intent.PutExtra("DataUri", Intent.Data);
					appServices.MainActivity.StartActivity(intent);
					if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
					{
					    this.FinishAndRemoveTask();
					}
					else
					{
						this.Finish();
					}
                    this.OverridePendingTransition(0, 0);
				}
			}
			else if (Intent != null && Intent.Extras != null && Intent.Extras.KeySet().Count > 0)
			{
				var isNewTask = this.Intent.Extras.GetBoolean("isNewTask", true);
				AppServices appServices = null;
				if (isNewTask)
				{
					try
					{
						appServices = DependencyService.Get<IAppServices>() as AppServices;
						isNewTask = !(appServices != null && appServices.MainActivity != null && !appServices.MainActivity.IsDestroyed && !appServices.MainActivity.IsFinishing);
					}
					catch
					{
						isNewTask = true;
					}
				}

				if (isNewTask && Intent.Extras.ContainsKey("logistics"))
				{
					UIHandler.PostDelayed(() =>
					{
						Intent intent = new Intent(this, this.GetType());
						intent.AddFlags(ActivityFlags.MultipleTask | ActivityFlags.NewTask | ActivityFlags.ClearTask | ActivityFlags.NoAnimation);
						intent.PutExtra("isNewTask", false);
						intent.PutExtras(this.Intent.Extras);
						StartActivity(intent);
						this.OverridePendingTransition(0, 0);
						if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
						{
							this.FinishAndRemoveTask();
						}
						else
						{
							this.Finish();
						}
					}, SplashTime);
				}
				else
				{
					if (appServices != null && appServices.MainActivity != null
						&& !appServices.MainActivity.IsDestroyed && !appServices.MainActivity.IsFinishing
						&& Intent.Extras.ContainsKey("logistics"))
					{
						string value = string.Empty;
						try
						{
							value = Intent.Extras.GetString("logistics");
							var logisticsNotification = JsonConvert.DeserializeObject<LogisticsNotification>(value);
							appServices.MainActivity.IsNeedRefresh = true;
							appServices.MainActivity.LogisticsNotification = logisticsNotification;
							if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
							{
								this.FinishAndRemoveTask();
							}
							else
							{
								this.Finish();
							}
							return;
						}
						catch (Exception)
						{
							if (!string.IsNullOrEmpty(value))
							{
								appServices.MainActivity.Finish();
								Intent intent = new Intent(this, typeof(MainActivity));
								intent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop | ActivityFlags.NoAnimation);
								intent.PutExtras(this.Intent.Extras);
								StartActivity(intent);
								Finish();
								this.OverridePendingTransition(0, 0);
							}
							else
							{
								if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
								{
									this.FinishAndRemoveTask();
								}
								else
								{
									this.Finish();
								}
							}
						}
					}
					else
					{
						UIHandler.PostDelayed(() =>
						{
							Intent intent = new Intent(this, typeof(MainActivity));
							intent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop | ActivityFlags.NoAnimation);
							intent.PutExtras(this.Intent.Extras);
							StartActivity(intent);
							this.Finish();
							this.OverridePendingTransition(0, 0);
						}, SplashTime);
					}
				}
			}
			else
			{
				AppServices appServices = null;
				var isAppOpen = false;
				try
				{
					appServices = DependencyService.Get<IAppServices>() as AppServices;
					isAppOpen = appServices != null && appServices.MainActivity != null && !appServices.MainActivity.IsDestroyed && !appServices.MainActivity.IsFinishing;
				}
				catch (Exception)
				{
					isAppOpen = false;
				}

				if (!isAppOpen)
				{
					UIHandler.PostDelayed(() =>
					{
						Intent intent = new Intent(this, typeof(MainActivity));
						intent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop | ActivityFlags.NoAnimation);
						StartActivity(intent);
						this.Finish();
						this.OverridePendingTransition(0, 0);
					}, SplashTime);
				}
				else
				{
					this.Finish();
				}
			}
		}

		protected override void OnNewIntent(Intent newIntent)
		{
			base.OnNewIntent(newIntent);
			if (newIntent != null && newIntent.Extras != null && newIntent.Extras.KeySet().Count > 0)
			{
				var isNewTask = newIntent.Extras.GetBoolean("isNewTask", true);
				AppServices appServices = null;
				if (isNewTask)
				{
					try
					{
						appServices = DependencyService.Get<IAppServices>() as AppServices;
						isNewTask = appServices == null;
					}
					catch
					{
						isNewTask = true;
					}
				}
				if (isNewTask && Intent.Extras.ContainsKey("logistics"))
				{
					Intent intentActivity = new Intent(this, this.GetType());
					intentActivity.AddFlags(ActivityFlags.MultipleTask | ActivityFlags.NewTask | ActivityFlags.ClearTask | ActivityFlags.NoAnimation);
					intentActivity.PutExtra("isNewTask", false);
					intentActivity.PutExtras(newIntent.Extras);
					StartActivity(intentActivity);
					this.OverridePendingTransition(0, 0);
					Finish();
				}
				else
				{
					if (appServices != null && appServices.MainActivity != null 
					    && !appServices.MainActivity.IsDestroyed && !appServices.MainActivity.IsFinishing
					    && Intent.Extras.ContainsKey("logistics"))
					{
						string value = string.Empty;
						try
						{
							value = newIntent.Extras.GetString("logistics");
							var logisticsNotification = JsonConvert.DeserializeObject<LogisticsNotification>(value);
							appServices.MainActivity.IsNeedRefresh = true;
							appServices.MainActivity.LogisticsNotification = logisticsNotification;
							this.Finish();
							return;
						}
						catch (Exception)
						{
							appServices.MainActivity.Finish();
							Intent intentActivity = new Intent(this, typeof(MainActivity));
							intentActivity.AddFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop | ActivityFlags.NoAnimation);
							intentActivity.PutExtras(newIntent.Extras);
							StartActivity(intentActivity);
							Finish();
                            this.OverridePendingTransition(0, 0);
						}
					}
				}
			}
		}
	}
}