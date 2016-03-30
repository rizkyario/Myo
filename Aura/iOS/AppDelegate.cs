using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;
using XLabs.Ioc;
using XLabs.Platform.Device;
using XLabs.Platform.Services;
using Xamarin;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Xamarin.Forms.Platform.iOS;

namespace Aura.iOS
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
	{
		MobileServiceClient Client;
		IMobileServiceSyncTable<User> userTable;
		IMobileServiceSyncTable<Sign> signTable;
		IMobileServiceSyncTable<Gesture> gestureTable;


		public static PageRenderer MainMenu;

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			OxyPlot.Xamarin.Forms.Platform.iOS.Forms.Init();
			global::Xamarin.Forms.Forms.Init ();
			InitiateAzureMobile ();
			InitiateXlab ();
			InitializeInsight ();
			LoadApplication (App.Instance);

			var dummy = typeof(OxyPlot.Xamarin.Forms.Platform.iOS.PlotViewRenderer);

			return base.FinishedLaunching (app, options);
		}

		public void InitiateXlab()
		{
			var container = new SimpleContainer ();
			container.Register<IDevice> (t => AppleDevice.CurrentDevice);
			container.Register<IDisplay> (t => t.Resolve<IDevice> ().Display);
			container.Register<INetwork>(t=> t.Resolve<IDevice>().Network);

			Resolver.SetResolver (container.GetResolver ());

		}

		public void InitializeInsight()
		{
			Insights.HasPendingCrashReport += (sender, isStartupCrash) =>
			{
				if (isStartupCrash) {
					Insights.PurgePendingCrashReports().Wait();
				}
			};
			#if DEBUG
			Insights.Initialize(Insights.DebugModeKey);
			#else
			Insights.Initialize(App.insightKey);
			#endif
		}

		public void InitiateAzureMobile()
		{
			CurrentPlatform.Init ();
			SQLitePCL.CurrentPlatform.Init();

			Client = new MobileServiceClient (
				App.applicationURL, 
				App.applicationKey);

//			InitUserTable().Wait();
//			InitGestureTable().Wait();

			InitializeStoreAsync().Wait();
			userTable = Client.GetSyncTable<User>();
			signTable = Client.GetSyncTable<Sign>();
			gestureTable = Client.GetSyncTable<Gesture>();

			App.UserServer = new UserService(Client,userTable);
			App.SignServer = new SignService(Client, signTable);
			App.GestureServer = new GestureService(Client, gestureTable);

		}

		public async Task InitializeStoreAsync()
		{
			string path = App.localDbPath;
			var store = new MobileServiceSQLiteStore (path);

			store.DefineTable<User> ();
			store.DefineTable<Sign> ();
			store.DefineTable<Gesture> ();

			await Client.SyncContext.InitializeAsync(store, new MobileServiceSyncHandler());
		}

		public async Task InitUserTable()
		{
			User u = new User {
				Name = "Ario",
				Email = "rizkyario@yahoo.com",
				Facebook = "848718597",
				Image = "",
				Language = "Indonesia",
				Followers ="",
				Following ="",
				Location = "Bandung",
				Token =""
			};
			await Client.GetTable<User>().InsertAsync(u);
		}

		public async Task InitGestureTable()
		{
			Gesture u = new Gesture {
				User="",
				Sign="",
				Word="",
				Dialect="",
				Accelaration="",
				EMG="",
				Gyro="",
				Orientation="",
				OrientationE="",
				Pose="",

			};
			await Client.GetTable<Gesture>().InsertAsync(u);
		}
	}
}

