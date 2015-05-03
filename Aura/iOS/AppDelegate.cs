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
using XLabs.Forms.Charting.Controls;

namespace Aura.iOS
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
	{
		MobileServiceClient Client;
		IMobileServiceSyncTable<User> userTable;
		public static PageRenderer MainMenu;


		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			OxyPlot.Xamarin.Forms.Platform.iOS.Forms.Init();

			global::Xamarin.Forms.Forms.Init ();
			InitiateAzureMobile ();
			InitiateXlab ();
			InitializeInsight ();
			LoadApplication (App.Instance);
			return base.FinishedLaunching (app, options);
		}

		public void InitiateXlab()
		{
			var container = new SimpleContainer ();
			container.Register<IDevice> (t => AppleDevice.CurrentDevice);
			container.Register<IDisplay> (t => t.Resolve<IDevice> ().Display);
			container.Register<INetwork>(t=> t.Resolve<IDevice>().Network);

			//Load Chart Renderer
			ChartRenderer chartRenderer = new ChartRenderer (); 

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


			InitializeStoreAsync().Wait();
			userTable = Client.GetSyncTable<User>();


			App.UserServer = new UserService(Client,userTable);

		}

		public async Task InitializeStoreAsync()
		{
			string path = App.localDbPath;
			var store = new MobileServiceSQLiteStore (path);

			store.DefineTable<User> ();


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
	}
}

