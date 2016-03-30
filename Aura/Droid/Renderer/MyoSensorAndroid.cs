using System;
using Android.App;
using Android.Content;
using Aura.Droid;
using Com.Thalmic.Myo;
using Com.Thalmic.Myo.Scanner;

[assembly: Xamarin.Forms.Dependency(typeof(MyoSensorAndroid))]
namespace Aura.Droid
{
	public class MyoSensorAndroid : IMyoSensor
	{
		//UINavigationController controller;
		Myo myo;

		static volatile MyoSensorAndroid _Instance;
		static object _SyncRoot = new Object();

		public static MyoSensorAndroid Instance
		{
			get
			{
				if (_Instance == null)
					lock (_SyncRoot)
						if (_Instance == null)
							_Instance = new MyoSensorAndroid();

				return _Instance;
			}
		}

		public void AttachToAdjacent()
		{
			Hub.Instance.AttachToAdjacentMyo();
		}

		public void Initialize()
		{
			Hub.Instance.AttachToAdjacentMyo();
			Hub.Instance.SetLockingPolicy(Hub.LockingPolicy.None);

			App.AuraVM.Status = "Innitiated";
		}

		public void Connect()
		{
			Xamarin.Insights.Track("touchupinside");

			var intent = new Intent(Android.App.Application.Context, typeof(ScanActivity));
			intent.SetFlags(ActivityFlags.NewTask);
			Android.App.Application.Context.StartActivity(intent);
		}
	}
}

