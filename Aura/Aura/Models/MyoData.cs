using System;
using System.ComponentModel;
using Newtonsoft.Json;
using XLabs;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;

namespace Aura
{
	public class MyoData: INotifyPropertyChanged
	{
		private string orientation;
		private Vector3 accelaration;
		private string pose;
		private string emg;
		private string status;


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
		[JsonProperty(PropertyName = "accelaration")]
		public Vector3 Accelaration {
			get{ return this.accelaration; }  
			set {
				if (this.accelaration != value) {
					;
					this.accelaration = value;
					OnPropertyChanged ("Accelaration");

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

		[JsonProperty(PropertyName = "status")]
		public string Status {
			get{ return this.status; }  
			set {
				if (this.status != value) {
					;
					this.status = value;
					OnPropertyChanged ("Status");

				}
			}
		}


		public void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged == null)
				return;

			PropertyChanged (this, new PropertyChangedEventArgs (propertyName));
		}

		private PlotModel accelarationModel;
		public PlotModel AccelarationModel
		{
			get
			{
				return accelarationModel;
			}
			set
			{
				accelarationModel = value;
				OnPropertyChanged("AccelarationModel");
			}
		}

		private PlotModel emgModel;
		public PlotModel EMGModel
		{
			get
			{
				return emgModel;
			}
			set
			{
				emgModel = value;
				OnPropertyChanged("EMGModel");
			}
		}

		public void InitializeAccelarationModel()
		{
			var myModel = new PlotModel { Title = "Accelaration" }; 
			for (int i = 0; i < 4; i++)
				myModel.Series.Add (new LineSeries (){ ItemsSource = new  List<DataPoint> () });

			myModel.Axes.Add (new LinearAxis () { Position = AxisPosition.Bottom,
				Minimum = 0,
				Maximum = 400
			});
			myModel.Axes.Add (new LinearAxis () { Position = AxisPosition.Left,
				Minimum = -2.5,
				Maximum = 2.5
			});

			App.MyoDataStream.AccelarationModel = myModel;
		}

		public void InitializeEMGModel()
		{
			var myModel = new PlotModel { Title = "EMG" }; 
			for (int i = 0; i < 8; i++)
				myModel.Series.Add (new LineSeries (){ ItemsSource = new  List<DataPoint> () });

			myModel.Axes.Add (new LinearAxis () { Position = AxisPosition.Bottom,
				Minimum = 0,
				Maximum = 400
			});
			myModel.Axes.Add (new LinearAxis () { Position = AxisPosition.Left,
				Minimum = -2.5,
				Maximum = 2.5
			});

			App.MyoDataStream.EMGModel = myModel;
		}

		public event PropertyChangedEventHandler PropertyChanged;

	}
}

