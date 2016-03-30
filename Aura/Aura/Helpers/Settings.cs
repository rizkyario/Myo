
using Newtonsoft.Json;
using System;
using System.IO;
using Accord.Neuro.Networks;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Accord.Neuro.ActivationFunctions;
using Plugin.Settings.Abstractions;
using Plugin.Settings;

namespace Aura.Helpers
{
  /// <summary>
  /// This is the Settings static class that can be used in your Core solution or in any
  /// of your client applications. All settings are laid out the same exact way with getters
  /// and setters. 
  /// </summary>
	public static class Settings
	{
		private static ISettings AppSettings {
			get {
				return CrossSettings.Current;
			}
		}

		#region Setting Constants

		private const string UserKey = "user_key"+App.debugNumber;
		private static readonly string UserDefault = JsonConvert.SerializeObject(null);

		private const string MyoKey = "myo_key"+App.debugNumber;
		private static readonly string MyoDefault = JsonConvert.SerializeObject(Guid.Empty);


		#endregion


		public static User CurrentUser {
			get {
				return JsonConvert.DeserializeObject<User>(AppSettings.GetValueOrDefault (UserKey, UserDefault));
			}
			set {
				AppSettings.AddOrUpdateValue (UserKey, JsonConvert.SerializeObject(value));
			}
		}

		public static Guid DefaultMyo {
			get {
				return JsonConvert.DeserializeObject<Guid>(AppSettings.GetValueOrDefault (MyoKey, MyoDefault));
			}
			set {
				AppSettings.AddOrUpdateValue (MyoKey, JsonConvert.SerializeObject(value));
			}
		}



		public static string LocationSettings {
			get {
				return CurrentUser.Location;
			}
			set {
				User u = CurrentUser;
				u.Location = value;
				CurrentUser = u;
			}
		}

		public static User GetDummyUser()
		{
			string dummyUser = "{\"id\":\"9322ecad-7781-40a1-b589-77117cd31d0b\",\"name\":\"Isara\",\"email\":\"isaratv@gmail.com\",\"facebook\":\"1468780186\",\"image\":\"http://www.isara.tv/wp-content/uploads/2015/04/logo-100x100.png\",\"language\":\"English\",\"token\":\"CAAUsLlDr2W4BAATB4ZCS6ZCYSicWezq41ZAl0zgZANqdSMCSMpbATcnwe8OaZCY7AEzu5iPg2LIlVMijMk6CCS0J4BO4lVOZBSLhgBvUHLS3gfaFitpmfY4lMeGHGJ91NpN0RVtF6lznjpjxvoqKU5OMRZBCeVjlSVJD0sqROYDFCAHJZCZBY12R0gnQeQRVzNxlqTwhcMkiVMKyZBxZAdT0JvRXZAZAgD4Y7r8EZD\",\"location\":\"Bandung\",\"followers\":null,\"following\":null}";
			return JsonConvert.DeserializeObject<User>(dummyUser);
		}

	}



	public class OAuthSettings
	{
		public OAuthSettings(
			string clientId,
			string clientSecret,
			string scope,
			string authorizeUrl,
			string redirectUrl,
			string accessTokenUrl,
			string getUsernameAsync)
		{
			ClientId = clientId;
			ClientSecret = clientSecret;
			Scope = scope;
			AuthorizeUrl = authorizeUrl;
			RedirectUrl = redirectUrl;
			AccessTokenUrl = accessTokenUrl;
			GetUsernameAsync = getUsernameAsync;
		}
		public OAuthSettings(
			string clientId,
			string scope,
			string authorizeUrl,
			string redirectUrl)
		{
			ClientId = clientId;
			Scope = scope;
			AuthorizeUrl = authorizeUrl;
			RedirectUrl = redirectUrl;
		}
		public string ClientId {get; private set;}
		public string ClientSecret {get; private set;}
		public string Scope {get; private set;}
		public string AuthorizeUrl {get; private set;}
		public string RedirectUrl{get; private set;}
		public string AccessTokenUrl{get; private set;}
		public string GetUsernameAsync{get; private set;}
	}
}