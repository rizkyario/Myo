using System;
using Xamarin.Forms;
using Aura;
using Xamarin.Forms.Platform.Android;
using Aura.Droid;
using Android.Graphics;

//[assembly: ExportRenderer(typeof(ImageCircle), typeof(ImageCircleRenderer))]
namespace Aura.Droid
{
	public class ImageCircleRenderer: ImageRenderer
	{
		public static void Init() { }

		protected override void OnElementChanged(ElementChangedEventArgs<Image> e)
		{
			base.OnElementChanged(e);

			if (e.OldElement == null)
			{
				//Only enable hardware accelleration on lollipop
				if ((int)Android.OS.Build.VERSION.SdkInt < 21)
				{
					SetLayerType(Android.Views.LayerType.Software, null);
				}

			}
		}

		protected override bool DrawChild(Canvas canvas, global::Android.Views.View child, long drawingTime)
		{
			try
			{
				var radius = Math.Min(Width, Height) / 2;
				var strokeWidth = 0;
				radius -= strokeWidth / 2;
				//Create path to clip
				var path = new Path();
				path.AddCircle(Width / 2, Height / 2, radius, Path.Direction.Ccw);
				canvas.Save();
				canvas.ClipPath(path);
				var result = base.DrawChild(canvas, child, drawingTime);
				canvas.Restore();
				//Properly dispose
				path.Dispose();
				return result;
			}
			catch (Exception ex)
			{
				Console.WriteLine("Unable to create circle image: " + ex);
			}
			return base.DrawChild(canvas, child, drawingTime);
		}
	}
}

