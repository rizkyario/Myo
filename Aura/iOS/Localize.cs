using System;
using Aura.iOS;
using Xamarin.Forms;
using System.Threading;
using Foundation;

[assembly:Dependency(typeof(Localize))]
namespace Aura.iOS
{
	public class Localize : ILocalize
	{
		public System.Globalization.CultureInfo GetCurrentCultureInfo ()
		{
			var netLanguage = "en";
			if (NSLocale.PreferredLanguages.Length > 0) {
				var pref = NSLocale.PreferredLanguages [0];
				netLanguage = pref.Replace ("_", "-"); // turns pt_BR into pt-BR
			}
			return new System.Globalization.CultureInfo(netLanguage);
		}
	}
}

