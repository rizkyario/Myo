using System;
using System.Threading;
using Aura.Droid;
using Xamarin.Forms;

[assembly:Dependency(typeof(Localize))]
namespace Aura.Droid
{
	public class Localize: ILocalize
	{
		public System.Globalization.CultureInfo GetCurrentCultureInfo ()
		{
			var androidLocale = Java.Util.Locale.Default;
			string netLanguage = androidLocale.ToString ();
			netLanguage = androidLocale.ToString().Replace ("in", "id"); // fix android
			netLanguage = netLanguage.Split('_')[0];
			System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("en");
			Aura.Debug.WriteLine(string.Format("Phone's Language: {0}",netLanguage));

			try{
				culture = new System.Globalization.CultureInfo(netLanguage);
			}catch(Exception e)
			{
				Aura.Debug.WriteLine (e.Message);
			}
			return culture;
		}

		public void SetLocale ()
		{
			var androidLocale = Java.Util.Locale.Default; // user's preferred locale
			var netLocale = androidLocale.ToString().Replace ("_", "-"); 

			netLocale = androidLocale.ToString().Replace ("in", "id"); // fix android
			netLocale = netLocale.Split('_')[0];
			System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("en");

			try{
				culture = new System.Globalization.CultureInfo(netLocale);

				Thread.CurrentThread.CurrentCulture = culture;
				Thread.CurrentThread.CurrentUICulture = culture;
				//				culture = new System.Globalization.CultureInfo(netLanguage);
			}catch(Exception e)
			{
				Thread.CurrentThread.CurrentCulture = culture;
				Thread.CurrentThread.CurrentUICulture = culture;
				Aura.Debug.WriteLine (e.Message);
			}

		}
	}
}

