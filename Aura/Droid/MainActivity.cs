using Android.App;
using Android.Widget;
using Android.OS;
using XLabs.Forms;
using Acr.UserDialogs;
using XLabs.Ioc;
using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Xamarin;
using XLabs.Platform.Device;
using XLabs.Serialization;
using XLabs.Platform.Services;
using ModernHttpClient;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using XLabs.Serialization.JsonNET;
using Android.Views;
using Android.Content.PM;
using Android.Graphics;

namespace Aura.Droid
{
	[Activity(Label = "Aura", Icon = "@drawable/icon", NoHistory = true, WindowSoftInputMode = SoftInput.AdjustPan, Theme = "@style/Theme.MainActivity", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, HardwareAccelerated = false)]
	public class MainActivity : XFormsApplicationDroid
	{
		MobileServiceClient Client;
		IMobileServiceSyncTable<User> userTable;
		IMobileServiceSyncTable<Sign> signTable;
		IMobileServiceSyncTable<Gesture> gestureTable;

		public static Typeface Font;

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			OxyPlot.Xamarin.Forms.Platform.Android.Forms.Init();
			global::Xamarin.Forms.Forms.Init(this, bundle);
			global::Xamarin.Forms.Forms.SetTitleBarVisibility(Xamarin.Forms.AndroidTitleBarVisibility.Never);//Oculta la barra
			InitiateAzureMobile();
			InitiateXlab();
			InitializeInsight();

			UserDialogs.Init(() => this);
			Font = Typeface.CreateFromAsset(Xamarin.Forms.Forms.Context.Assets, "FontAwesome.ttf");

			LoadApplication(App.Instance);
		}

		public void InitializeInsight()
		{
			Insights.HasPendingCrashReport += (sender, isStartupCrash) =>
			{
				if (isStartupCrash)
				{
					Insights.PurgePendingCrashReports().Wait();
				}
			};

			#if DEBUG
			Insights.Initialize(Insights.DebugModeKey, this);
			#else
			Insights.Initialize(App.insightKey, this);
			#endif
		}

		public void InitiateXlab()
		{
			var container = new SimpleContainer();
			container.Register<IDevice>(t => AndroidDevice.CurrentDevice);
			container.Register<IDisplay>(t => t.Resolve<IDevice>().Display);
			container.Register<INetwork>(t => t.Resolve<IDevice>().Network);
			container.Register<IJsonSerializer, JsonSerializer>();

			Resolver.ResetResolver();
			Resolver.SetResolver(container.GetResolver());

		}

		public void InitiateAzureMobile()
		{
			CurrentPlatform.Init();

			Client = new MobileServiceClient(
				App.applicationURL,
				App.applicationKey, new NativeMessageHandler());

			InitializeStoreAsync().Wait();
			userTable = Client.GetSyncTable<User>();
			signTable = Client.GetSyncTable<Sign>();
			gestureTable = Client.GetSyncTable<Gesture>();

			App.UserServer = new UserService(Client, userTable);
			App.SignServer = new SignService(Client, signTable);
			App.GestureServer = new GestureService(Client, gestureTable);

		}

		public async Task InitializeStoreAsync()
		{
			string path = App.localDbPath;
			var store = new MobileServiceSQLiteStore(path);

			store.DefineTable<User>();
			store.DefineTable<Sign>();
			store.DefineTable<Gesture>();

			await Client.SyncContext.InitializeAsync(store, new MobileServiceSyncHandler());
		}

		public override void OnBackPressed()
		{
			if (App.Instance.DoBack)
			{
				base.OnBackPressed();
			}
		}
	}
}


