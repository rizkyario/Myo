using System;
using System.Globalization;
using Xamarin.Forms;
using System.Resources;
using System.Reflection;

namespace Aura
{
	public class Localize
	{
		static readonly CultureInfo ci;

		static Localize () 
		{
			ci = DependencyService.Get<ILocalize>().GetCurrentCultureInfo ();
		}

		public static string GetString(string key, string comment)
		{
			ResourceManager temp = new ResourceManager ("Aura.AppResources", typeof(Localize).GetTypeInfo ().Assembly);

			string result = temp.GetString (key, ci);

			return result; 
		}
	}
}

