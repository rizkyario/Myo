using System;
using Xamarin.Forms;
using Aura;
using Aura.iOS;
using Xamarin.Forms.Platform.iOS;
using System.Diagnostics;
using UIKit;
using Foundation;

[assembly:ExportRendererAttribute(typeof(TabbedPageCustom), typeof(TabbedPageCustomRenderer))]
namespace Aura.iOS
{

	public class TabbedPageCustomRenderer: TabbedRenderer
	{
		public override void ViewWillAppear (bool animated)
		{
			HideBar ();

			base.ViewWillAppear (animated);
		}

		public override void ViewDidAppear (bool animated)
		{
			HideBar ();

			base.ViewDidAppear (animated);

		}

		public void HideBar()
		{
			try{
				TabBar.Hidden = true;
				TabBar.Frame = new CoreGraphics.CGRect (0,0,0,0);}
			catch{
			}
		}

		protected override void Dispose (bool disposing)
		{
			base.Dispose (disposing);
		}

	}
}

