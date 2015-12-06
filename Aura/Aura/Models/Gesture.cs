using System;
using System.ComponentModel;
using Newtonsoft.Json;
using XLabs;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;
using System.Threading.Tasks;
using Xamarin.Forms;
using Humanizer;

namespace Aura
{
	public class Gesture: INotifyPropertyChanged
	{
		private string orientation;
		private string orientatione;
		private string accelaration;
		private string pose;
		private string emg;
		private string gyro;

		[JsonProperty(PropertyName = "id")]
		public string ID { get; set; }

		[JsonProperty(PropertyName = "user")]
		public string User {get;set;}

		[JsonProperty(PropertyName = "sign")]
		public string Sign { get; set; }

		[JsonProperty(PropertyName = "word")]
		public string Word { get; set; }

		[JsonProperty(PropertyName = "dialect")]
		public string Dialect { get; set; }

		[JsonProperty(PropertyName = "accelaration")]
		public string Accelaration {
			get{ return this.accelaration; }  
			set {
				if (this.accelaration != value) {
					;
					this.accelaration = value;
					OnPropertyChanged ("Accelaration");

				}
			}
		}

		[JsonProperty(PropertyName = "orientation")]
		public string Orientation {
			get{ return this.orientation; }  
			set {
				if (this.orientation != value) {
					this.orientation = value;
					OnPropertyChanged ("Orientation");

				}
			}
		}

		[JsonProperty(PropertyName = "orientatione")]
		public string OrientationE {
			get{ return this.orientatione; }  
			set {
				if (this.orientatione != value) {
					this.orientatione = value;
					OnPropertyChanged ("OrientationE");

				}
			}
		}

		[JsonProperty(PropertyName = "gyro")]
		public string Gyro {
			get{ return this.gyro; }  
			set {
				if (this.gyro != value) {
					this.gyro = value;
					OnPropertyChanged ("Gyro");

				}
			}
		}

		[JsonProperty(PropertyName = "emg")]
		public string EMG {
			get{ return this.emg; }  
			set {
				if (this.emg != value) {
					;
					this.emg = value;
					OnPropertyChanged ("EMG");

				}
			}
		}

		[JsonProperty(PropertyName = "pose")]
		public string Pose {
			get{ return this.pose; }  
			set {
				if (this.pose != value) {
					;
					this.pose = value;
					OnPropertyChanged ("Pose");

				}
			}
		}
			
		public void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged == null)
				return;

			PropertyChanged (this, new PropertyChangedEventArgs (propertyName));
		}

		[JsonIgnore]
		private Command moreCommand;
		[JsonIgnore]
		public Command MoreCommand {
			get {

				return moreCommand ?? (moreCommand = new Command (
					() => More (),
					() => true)); 
			}
		}

		private async Task More()
		{
			Debug.WriteLine ("is Triggered");
			string userChoice;
			userChoice = await App.Instance.MainPage.DisplayActionSheet (AppResources.SettingMsgLabel, AppResources.CancelBtnLabel, null, AppResources.DeleteBtnLabel);

			if (userChoice == AppResources.DeleteBtnLabel)
				await Delete ();
		}

		private async Task Delete()
		{
			Debug.WriteLine ("is Triggered");
			var answer = await App.Instance.MainPage.DisplayAlert (AppResources.DeleteVideoLabel, AppResources.DeleteVideoMsgLabel, AppResources.YesBtnLabel, AppResources.NoBtnLabel);
			if (answer) {
				App.GestureServer.DeleteTaskAsync (this);
				App.AuraVM.GesturesVM.GestureList.Remove (this);
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

	}
}

