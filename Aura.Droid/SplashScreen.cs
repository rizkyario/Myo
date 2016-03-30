
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Aura.Droid
{
	[Activity(Label = "Aura", Icon = "@drawable/icon", MainLauncher = true, Theme = "@style/Theme.Splash", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class SplashScreen : Activity
	{
		protected override void OnCreate(Bundle bundle)
		{
			Xamarin.Insights.Initialize(App.insightKey, this);
			base.OnCreate(bundle);
			var intent = new Intent(this, typeof(MainActivity));
			StartActivity(intent);
			Finish();
		}
	}
}

