using System;
using Xamarin.Forms;
using Aura;
using Aura.Droid;
using Xamarin.Forms.Platform.Android;
using Xamarin.Auth;
using System.Threading.Tasks;
using Facebook;
using System.Collections.Generic;
using Aura.Helpers;
using System.Net;
using Newtonsoft.Json;


[assembly: ExportRenderer(typeof(LoginPage), typeof(LoginPageRenderer))]
namespace Aura.Droid
{
	public class LoginPageRenderer : PageRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<Page> e)
		{
			base.OnElementChanged(e);

			if (e.OldElement != null || Element == null)
				return;
			try
			{
				var activity = this.Context;
				OAuth2Authenticator auth;
				if (((LoginPage)Element).OAuthType == "Facebook")
				{
					auth = new OAuth2Authenticator(
						clientId: App.OAuthDefault.ClientId, // your OAuth2 client id
						scope: App.OAuthDefault.Scope, // The scopes for the particular API you're accessing. The format for this will vary by API.
						authorizeUrl: new Uri(App.OAuthDefault.AuthorizeUrl), // the auth URL for the service
						redirectUrl: new Uri(App.OAuthDefault.RedirectUrl)); // the redirect URL for the service


				}
				else {
					auth = new OAuth2Authenticator(
						clientId: App.OAuthGoogle.ClientId,
						clientSecret: App.OAuthGoogle.ClientSecret,
						scope: App.OAuthGoogle.Scope,
						authorizeUrl: new Uri(App.OAuthGoogle.AuthorizeUrl),
						redirectUrl: new Uri(App.OAuthGoogle.RedirectUrl),
						accessTokenUrl: new Uri(App.OAuthGoogle.AccessTokenUrl),
						getUsernameAsync: null
					);
				}


				auth.Completed += OnAuthCompleted;
				activity.StartActivity(auth.GetUI(activity));
			}
			catch (Exception ex)
			{
				Aura.Debug.WriteLine(ex.Message);
			}
		}


		async void OnAuthCompleted(object sender, AuthenticatorCompletedEventArgs e)
		{
			if (e.IsAuthenticated)
				if (((LoginPage)Element).OAuthType == "Facebook")
					Settings.CurrentUser = await GetFBInfo(e.Account.Properties["access_token"]);
				else
					Settings.CurrentUser = await GetGoogleInfo(e.Account.Properties["access_token"]);


			App.Instance.FinishLogin.Invoke();
		}

		private async Task<User> GetGoogleInfo(string userToken)
		{

			User u = new User();
			string strURL = "https://www.googleapis.com/oauth2/v1/userinfo?access_token=" + userToken;

			WebClient client = new WebClient();

			await client.DownloadStringTaskAsync(new Uri(strURL)).ContinueWith(t =>
			{
				if (!t.IsFaulted)
				{
					GoogleUserInfo googleInfo = JsonConvert.DeserializeObject<GoogleUserInfo>(t.Result);
					u = new User()
					{
						Name = (googleInfo.name != "" ? googleInfo.name : "Isara User"),
						Token = userToken,
						Email = googleInfo.email,
						Facebook = "",
						Image = googleInfo.picture,
						Language = "English",
						Location = "Jakarta"
					};
				}
				Console.WriteLine(t.Result);
			});

			return u;
		}

		private async Task<User> GetFBInfo(string userToken)
		{

			User u = new User();
			FacebookClient fb = new FacebookClient(userToken);
			await fb.GetTaskAsync("me").ContinueWith(t =>
				{
					if (!t.IsFaulted)
					{
						IDictionary<string, object> result = (IDictionary<string, object>)t.Result;
						object email;
						result.TryGetValue("email", out email);

						if (email == null)
							email = (string)result["id"];

						u = new User()
						{
							Name = (string)result["name"],
							Token = userToken,
							Email = (string)email,
							Facebook = (string)result["id"],
							Image = String.Format("http://graph.facebook.com/{0}/picture?type=large", (string)result["id"]),
							Language = "English",
							Location = "Jakarta"
						};
						Console.WriteLine(result);
					}
				});

			return u;
		}
	}

	public class GoogleUserInfo
	{
		public string id { get; set; }
		public string email { get; set; }
		public bool verified_email { get; set; }
		public string name { get; set; }
		public string given_name { get; set; }
		public string family_name { get; set; }
		public string link { get; set; }
		public string picture { get; set; }
		public string gender { get; set; }
	}
}

