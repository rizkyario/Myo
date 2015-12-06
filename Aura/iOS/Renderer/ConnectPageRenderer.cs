using System;
using Xamarin.Forms;
using Aura;
using Aura.iOS;
using Xamarin.Forms.Platform.iOS;
using UIKit;
using Xamarin.Auth;
using System.Threading.Tasks;
using System.Net;
using Aura.Helpers;
using Myo;

[assembly: ExportRenderer (typeof (Page), typeof (SettingsPageRenderer))]
namespace Aura.iOS
{
	public class SettingsPageRenderer: PageRenderer
	{
		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
			AppDelegate.MainMenu = this;
		}
			
	}
}

