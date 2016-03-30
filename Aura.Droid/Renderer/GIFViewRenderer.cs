using System.Drawing;
using Aura;
using Aura.Droid;
using Xamarin.Forms.Platform.Android;
using System.ComponentModel;
using System;
using Xamarin.Forms;
using Android.Graphics;
using System.Threading.Tasks;
using Android.Content;
using Android.Util;
using System.IO;
using Android.Widget;
using System.Net.Http;

[assembly: Xamarin.Forms.ExportRenderer (typeof (VideoCustomView), typeof (GIFViewRenderer))]
namespace Aura.Droid
{
	public class GIFViewRenderer : ViewRenderer<VideoCustomView, Android.Views.View>
	{
		protected override void OnElementChanged (ElementChangedEventArgs<VideoCustomView> e)
		{
			if (e.OldElement != null || Element == null)
				return;
			try {	
				VideoCustomView video = (VideoCustomView)Element;

				if (video.IsVisible && (video.GIFSource != "" || video.FileSource != "")) {
					try {
						Android.Widget.RelativeLayout relativeLayout = new Android.Widget.RelativeLayout (Context);
						relativeLayout.LayoutParameters = new Android.Views.ViewGroup.LayoutParams (Android.Views.ViewGroup.LayoutParams.WrapContent, Android.Views.ViewGroup.LayoutParams.MatchParent);

						var ProgressLayoutParams = new Android.Widget.RelativeLayout.LayoutParams (
							                           LayoutParams.WrapContent, LayoutParams.WrapContent);
						ProgressLayoutParams.AddRule (LayoutRules.CenterInParent);

						Android.Widget.ProgressBar progressBar = new Android.Widget.ProgressBar (Context);
						progressBar.LayoutParameters = ProgressLayoutParams;

						relativeLayout.AddView (progressBar);
						base.SetNativeControl (relativeLayout);

						Task.Run (async () => {
							try {
								var GIFPlayer = new AnimatedImageView (Context);

								if (video.GIFSource == "")
									video.GIFSource = "http://isaratv.azurewebsites.net/isarathumb/" + video.FileSource.Substring (video.FileSource.LastIndexOf ('/') + 1, (video.FileSource.IndexOf ("mp4") - video.FileSource.LastIndexOf ('/') + 2)).Replace (' ', '-').Replace (".mp4", ".gif");
								else
									video.GIFSource = video.GIFSource.Replace (".jpg", "_NHQ.gif");

								string uri = video.GIFSource.Replace (" ", "%20");
								uri = uri.Replace ("(", "%28");
								uri = uri.Replace (")", "%29");

								uri = uri.Replace ("http://isaratv.azurewebsites.net/isarathumb/", "https://isaramobile.blob.core.windows.net/gif/");

								var httpClient = new HttpClient ();
								Stream stream =	await httpClient.GetStreamAsync (uri);
								await GIFPlayer.Initialize (stream);

								var VideoPlayerLayoutParams = new Android.Widget.RelativeLayout.LayoutParams (
									                             LayoutParams.MatchParent, LayoutParams.MatchParent);
								VideoPlayerLayoutParams.AddRule (LayoutRules.CenterInParent);

								GIFPlayer.LayoutParameters = VideoPlayerLayoutParams;

								Device.BeginInvokeOnMainThread (async () => {
									if (((Android.Widget.RelativeLayout)Control) != null)
										((Android.Widget.RelativeLayout)Control).AddView (GIFPlayer);
								});
							} catch (Exception ex) {
								Aura.Debug.WriteLine (ex.Message);
							}
						});

					} catch (Exception ex) {
						Aura.Debug.WriteLine (ex.Message);
					}
				} else
					base.SetNativeControl (new Android.Views.View (Context));
			} catch (Exception ex) {
				Aura.Debug.WriteLine (ex);
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged (sender, e);
			VideoCustomView video = (VideoCustomView)sender;

			if (e.PropertyName == VisualElement.IsVisibleProperty.PropertyName && video.IsVisible && (video.GIFSource!="" || video.FileSource!="")) {
				try {
					Android.Widget.RelativeLayout relativeLayout = new Android.Widget.RelativeLayout(Context);
					relativeLayout.LayoutParameters = new Android.Views.ViewGroup.LayoutParams(Android.Views.ViewGroup.LayoutParams.WrapContent, Android.Views.ViewGroup.LayoutParams.MatchParent);

					var ProgressLayoutParams = new Android.Widget.RelativeLayout.LayoutParams(
						LayoutParams.WrapContent, LayoutParams.WrapContent);
					ProgressLayoutParams.AddRule(LayoutRules.CenterInParent);

					Android.Widget.ProgressBar progressBar = new Android.Widget.ProgressBar(Context);
					progressBar.LayoutParameters = ProgressLayoutParams;

					relativeLayout.AddView(progressBar);
					base.SetNativeControl (relativeLayout);

					Task.Run(async ()=>{
						try {
							var GIFPlayer = new AnimatedImageView (Context);

							if (video.GIFSource == "")
								video.GIFSource = "http://isaratv.azurewebsites.net/isarathumb/" + video.FileSource.Substring (video.FileSource.LastIndexOf ('/') + 1, (video.FileSource.IndexOf ("mp4") - video.FileSource.LastIndexOf ('/') + 2)).Replace (' ', '-').Replace (".mp4", ".gif");
							else
								video.GIFSource = video.GIFSource.Replace (".jpg", "_NHQ.gif");

							string uri = video.GIFSource.Replace(" ", "%20");
							uri = uri.Replace("http://isaratv.azurewebsites.net/isarathumb/","https://isaramobile.blob.core.windows.net/gif/");
							uri = uri.Replace ("(", "%28");
							uri = uri.Replace (")", "%29");

							var httpClient = new HttpClient();
							Stream stream =	await httpClient.GetStreamAsync(uri);
							await GIFPlayer.Initialize(stream);

							var VideoPlayerLayoutParams = new Android.Widget.RelativeLayout.LayoutParams(
								LayoutParams.MatchParent, LayoutParams.MatchParent);
							VideoPlayerLayoutParams.AddRule(LayoutRules.CenterInParent);

							GIFPlayer.LayoutParameters = VideoPlayerLayoutParams;

							Device.BeginInvokeOnMainThread(async () =>
								{
									((Android.Widget.RelativeLayout)Control).AddView(GIFPlayer);
								});
						} catch (Exception ex) {
							Aura.Debug.WriteLine (ex.Message);
						}
					});

				} catch (Exception ex) {
					Aura.Debug.WriteLine (ex.Message);
				}
			}
		}
	}

	public class AnimatedImageView : Android.Views.View
	{
		private Movie movie;
		private long movieStart;

		public AnimatedImageView (Context context) : base (context)
		{
		}

		public AnimatedImageView(Context context, IAttributeSet attrs) :
		base(context, attrs)
		{
		}

		public AnimatedImageView(Context context, IAttributeSet attrs, int defStyle) :
		base(context, attrs, defStyle)
		{
		}

		public async static Task<byte[]> ReadFully(System.IO.Stream input)
		{
			return await Task.Run (() => {
				var buffer = new byte[16 * 1024];
				using (var ms = new MemoryStream ()) {
					int read;
					while ((read = input.Read (buffer, 0, buffer.Length)) > 0) {
						ms.Write (buffer, 0, read);
					}
					return ms.ToArray ();
				}
			});
		}

		public async Task Initialize(System.IO.Stream input)
		{
			Focusable = true;

			try
			{
				if (false)
				{
					movie = Movie.DecodeStream(input);
					movieStart = 0;
				}
				else
				{
					var array = await ReadFully(input);
					movie = Movie.DecodeByteArray(array, 0, array.Length);
					var duration = movie.Duration();
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine (ex.Message);
			}

		}

		private bool playing = true;
		public void Start()
		{
			playing = true;
			this.Invalidate();
		}

		public void Stop()
		{
			playing = false;
		}

		protected override void OnDraw(Canvas canvas)
		{
			canvas.DrawColor(Android.Graphics.Color.Transparent);

			Paint p = new Paint(); 
			p.AntiAlias = true; 
//			SetLayerType(Android.Views.LayerType.Software, p);
			long now = Android.OS.SystemClock.UptimeMillis();
			if (movieStart == 0)
			{   // first time
				movieStart = now;
			}
			if (movie != null) {
				int dur = movie.Duration ();
				if (dur == 0) {
					dur = 1000;
				}
				var relTime = (int)((now - movieStart) % dur);
				movie.SetTime (relTime);
				var movieWidth = (float)movie.Width ();
				var movieHeight = (float)movie.Height ();
				var scale = 1.0f;
				if (movieWidth > movieHeight) {
					scale = this.Width / movieWidth;
					if (scale * movieHeight > Height)
						scale = Height / movieHeight;
				} else {
					scale = this.Height / movieHeight;
					if (scale * movieWidth > Width)
						scale = Height / movieWidth;
				}
				canvas.Scale (scale, scale);
				try {
					movie.Draw(canvas,(Width/2) - ((scale * movieWidth)/2), (Height/scale/2) - ((movieHeight)/2), p);
				} catch (Exception ex) {
					Debug.WriteLine (ex.Message);
				}

				if (playing)
					Invalidate ();
			}
		}
	}

}

