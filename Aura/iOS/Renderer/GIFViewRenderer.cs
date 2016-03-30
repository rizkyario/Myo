using System;
using Xamarin.Forms.Platform.iOS;
using System.Drawing;
using Xamarin.Forms;
using Aura;
using Aura.iOS;

using UIKit;
using AVFoundation;
using Foundation;
using CoreGraphics;
using CoreMedia;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Net.Http;
using System.IO;
using ObjCRuntime;
using MediaPlayer;
using ImageIO;
using CoreAnimation;

[assembly: ExportRenderer (typeof (VideoCustomView), typeof (GIFViewRenderer))]
namespace Aura.iOS
{
	public class GIFViewRenderer : ViewRenderer<VideoCustomView,UIView>
	{
		CGImageSource sourceRef;

		protected async override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged (sender, e);
			VideoCustomView video = (VideoCustomView)sender;


			if (e.PropertyName == VisualElement.IsVisibleProperty.PropertyName && video.IsVisible && video.GIFSource!="" && sourceRef==null) {
				Task.Run(() => {
					try {
						CGImageOptions options = new CGImageOptions () { ShouldCache = true };
						sourceRef = CGImageSource.FromUrl(NSUrl.FromString(video.GIFSource.Replace("http://isaratv.azurewebsites.net/isarathumb/","https://isaramobile.blob.core.windows.net/gif/").Replace(".jpg","_NHQ.gif")), options);
						
							Device.BeginInvokeOnMainThread(async () =>
								{
								if(sourceRef!=null){
									if(sourceRef.ImageCount>0){
									var vp = CreateAnimatedImageView (sourceRef); 
									SetNativeControl(vp);
									}
								}
								});

					} catch (Exception ex) {
						Debug.WriteLine (ex);
					}
				});
			}
		}

		protected override void OnElementChanged(ElementChangedEventArgs<VideoCustomView> e)
		{
			base.OnElementChanged (e);

			if (e.OldElement != null || Element == null)
				return;
			try {
				SetNativeControl (new UIImageView());
			} catch (Exception ex) {
				Debug.WriteLine (ex);
			}
		}

		protected override void Dispose (bool disposing)
		{
			//			base.Dispose (disposing);
		}

		private static UIImageView CreateAnimatedImageView(CGImageSource imageSource)
		{
			UIImageView imageView = null;
			var frameCount = imageSource.ImageCount;

			var frameImages = new List<NSObject>((int)frameCount);
			var frameCGImages = new List<CGImage>((int)frameCount);
			var frameDurations = new List<double>((int)frameCount);

			var totalFrameDuration = 0.0;

			for (int i = 0; i < frameCount; i++)
			{
				var frameImage = imageSource.CreateImage(i, null);

				frameCGImages.Add(frameImage);
				frameImages.Add(NSObject.FromObject(frameImage));

				var properties = imageSource.GetProperties(i, null);
				var duration = properties.Dictionary["{GIF}"];
				var delayTime = duration.ValueForKey(new NSString("DelayTime"));
				duration.Dispose ();
				var realDuration = double.Parse(delayTime.ToString());
				frameDurations.Add(realDuration);
				totalFrameDuration += realDuration;
				frameImage.Dispose ();
			}

			var framePercentageDurations = new List<NSNumber>((int)frameCount);
			var framePercentageDurationsDouble = new List<double>((int)frameCount);
			NSNumber currentDurationPercentage = 0.0f;
			double currentDurationDouble = 0.0f;
			for (int i = 0; i < frameCount; i++)
			{
				if (i != 0)
				{
					var previousDuration = frameDurations[i - 1];
					var previousDurationPercentage = framePercentageDurationsDouble[i - 1];

					var number = previousDurationPercentage + (previousDuration/totalFrameDuration);
					currentDurationDouble = number;
					currentDurationPercentage = new NSNumber(number);
				}
				framePercentageDurationsDouble.Add(currentDurationDouble);
				framePercentageDurations.Add(currentDurationPercentage);
			}

//			var imageSourceProperties = imageSource.GetProperties(null);
//			var imageSourceGIFProperties = imageSourceProperties.Dictionary["{GIF}"];
//			var loopCount = imageSourceGIFProperties.ValueForKey(new NSString("LoopCount"));
//			var imageSourceLoopCount = float.Parse(loopCount.ToString());
			var frameAnimation = new CAKeyFrameAnimation();
			frameAnimation.KeyPath = "contents";
//			if (imageSourceLoopCount <= 0.0f)
//			{
				frameAnimation.RepeatCount = float.MaxValue;
//			}
//			else
//			{
//				frameAnimation.RepeatCount = imageSourceLoopCount;
//			}

//			imageSourceGIFProperties.Dispose ();


			frameAnimation.CalculationMode = CAAnimation.AnimationDescrete;
			frameAnimation.Values = frameImages.ToArray();
			frameAnimation.Duration = totalFrameDuration;
			frameAnimation.KeyTimes = framePercentageDurations.ToArray();
			frameAnimation.RemovedOnCompletion = false;
			var firstFrame = frameCGImages[0];
			if(imageView == null)
				imageView = new UIImageView(new CGRect(0.0f, 0.0f, firstFrame.Width, firstFrame.Height));
			else
				imageView.Layer.RemoveAllAnimations();

			imageView.Layer.AddAnimation(frameAnimation, "contents");
			imageView.Layer.ContentsGravity = CALayer.GravityResizeAspect;
			frameAnimation.Dispose ();
			return imageView;
		}
	}
		
}
