using System;
using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms;
using System.ComponentModel;
using Aura;
using CoreGraphics;
using Aura.iOS;

[assembly: ExportRenderer(typeof(ImageCircle), typeof(ImageCircleRenderer))]
namespace Aura.iOS
{
	public class ImageCircleRenderer : ImageRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<Image> e)
		{
			base.OnElementChanged(e);

			if (e.OldElement != null || Element == null)
				return;
			try
			{
				double min = Math.Min(Element.Width, Element.Height);
				Control.Layer.CornerRadius = (float)(min / 2.0);
				Control.Layer.MasksToBounds = false;
				Control.ClipsToBounds = true;
			}
			catch (Exception ex)
			{
				Debug.WriteLine (ex);
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == VisualElement.HeightProperty.PropertyName ||
				e.PropertyName == VisualElement.WidthProperty.PropertyName)
			{
				try
				{
					double min = Math.Min(Element.Width, Element.Height);
					Control.Layer.CornerRadius = (float)(min / 2.0);
					Control.Layer.MasksToBounds = false;
					Control.ClipsToBounds = true;
				}
				catch (Exception ex)
				{
					Debug.WriteLine (ex);
				}
			}
		}
	}
}

