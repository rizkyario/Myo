using System;
using Xamarin.Forms;
using System.ComponentModel;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace Aura
{
	public class AuraViewModel: INotifyPropertyChanged
	{
		private User currentUser;
		private string status;
		private Sign selectedSign;

		private bool isRecording;

		public bool IsRecording {
			get { return isRecording; }
			set {
				if (isRecording == value)
					return;

				isRecording = value;
				OnPropertyChanged ("IsRecording");
				OnPropertyChanged ("IsNotRecording");
			}
		}

		private bool isDashboard;
		public bool IsDashboard {
			get { return isDashboard; }
			set {
				if (isDashboard == value)
					return;

				isDashboard = value;
				OnPropertyChanged ("IsDashboard");
			}
		}

		private bool isSynced;
		public bool IsSynced {
			get { return isSynced; }
			set {
				if (isSynced == value)
					return;

				isSynced = value;
				OnPropertyChanged ("IsSynced");
			}
		}

		private bool isTraining;
		public bool IsTraining {
			get { return isTraining; }
			set {
				if (isTraining == value)
					return;

				isTraining = value;
				OnPropertyChanged ("IsTraining");
			}
		}

		private bool isRecognizing;
		public bool IsRecognizing {
			get { return isRecognizing; }
			set {
				if (isRecognizing == value)
					return;

				isRecognizing = value;
				OnPropertyChanged ("IsRecognizing");
			}
		}

		private bool isAutomatic;

		public bool IsAutomatic {
			get { return isAutomatic; }
			set {
				if (isAutomatic == value)
					return;

				isAutomatic = value;
				OnPropertyChanged ("IsAutomatic");
			}
		}

		[JsonIgnore]
		public bool IsNotRecording {
			get{ return !isRecording; }
		} 

		public User CurrentUser {
			get{ return this.currentUser; }  
			set {
				if (this.currentUser != value) {
					this.currentUser = value;
					OnPropertyChanged ("CurrentUser");
				}
			}
		}

		public int CurrentPose;

		public List<int> Poses;

		public Sign SelectedSign {
			get{ return this.selectedSign; }  
			set {
				if (this.selectedSign != value) {
					this.selectedSign = value;
					OnPropertyChanged ("SelectedSign");
				}
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private SignsViewModel signVM;
		public SignsViewModel SignVM {
			get{ return this.signVM; }  
			set {
				if (this.signVM != value) {
					this.signVM = value;
					OnPropertyChanged ("SignVM");
				}
			}
		}

		private GesturesViewModel gesturesVM;
		public GesturesViewModel GesturesVM {
			get{ return this.gesturesVM; }  
			set {
				if (this.gesturesVM != value) {
					this.gesturesVM = value;
					OnPropertyChanged ("GesturesVM");
				}
			}
		}
			
		public AuraViewModel (User user)
		{
			CurrentUser = user;
			IsSynced = false;
			IsAutomatic = true;
			signVM = new SignsViewModel ();
			gesturesVM = new GesturesViewModel ();
		}


		public void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged == null)
				return;

			PropertyChanged (this, new PropertyChangedEventArgs (propertyName));
		}


		private PlotModel accelarationModel;
		public PlotModel AccelarationModel {
			get {
				return accelarationModel;
			}
			set {
				accelarationModel = value;
				OnPropertyChanged ("AccelarationModel");
			}
		}

		private PlotModel emgModel;
		public PlotModel EMGModel {
			get {
				return emgModel;
			}
			set {
				emgModel = value;
				OnPropertyChanged ("EMGModel");
			}
		}

		private PlotModel recordedaccelarationModel;
		public PlotModel RecordedAccelarationModel {
			get {
				return recordedaccelarationModel;
			}
			set {
				recordedaccelarationModel = value;
				OnPropertyChanged ("RecordedAccelarationModel");
			}
		}

		private PlotModel recordedemgModel;
		public PlotModel RecordedEMGModel {
			get {
				return recordedemgModel;
			}
			set {
				recordedemgModel = value;
				OnPropertyChanged ("RecordedEMGModel");
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

			var recordedModel = new PlotModel { Title = "Accelaration" }; 
			for (int i = 0; i < 4; i++)
				recordedModel.Series.Add (new LineSeries (){ ItemsSource = new  List<DataPoint> () });

			recordedModel.Axes.Add (new LinearAxis () { Position = AxisPosition.Bottom,
				Minimum = 0,
				Maximum = 400
			});
			recordedModel.Axes.Add (new LinearAxis () { Position = AxisPosition.Left,
				Minimum = -2.5,
				Maximum = 2.5
			});

			AccelarationModel = myModel;
			RecordedAccelarationModel = recordedModel;
		}

		public void RecordGesture()
		{
			Status = "Recording";
			IsRecording = true;

			//reset recorded data
			for (int i = 0; i < 3; i++)
				(((LineSeries)App.AuraVM.RecordedAccelarationModel.Series [i]).ItemsSource as List<DataPoint>).Clear ();

			for (int i = 0; i < 8; i++)
				(((LineSeries)App.AuraVM.RecordedEMGModel.Series [i]).ItemsSource as List<DataPoint>).Clear ();

			for (int i = 0; i < 3; i++)
				(((LineSeries)App.AuraVM.AccelarationModel.Series [i]).ItemsSource as List<DataPoint>).Clear ();

			for (int i = 0; i < 8; i++)
				(((LineSeries)App.AuraVM.EMGModel.Series [i]).ItemsSource as List<DataPoint>).Clear ();
			

			App.AuraVM.Poses = new List<int> ();
		}

		public void StopGesture()
		{
			Status = "Rest";
			IsRecording = false;
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

			var recordedModel = new PlotModel { Title = "EMG" }; 
			for (int i = 0; i < 8; i++)
				recordedModel.Series.Add (new LineSeries (){ ItemsSource = new  List<DataPoint> () });

			recordedModel.Axes.Add (new LinearAxis () { Position = AxisPosition.Bottom,
				Minimum = 0,
				Maximum = 400
			});
			recordedModel.Axes.Add (new LinearAxis () { Position = AxisPosition.Left,
				Minimum = -2.5,
				Maximum = 2.5
			});

			RecordedEMGModel = recordedModel;
			EMGModel = myModel;
		}
	}
}

