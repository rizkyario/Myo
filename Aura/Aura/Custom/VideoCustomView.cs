using System;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace Aura
{
	public class VideoCustomView : View
	{
		public VideoCustomView ()
		{
		}

		public static readonly BindableProperty IsFitProperty = 
			BindableProperty.Create<VideoCustomView,bool>(
				p => p.IsFit,false);

		public bool IsFit {
			get { return (bool)GetValue (IsFitProperty); }
			set { SetValue (IsFitProperty, value); }
		}

		public static readonly BindableProperty FileSourceProperty = 
			BindableProperty.Create<VideoCustomView,string>(
				p => p.FileSource,string.Empty);

		public string FileSource {
			get { return (string)GetValue (FileSourceProperty); }
			set { SetValue (FileSourceProperty, value); }
		}

		public static readonly BindableProperty GIFSourceProperty = 
			BindableProperty.Create<VideoCustomView,string>(
				p => p.GIFSource,string.Empty);

		public string GIFSource {
			get { return (string)GetValue (GIFSourceProperty); }
			set { SetValue (GIFSourceProperty, value); }
		}

		public static readonly BindableProperty IsPlayingProperty = 
			BindableProperty.Create<VideoCustomView,bool> (
				p => p.IsPlaying, false);

		public bool IsPlaying {
			get { return (bool)GetValue (IsPlayingProperty); }
			set { SetValue (IsPlayingProperty, value); }
		}
	}
}

