using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Aura.Helpers;
using Xamarin;
using OxyPlot.Xamarin.Forms;

namespace Aura
{
	public partial class App : Application
	{
		public static readonly OAuthSettings OAuthDefault = new OAuthSettings (
			clientId: "1455952321304942",
			scope: "",
			authorizeUrl: "https://m.facebook.com/dialog/oauth/",
			redirectUrl: "https://www.facebook.com/connect/login_success.html");

		public static readonly OAuthSettings OAuthGoogle = new OAuthSettings (
			clientId: "257059293281-e01fsb9qo8cov281so8heskhqis3rvmp.apps.googleusercontent.com",
			clientSecret: "Rdw3pIMhqJlfsnNrpaMxHbpQ",
			scope: "openid email ",
			authorizeUrl: "https://accounts.google.com/o/oauth2/auth",
			redirectUrl: "http://localhost",
			accessTokenUrl: "https://accounts.google.com/o/oauth2/token",
			getUsernameAsync: null);
		

		public const string applicationURL = @"http://isaramobile.azure-mobile.net/";
		public const string applicationKey = @"ZBgdjiMLuWWdPyUCLmYISAqsCtnrWu66";
		public const string debugNumber	   = "0";
		public const string localDbPath    = "Aura"+debugNumber+".db";

		public const string insightKey    = "9541cbaf8298af0bd9ce18375264b234ff3c3530";

		static volatile App _Instance;
		static object _SyncRoot = new Object();

		public static UserService UserServer;
		public static SignService SignServer;
		public static GestureService GestureServer;

		static public AuraViewModel AuraVM;

		public static App Instance {
			get {
				if (_Instance == null)
					lock (_SyncRoot)
						if (_Instance == null)
							_Instance = new App ();

				return _Instance;
			}
		}

		public App ()
		{
			InitializeComponent ();
			InitializePage ();
		}

		public async void InitializePage (){
			NavigationPage NavPage;
			Insights.Identify(Insights.Traits.GuestIdentifier, null);

			if (Settings.CurrentUser != null) {
				
				var traits = new Dictionary<string, string> {
					{ Insights.Traits.Email, Settings.CurrentUser.Email },
					{ Insights.Traits.Name, Settings.CurrentUser.Name }
				};

				Insights.Identify (Insights.Traits.GuestIdentifier, traits);

				NavPage = new NavigationPage (new MainMenu ());
			}
			else
				NavPage = new NavigationPage (new IntroPage ());

			NavPage.BarTextColor = Color.White;

			this.MainPage =  NavPage;
		}

		public Action FinishLogin {
			get {
				return new Action (async () => {
					if (Settings.CurrentUser != null) {
						Settings.CurrentUser = await UserServer.GetUserByEmailAsync (Settings.CurrentUser);

						if (Device.OS == TargetPlatform.iOS) {
							// move layout under the status bar
							while (MainPage.Navigation.ModalStack.Count > 0)
								await MainPage.Navigation.PopModalAsync ();
						}

						InitializePage ();

					} else
						await MainPage.Navigation.PopModalAsync ();
				});
			}
		}

		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}

