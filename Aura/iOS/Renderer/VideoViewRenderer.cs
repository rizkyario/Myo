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

//[assembly: ExportRenderer (typeof (VideoCustomView), typeof (VideoViewRenderer))]
namespace Aura.iOS
{
	public class VideoViewRenderer : ViewRenderer<VideoCustomView,UIView>
	{
		protected override  void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged (sender, e);
			VideoCustomView video = (VideoCustomView)sender;


			if (e.PropertyName == VisualElement.IsVisibleProperty.PropertyName && video.IsVisible && video.FileSource!="") {
				try {

					var vp = new UIVideoView (video.FileSource, new CGRect (0, 0, (nfloat)video.HeightRequest, (nfloat)video.WidthRequest), video.IsFit); 
					SetNativeControl (vp);

				} catch (Exception ex) {
					Debug.WriteLine (ex);
				}
			}
		}

		protected override void OnElementChanged(ElementChangedEventArgs<VideoCustomView> e)
		{
			base.OnElementChanged (e);

			if (e.OldElement != null || Element == null)
				return;
			try {
				SetNativeControl (new UIVideoView ());
			} catch (Exception ex) {
				Debug.WriteLine (ex);
			}
		}

		protected override void Dispose (bool disposing)
		{
//			base.Dispose (disposing);
		}

			
	}
		
	public class UIVideoView : UIView
	{
		AVPlayer _player;
		AVPlayerLayer _playerLayer;
		AVAsset _asset;
		AVPlayerItem _playerItem;
		UIActivityIndicatorView activityIndicator;
		UIView viewContainer;

		bool isAspectFit;
		public string Source;

		public UIVideoView ()
		{
		}
		public UIVideoView (string FileSource, CGRect frame, bool isFit)
		{
			Source = FileSource;
			var uri = new Uri (Source);
			var nsurl = new NSUrl (uri.GetComponents (UriComponents.HttpRequestUrl, UriFormat.UriEscaped));
			_asset = AVAsset.FromUrl (nsurl);

			_playerItem = new AVPlayerItem (_asset);
			_player = new AVPlayer (_playerItem);
			_playerLayer = AVPlayerLayer.FromPlayer (_player);
			_playerLayer.Hidden = true;
			_player.Volume = 0;
			_playerLayer.Frame = frame;
			_playerLayer.Hidden = true;

			if (isFit) {
				isAspectFit = true;
				_playerLayer.VideoGravity = AVLayerVideoGravity.ResizeAspectFill;
			} else {
				isAspectFit = false;
				_playerLayer.VideoGravity = AVLayerVideoGravity.ResizeAspect;
			}


			viewContainer = new UIView ();
			viewContainer.Frame = frame;
			viewContainer.Layer.AddSublayer (_playerLayer);

			activityIndicator = new UIActivityIndicatorView ();
			activityIndicator.Center = viewContainer.Center;
			activityIndicator.StartAnimating ();
			activityIndicator.Color = UIColor.White;

			viewContainer.AddSubview (activityIndicator);

			_playerItem.AddObserver (this, new NSString("playbackLikelyToKeepUp"), NSKeyValueObservingOptions.New, IntPtr.Zero);

			Action singleTap = () => { 
				// Toggle the image
				if (activityIndicator.IsAnimating == false) {
					_player.Seek(CMTime.Zero);
					_playerLayer.Hidden = false;
					_player.Play ();
				}
			};
			Action doubleTap = () => { 
				if (isAspectFit) {
					isAspectFit = false;
					_playerLayer.VideoGravity = AVLayerVideoGravity.ResizeAspect;
				} else {
					isAspectFit = true;
					_playerLayer.VideoGravity = AVLayerVideoGravity.ResizeAspectFill;
				}
			};

			UITapGestureRecognizer tapGesture  = new UITapGestureRecognizer(doubleTap);
			tapGesture.NumberOfTapsRequired = 2;

			this.AddGestureRecognizer( tapGesture);
			this.AddGestureRecognizer( new UITapGestureRecognizer(singleTap));
			Add(viewContainer);

		}


		public override void ObserveValue(NSString keyPath, NSObject ofObject, NSDictionary change, IntPtr context)
		{
			if (_player == null) {
				return;
			} else if (_playerItem != null && keyPath.IsEqual (new NSString ("playbackLikelyToKeepUp"))) {
				if (_playerItem.PlaybackLikelyToKeepUp) {
					activityIndicator.StopAnimating ();
					_playerLayer.Hidden = false;
					_player.Play ();
				}
			}
		}

		protected override void Dispose (bool disposing)
		{
			base.Dispose (disposing);

			_playerItem.RemoveObserver (this, new NSString ("playbackLikelyToKeepUp"));

		}
	}
}
