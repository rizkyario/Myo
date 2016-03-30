using System;
using Aura;
using Aura.Droid;
using Xamarin.Forms.Platform.Android;
using Android.Widget;
using Android.Graphics;
using Android.Views;
using System.ComponentModel;
using Xamarin.Forms;
using Android.Graphics.Drawables;

[assembly: Xamarin.Forms.ExportRenderer (typeof (Xamarin.Forms.Button), typeof (ButtonCustomRenderer))]
namespace Aura.Droid
{
	public class ButtonCustomRenderer: ButtonRenderer
	{
		protected override void OnElementChanged (ElementChangedEventArgs<Xamarin.Forms.Button> e) {
			base.OnElementChanged (e);
			if (e.OldElement != null || Element == null)
				return;

			((Android.Widget.Button)Control).SetPadding (0, 0, 0, 0);
			((Android.Widget.Button)Control).Typeface = MainActivity.Font;
			((Android.Widget.Button)Control).SetMinimumWidth (0);
			((Android.Widget.Button)Control).SetSingleLine ();
			if (((Xamarin.Forms.Button)e.NewElement).HorizontalOptions.Equals (LayoutOptions.Start))
				((Android.Widget.Button)Control).Gravity = GravityFlags.Left | GravityFlags.CenterVertical;
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged (sender, e);

			if (!((Android.Widget.Button)Control).Typeface.Equals (MainActivity.Font))
				((Android.Widget.Button)Control).Typeface = MainActivity.Font;

			if (((Xamarin.Forms.Button)sender).HorizontalOptions.Equals (LayoutOptions.Start))
				((Android.Widget.Button)Control).Gravity = GravityFlags.Left | GravityFlags.CenterVertical;
			
		}

	}
}

