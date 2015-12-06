using System;

using Xamarin.Forms;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Net.Http;
using System.Collections.Generic;
using System.Net.Http.Headers;
using OxyPlot.Xamarin.Forms;
using OxyPlot;
using OxyPlot.Series;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using XLabs.Ioc;
using Plugin.TextToSpeech;

namespace Aura
{
	public class StringTable
	{
		public string[] ColumnNames { get; set; }
		public string[,] Values { get; set; }
	}

	public class GesturesViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		ObservableCollection<Gesture> gestureList;


		public GesturesViewModel()
		{
		}
			
		void ItemAppearing (object sender, ItemVisibilityEventArgs e)
		{
			if (IsBusy || gestureList.Count == 0)
				return;  
		}

		public ObservableCollection<Gesture> GestureList { 
			set {
				if (gestureList != value) {
					gestureList = value;
					OnPropertyChanged ("GestureList");
				}
			}
			get {
				return gestureList;
			}
		}


		public async Task GetGestures(Sign sign){
			IsBusy = true; 
			GestureList = await App.GestureServer.GetGesturesByUserCache (sign);
			var syncedGestureList = await App.GestureServer.GetGesturesByUserAsync (sign);
			if (syncedGestureList != null)
				GestureList = syncedGestureList;
			IsBusy = false;
		}

		private bool isBusy;
		public bool IsBusy {
			get { return isBusy; }
			set {
				if (isBusy == value)
					return;
				isBusy = value;
				OnPropertyChanged ("IsBusy");
				OnPropertyChanged ("IsNotBusy");
			}
		}

		public bool IsNotBusy {
			get { return !isBusy; }
		}

		private bool isWordExist;
		public bool IsWordExist {
			get { return isWordExist; }
			set {
				if (isWordExist == value)
					return;

				isWordExist = value;
				OnPropertyChanged ("IsWordExist");
			}
		}
			
		public async Task QuickRefreshCommand(Sign sign)
		{
			var temp = await App.GestureServer.GetGesturesByUserCache (sign);
			GestureList = temp;
		}

		public void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged == null)
				return;

			PropertyChanged (this, new PropertyChangedEventArgs (propertyName));
		}

		public string[,] Sampling(PlotModel pv)
		{
			
			List<DataPoint> x = (((LineSeries)pv.Series [0]).ItemsSource as List<DataPoint>);
			List<DataPoint> y = (((LineSeries)pv.Series [1]).ItemsSource as List<DataPoint>);
			List<DataPoint> z = (((LineSeries)pv.Series [2]).ItemsSource as List<DataPoint>);


			int sampling_number = 30;
			int modulo = x.Count / (sampling_number);

			string[,] acc = new string[1,96];

			acc [0, 0] = "values";

			int i = 0;
			int counter = 1;
			foreach (DataPoint dp in x) {
				if (i % modulo == 0)
				if (counter-1 < sampling_number) {
					acc [0, counter] = (dp.Y.ToString ());
					counter++;
				}
				i++;
			}

			foreach (DataPoint dp in y) {
				if (i % modulo == 0)
				if (counter-1 < (sampling_number*2)){
					acc [0, counter] = (dp.Y.ToString ());
					counter++;
				}
				i++;
			}

			foreach (DataPoint dp in z) {
				if (i % modulo == 0)
				if (counter-1 < (sampling_number*3)){
					acc [0, counter] = (dp.Y.ToString ());
					counter++;
				}
				i++;
			}

			for (int j = 91; j <= 95; j++) {
				acc [0, j] = App.AuraVM.Poses [j - 91].ToString();
			}

			return acc;
		}

		public async Task RecognizeGestureTest()
		{
			using (var client = new HttpClient())
			{
				var scoreRequest = new
				{

					Inputs = new Dictionary<string, StringTable> () { 
						{ 
							"input1", 
							new StringTable() 
							{
								ColumnNames = new string[] {"0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "30", "31", "32", "33", "34", "35", "36", "37", "38", "39", "40", "41", "42", "43", "44", "45", "46", "47", "48", "49", "50", "51", "52", "53", "54", "55", "56", "57", "58", "59", "60", "61", "62", "63", "64", "65", "66", "67", "68", "69", "70", "71", "72", "73", "74", "75", "76", "77", "78", "79", "80", "81", "82", "83", "84", "85", "86", "87", "88", "89", "90", "91", "92", "93", "94", "95"},
								Values = new string[,] {  { "value", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0" },  { "value", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0" },  }
							}
						},
					},
					GlobalParameters = new Dictionary<string, string>() {
					}
				};
				const string apiKey = "Zde7lPecaF6NTADrj9m+qhG+g0xaCriIzKiGNUQ6nq9tx5IzZNzKeHHN7/dtl4VOONmX3kEc6LwgNUe9xuchCg=="; // Replace this with the API key for the web service
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue( "Bearer", apiKey);

				client.BaseAddress = new Uri("https://ussouthcentral.services.azureml.net/workspaces/b153901a004942889ca269de2bef1273/services/d63b788bfd3148e8af98ae5dbcb3cb50/execute?api-version=2.0&details=true");

				// WARNING: The 'await' statement below can result in a deadlock if you are calling this code from the UI thread of an ASP.Net application.
				// One way to address this would be to call ConfigureAwait(false) so that the execution does not attempt to resume on the original context.
				// For instance, replace code such as:
				//      result = await DoSomeTask()
				// with the following:
				//      result = await DoSomeTask().ConfigureAwait(false)


				HttpResponseMessage response = await client.PostAsJsonAsync("", scoreRequest);

				if (response.IsSuccessStatusCode)
				{
					RootResult g = JsonConvert.DeserializeObject<RootResult>(await response.Content.ReadAsStringAsync());
//					
					IDictionary<string, object> output = (IDictionary<string, object>)g.Results.Output;
					object value;
					output.TryGetValue("value", out value);

					JObject json = JObject.Parse(value.ToString());
	
					JToken Values;
					json.TryGetValue("Values", out Values);

					foreach (JToken token in Values.Children())
					{
						Debug.WriteLine(token.First.ToString());
					}
//					string[] score = (string[]) Values;
//					Debug.WriteLine("Result: {0}", g.Results.Output.Score);
				}
				else
				{
					Debug.WriteLine(string.Format("The request failed with status code: {0}", response.StatusCode));

					// Print the headers - they include the requert ID and the timestamp, which are useful for debugging the failure
					Debug.WriteLine(response.Headers.ToString());

					string responseContent = await response.Content.ReadAsStringAsync();
					Debug.WriteLine(responseContent);
				}
			}
		}

		public async Task RecognizeGesture(PlotModel pv)
		{
			string[,] SampleGestures = Sampling (pv);

			using (var client = new HttpClient())
			{
				var scoreRequest = new
				{

					Inputs = new Dictionary<string, StringTable> () { 
						{ 
							"input1", 
							new StringTable() 
							{
								ColumnNames = new string[] {"0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "30", "31", "32", "33", "34", "35", "36", "37", "38", "39", "40", "41", "42", "43", "44", "45", "46", "47", "48", "49", "50", "51", "52", "53", "54", "55", "56", "57", "58", "59", "60", "61", "62", "63", "64", "65", "66", "67", "68", "69", "70", "71", "72", "73", "74", "75", "76", "77", "78", "79", "80", "81", "82", "83", "84", "85", "86", "87", "88", "89", "90", "91", "92", "93", "94", "95"},
								Values = SampleGestures
							}
						},
					},
					GlobalParameters = new Dictionary<string, string>() {
					}
				};
				const string apiKey = "Zde7lPecaF6NTADrj9m+qhG+g0xaCriIzKiGNUQ6nq9tx5IzZNzKeHHN7/dtl4VOONmX3kEc6LwgNUe9xuchCg=="; // Replace this with the API key for the web service
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue( "Bearer", apiKey);

				client.BaseAddress = new Uri("https://ussouthcentral.services.azureml.net/workspaces/b153901a004942889ca269de2bef1273/services/d63b788bfd3148e8af98ae5dbcb3cb50/execute?api-version=2.0&details=true");

				// WARNING: The 'await' statement below can result in a deadlock if you are calling this code from the UI thread of an ASP.Net application.
				// One way to address this would be to call ConfigureAwait(false) so that the execution does not attempt to resume on the original context.
				// For instance, replace code such as:
				//      result = await DoSomeTask()
				// with the following:
				//      result = await DoSomeTask().ConfigureAwait(false)


				HttpResponseMessage response = await client.PostAsJsonAsync("", scoreRequest);

				if (response.IsSuccessStatusCode)
				{
					RootResult g = JsonConvert.DeserializeObject<RootResult>(await response.Content.ReadAsStringAsync());
					//					
					IDictionary<string, object> output = (IDictionary<string, object>)g.Results.Output;
					object value;
					output.TryGetValue("value", out value);

					JObject json = JObject.Parse(value.ToString());

					JToken Values;
					json.TryGetValue("Values", out Values);

					foreach (JToken token in Values.Children())
					{
						Debug.WriteLine(token.First.ToString());
						CrossTextToSpeech.Current.Speak(token.First.ToString());
					}

					App.AuraVM.IsRecognizing = false;
				}
				else
				{
					Debug.WriteLine(string.Format("The request failed with status code: {0}", response.StatusCode));

					// Print the headers - they include the requert ID and the timestamp, which are useful for debugging the failure
					Debug.WriteLine(response.Headers.ToString());

					string responseContent = await response.Content.ReadAsStringAsync();
					Debug.WriteLine(responseContent);
					App.AuraVM.IsRecognizing = false;
				}
			}
		}
	}

	public class GestureResult{
		[JsonProperty(PropertyName = "output1")]
		public Dictionary<string, object> Output { get; set; }
	}

	public class RootResult{
		[JsonProperty(PropertyName = "Results")]
		public GestureResult Results { get; set; }
	}
}



