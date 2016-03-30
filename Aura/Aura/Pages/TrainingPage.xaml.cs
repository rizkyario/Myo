using System;
using System.Collections.Generic;

using Xamarin.Forms;
using OxyPlot.Series;
using OxyPlot;
using Newtonsoft.Json;
using Acr.UserDialogs;
using Accord.Neuro.Networks;
using System.Linq;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Accord.Neuro.ActivationFunctions;
using Accord.Neuro.Learning;
using Aura.Helpers;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Aura
{
	public partial class TrainingPage : TabbedPageCustom
	{
		DeepBeliefNetwork deepLearning;

		public TrainingPage (Sign sign)
		{
			NavigationPage.SetHasNavigationBar (this, false);
			InitializeComponent ();
			App.AuraVM.SelectedSign = sign;

			App.AuraVM.GesturesVM.GetGestures (sign);
			GestureCount.BindingContext = App.AuraVM.GesturesVM;
			GestureCount.SetBinding (Label.TextProperty, "GestureList.Count", 0, null, "{0} Gesture(s)");

			AccelarationPlot.Model = null;
			App.AuraVM.InitializeAccelarationModel ();
			AccelarationPlot.Model = App.AuraVM.RecordedAccelarationModel;
			BindingContext = App.AuraVM;

			TabPanel.Opacity = 0;
			TabPanel.TranslateTo (0, 50, 0, Easing.Linear);

			inter = 0;
			sampling_number = 50;


			try{
				deepLearning = (DeepBeliefNetwork)DependencyService.Get<ISaveAndLoad>().LoadObject(App.AuraVM.SelectedSign.Word+".ann");
			}
			catch(Exception ex){
			}
//			if (Settings.DefaultDeepLearning != null)
//				deepLearning = Settings.DefaultDeepLearning;
		}
	

		protected override async void OnAppearing ()
		{
			base.OnAppearing ();


			TabPanel.FadeTo (1, 500, Easing.Linear);
			TabPanel.TranslateTo (0,0, 500, Easing.CubicOut);
		}

		public void OnConnectButtonClicked (object o, EventArgs e)
		{
			DependencyService.Get<IMyoSensor> ().Connect();
		}


		public async void OnXBtnClicked (object o, EventArgs e)
		{
			App.AuraVM.InitializeAccelarationModel ();
			await this.Navigation.PopAsync ();
		}

		public async void OnSyncBtnClicked (object o, EventArgs e)
		{
			UserDialogs.Instance.ShowLoading ("Syncing");

			await App.GestureServer.Sync ();
//			await App.AuraVM.GesturesVM.QuickRefreshCommand (App.AuraVM.SelectedSign);
			UserDialogs.Instance.HideLoading ();
		}

		public async void OnTrainButtonClicked (object o, EventArgs e)
		{
			Task.Run (() => {
				SearchSolution ();
			});
		}

		public async void OnDetectButtonClicked (object o, EventArgs e)
		{
			Task.Run (() => {
				DetectGesture ();
			});
		}


		List<string> outputType;
		double error;
		int sampling_number;
		int inter;
		double[] o;
		public async void DetectGesture()
		{


			ObservableCollection<Gesture> ControlList = await App.GestureServer.GetGesturesByWordCache ("Maaf");
			ControlList = new ObservableCollection<Gesture> (ControlList.Concat (await App.GestureServer.GetGesturesByWordCache ("Mabuk")));
			ControlList = new ObservableCollection<Gesture> (ControlList.Concat (await App.GestureServer.GetGesturesByWordCache ("Nakal")));
			ObservableCollection<Gesture> AllGestures = new ObservableCollection<Gesture> (ControlList.Concat (App.AuraVM.GesturesVM.GestureList));

			//			ObservableCollection<Gesture> AllGestures = await App.GestureServer.GetGesturesAllCache ();

			outputType = new List<string> ();
			foreach (Gesture g in AllGestures) {
				if (!outputType.Contains (g.Word))
					outputType.Add (g.Word);
			}


			List<double[]> acc = new List<double[]> ();

			List<DataPoint> x = (((LineSeries)App.AuraVM.RecordedAccelarationModel.Series [0]).ItemsSource as List<DataPoint>);
			List<DataPoint> y = (((LineSeries)App.AuraVM.RecordedAccelarationModel.Series [1]).ItemsSource as List<DataPoint>);
			List<DataPoint> z = (((LineSeries)App.AuraVM.RecordedAccelarationModel.Series [2]).ItemsSource as List<DataPoint>);

			if (x.Count > sampling_number) {
				List<Double> xd = new List<double> ();
				List<Double> yd = new List<double> ();
				List<Double> zd = new List<double> ();

				foreach (DataPoint dp in x) {
					xd.Add (dp.Y);
				}

				foreach (DataPoint dp in y) {
					yd.Add (dp.Y);
				}

				foreach (DataPoint dp in z) {
					zd.Add (dp.Y);
				}

				acc.Add (xd.ToArray ());
				acc.Add (yd.ToArray ());
				acc.Add (zd.ToArray ());

				Gesture gesture = new Gesture () {
					Word = App.AuraVM.SelectedSign.Word,
					Sign = App.AuraVM.SelectedSign.ID,
					User = App.AuraVM.CurrentUser.ID,
					Accelaration = JsonConvert.SerializeObject (acc.ToArray ()),
				};

				List<double[][]> preP = await PreProcess (new ObservableCollection<Gesture> (){ gesture });

				double[][] input = preP [0];
				double[][] output = preP [1];

				o = deepLearning.Compute (input [0]);
				int indexResult = (o.ToList ()).IndexOf (o.Max ());

				Device.BeginInvokeOnMainThread (() => {
					resultLabel.Text = outputType [indexResult] + "(" + o.Max () + ")";
				});
			} else {
				Device.BeginInvokeOnMainThread (() => {
					resultLabel.Text = "Slowdown";
				});
			}

		}

		public async void SearchSolution()
		{
			ObservableCollection<Gesture> ControlList = await App.GestureServer.GetGesturesByWordCache ("Maaf");
			ControlList = new ObservableCollection<Gesture> (ControlList.Concat (await App.GestureServer.GetGesturesByWordCache ("Mabuk")));
			ControlList = new ObservableCollection<Gesture> (ControlList.Concat (await App.GestureServer.GetGesturesByWordCache ("Nakal")));
			ObservableCollection<Gesture> AllGestures = new ObservableCollection<Gesture> (ControlList.Concat (App.AuraVM.GesturesVM.GestureList));

//			ObservableCollection<Gesture> AllGestures = await App.GestureServer.GetGesturesAllCache ();

			outputType = new List<string> ();
			foreach (Gesture g in AllGestures) {
				if (!outputType.Contains (g.Word))
					outputType.Add (g.Word);
			}

			List<double[][]> preP = await PreProcess (AllGestures);

			double[][] input = preP [0];
			double[][] output = preP [1];


			int layer = 0;
			int.TryParse (layerLabel.Text,out layer);

			deepLearning = new DeepBeliefNetwork (new BernoulliFunction (), input [0].Length, layer, 30);
			deepLearning.Push (outputType.Count, 
				visibleFunction: new GaussianFunction (),
				hiddenFunction: new BernoulliFunction ());

			BackPropagationLearning teacher = new BackPropagationLearning (deepLearning) {
				LearningRate = 0.1,
				Momentum = 0.9
			};

			int i = 0;
			int.TryParse (iterationLabel.Text,out inter);

			while (true) {
				error = teacher.RunEpoch (input, output) / AllGestures.Count;
				Debug.WriteLine (error.ToString ());
				if (i > inter)
					break;

				Device.BeginInvokeOnMainThread (() => {
					errorLabel.Text = error.ToString ().Substring (0, 5);
				});

				i++;
			}

			DependencyService.Get<ISaveAndLoad> ().SaveObject (App.AuraVM.SelectedSign.Word + ".ann", deepLearning);
//			Settings.DefaultDeepLearning = deepLearning;
		}

		public async Task<List<double[][]>> PreProcess(ObservableCollection<Gesture> AllGestures)
		{
			double[][] input;
			double[][] output;

			List<double[]> inputList = new List<double[]> ();
			List<double[]> outputList = new List<double[]> ();

			foreach (Gesture g in AllGestures) {
				
				List<double[]> acc = JsonConvert.DeserializeObject<List<double[]>> (g.Accelaration);
				int totalLength = acc.First ().Length;

				if (totalLength >= sampling_number) {
					int _1halfcut = (totalLength - sampling_number) / 2;
					int _2halfcut = (totalLength - sampling_number) - _1halfcut;

					double[] samplex = acc [0].Skip (_1halfcut).Take (totalLength - _1halfcut - _2halfcut).ToArray ();
					double[] sampley = acc [1].Skip (_1halfcut).Take (totalLength - _1halfcut - _2halfcut).ToArray ();
					double[] samplez = acc [2].Skip (_1halfcut).Take (totalLength - _1halfcut - _2halfcut).ToArray ();

					double[] sample = samplex.Concat (sampley).ToArray ().Concat (samplez).ToArray ();
					inputList.Add (sample);

					double[] outmatrix = new double[outputType.Count];
					for (int j = 0; j < outputType.Count; j++) {
						if (outputType [j] == g.Word)
							outmatrix [j] = 1;
						else
							outmatrix [j] = 0;
					}

					outputList.Add (outmatrix);
				}
			}

			input = inputList.ToArray ();
			output = outputList.ToArray ();

			return new List<double[][]>(){ input, output };
		}

		public async void OnSaveBtnClicked (object o, EventArgs e)
		{
			List<double[]> acc = new List<double[]> ();

			List<DataPoint> x = (((LineSeries)App.AuraVM.RecordedAccelarationModel.Series [0]).ItemsSource as List<DataPoint>);
			List<DataPoint> y = (((LineSeries)App.AuraVM.RecordedAccelarationModel.Series [1]).ItemsSource as List<DataPoint>);
			List<DataPoint> z = (((LineSeries)App.AuraVM.RecordedAccelarationModel.Series [2]).ItemsSource as List<DataPoint>);

			List<Double> xd = new List<double> ();
			List<Double> yd = new List<double> ();
			List<Double> zd = new List<double> ();

			foreach (DataPoint dp in x) {
				xd.Add (dp.Y);
			}

			foreach (DataPoint dp in y) {
				yd.Add (dp.Y);
			}

			foreach (DataPoint dp in z) {
				zd.Add (dp.Y);
			}

			acc.Add (xd.ToArray ());
			acc.Add (yd.ToArray ());
			acc.Add (zd.ToArray ());

			List<double[]> emg = new List<double[]> ();


			List<DataPoint> g0 = (((LineSeries)App.AuraVM.RecordedEMGModel.Series [0]).ItemsSource as List<DataPoint>);
			List<DataPoint> g1 = (((LineSeries)App.AuraVM.RecordedEMGModel.Series [1]).ItemsSource as List<DataPoint>);
			List<DataPoint> g2 = (((LineSeries)App.AuraVM.RecordedEMGModel.Series [2]).ItemsSource as List<DataPoint>);
			List<DataPoint> g3 = (((LineSeries)App.AuraVM.RecordedEMGModel.Series [3]).ItemsSource as List<DataPoint>);
			List<DataPoint> g4 = (((LineSeries)App.AuraVM.RecordedEMGModel.Series [4]).ItemsSource as List<DataPoint>);
			List<DataPoint> g5 = (((LineSeries)App.AuraVM.RecordedEMGModel.Series [5]).ItemsSource as List<DataPoint>);
			List<DataPoint> g6 = (((LineSeries)App.AuraVM.RecordedEMGModel.Series [6]).ItemsSource as List<DataPoint>);
			List<DataPoint> g7 = (((LineSeries)App.AuraVM.RecordedEMGModel.Series [7]).ItemsSource as List<DataPoint>);

			List<Double> g0d = new List<double> ();
			List<Double> g1d = new List<double> ();
			List<Double> g2d = new List<double> ();
			List<Double> g3d = new List<double> ();
			List<Double> g4d = new List<double> ();
			List<Double> g5d = new List<double> ();
			List<Double> g6d = new List<double> ();
			List<Double> g7d = new List<double> ();


			foreach (DataPoint dp in g0) {
				g0d.Add (dp.Y);
			}
			foreach (DataPoint dp in g1) {
				g1d.Add (dp.Y);
			}
			foreach (DataPoint dp in g2) {
				g2d.Add (dp.Y);
			}
			foreach (DataPoint dp in g3) {
				g3d.Add (dp.Y);
			}
			foreach (DataPoint dp in g4) {
				g4d.Add (dp.Y);
			}
			foreach (DataPoint dp in g5) {
				g5d.Add (dp.Y);
			}
			foreach (DataPoint dp in g6) {
				g6d.Add (dp.Y);
			}
			foreach (DataPoint dp in g7) {
				g7d.Add (dp.Y);
			}

			emg.Add (g0d.ToArray ());
			emg.Add (g1d.ToArray ());
			emg.Add (g2d.ToArray ());
			emg.Add (g3d.ToArray ());
			emg.Add (g4d.ToArray ());
			emg.Add (g5d.ToArray ());
			emg.Add (g6d.ToArray ());
			emg.Add (g7d.ToArray ());

			Gesture g = new Gesture () {
				Word = App.AuraVM.SelectedSign.Word,
				Dialect = App.AuraVM.SelectedSign.Location,
				Sign = App.AuraVM.SelectedSign.ID,
				User = App.AuraVM.CurrentUser.ID,
				Accelaration = JsonConvert.SerializeObject (acc.ToArray ()),
				Gyro = "",
				Pose = JsonConvert.SerializeObject (App.AuraVM.Poses.ToArray ()),
				EMG = JsonConvert.SerializeObject (emg.ToArray ()),
				Orientation = "",
				OrientationE = ""
			};

			UserDialogs.Instance.ShowLoading ("Saving");
			await App.GestureServer.SaveGesture (g);
//			App.AuraVM.GesturesVM.GestureList.Add (g);
			AccelarationPlot.Model = null;
			App.AuraVM.InitializeAccelarationModel ();
			AccelarationPlot.Model = App.AuraVM.RecordedAccelarationModel;
			UserDialogs.Instance.HideLoading ();
				
		}

		public  void OnStopBtnClicked (object o, EventArgs e)
		{
			App.AuraVM.StopGesture ();
		}

		public  void OnRecordBtnClicked (object o, EventArgs e)
		{
			if (App.AuraVM.IsSynced)
				App.AuraVM.RecordGesture ();
			else
				UserDialogs.Instance.AlertAsync ("Calibrate the Sensor first!", "Uncalibrated Sensor", "Ok");
		}

		void OnButtonClicked (object sender, EventArgs e)
		{
			Button b = (Button)sender;
			if (b.CommandParameter.ToString() == "VideoBtn") {
				this.CurrentPage = this.Children [0];
			} else if (b.CommandParameter.ToString() == "GraphBtn") {
				this.CurrentPage = this.Children [1];
			}  else if (b.CommandParameter.ToString() == "GesturesBtn") {
					this.CurrentPage = this.Children [2];
			} else if (b.CommandParameter.ToString() == "SettingsBtn") {
				this.CurrentPage = this.Children [3];
			}
		}

		public void OnPlayClicked(object o, EventArgs e)
		{
			((Sign)((Button)o).CommandParameter).Play();
		}

	}
}

