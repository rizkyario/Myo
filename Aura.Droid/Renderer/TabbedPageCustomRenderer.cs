using System;
using Xamarin.Forms;
using Aura;
using Aura.Droid;
using System.Diagnostics;
using Xamarin.Forms.Platform.Android;
using System.ComponentModel;
using Android.Views;
using Android.App;
using Android.Graphics.Drawables;

[assembly:ExportRendererAttribute(typeof(TabbedPageCustom), typeof(TabbedPageCustomRenderer))]
namespace Aura.Droid
{

	public class TabbedPageCustomRenderer: TabbedRenderer
	{
		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			Activity activity = this.Context as Activity;
			ActionBar actionBar = activity.ActionBar;
			actionBar.RemoveAllTabs ();
			actionBar.Hide ();
			ColorDrawable colorDrawable = new ColorDrawable(global::Android.Graphics.Color.Transparent);
			actionBar.SetStackedBackgroundDrawable(colorDrawable);

			base.OnElementPropertyChanged(sender, e);
		}
	}
}

