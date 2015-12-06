using System;
using System.Collections.Generic;
using Xamarin.Forms;
using OxyPlot;
using Newtonsoft.Json;
using OxyPlot.Series;
using OxyPlot.Axes;
using Acr.UserDialogs;

namespace Aura
{
	public partial class GestureViewCell : ContentView
	{
		public GestureViewCell ()
		{
			InitializeComponent ();
			this.BindingContextChanged += onBindingContextChanged;
			AccPlot.Model = null;
			EMGPlot.Model = null;
		}

		public void onBindingContextChanged(object sender, EventArgs args)
		{
			if (this.BindingContext != null) {
				Gesture g = (Gesture)this.BindingContext;
				if (g != null) {
					User user = App.UserServer.GetUserById (g.User);
					if (user != null) {
						profilPic.Source = user.Image;
						nameBtn.Text = user.Name;
					}

					MoreBtn.Command = g.MoreCommand;

					if (AccPlot.Model == null) {

						List<double[]> acc = JsonConvert.DeserializeObject<List<double[]>> (g.Accelaration);


						var gestureModel = new PlotModel { Title = "Accelaration" }; 
						for (int i = 0; i < acc.Count; i++)
							gestureModel.Series.Add (new LineSeries (){ ItemsSource = new  List<DataPoint> () });

						gestureModel.Axes.Add (new LinearAxis () { Position = AxisPosition.Bottom,
							Minimum = 0,
							Maximum = acc[0].Length
						});
						gestureModel.Axes.Add (new LinearAxis () { Position = AxisPosition.Left,
							Minimum = -2.5,
							Maximum = 2.5
						});


						int cx = 0;
						foreach (double[] x in acc) {
							for (int i = 0; i < x.Length; i++)
								(((LineSeries)gestureModel.Series [cx]).ItemsSource as List<DataPoint>).Add (new DataPoint (i, x [i]));

							cx++;
						}

						List<double[]> emg = JsonConvert.DeserializeObject<List<double[]>> (g.EMG);


						var emgModel = new PlotModel { Title = "EMG" }; 
						for (int i = 0; i < emg.Count; i++)
							emgModel.Series.Add (new LineSeries (){ ItemsSource = new  List<DataPoint> () });

						emgModel.Axes.Add (new LinearAxis () { Position = AxisPosition.Bottom,
							Minimum = 0,
							Maximum = emg[0].Length
						});
						emgModel.Axes.Add (new LinearAxis () { Position = AxisPosition.Left,
							Minimum = -2.5,
							Maximum = 2.5
						});


						cx = 0;
						foreach (double[] x in emg) {
							for (int i = 0; i < x.Length; i++)
								(((LineSeries)emgModel.Series [cx]).ItemsSource as List<DataPoint>).Add (new DataPoint (i, x [i]));

							cx++;
						}

						try{
						AccPlot.Model = gestureModel;
						EMGPlot.Model = emgModel;
						}catch(Exception e){
							Debug.WriteLine (e.Message);
						}
					}
				}		
			}
		}
	}
}

