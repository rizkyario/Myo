using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace Aura
{
	public class SignsViewModel: INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		ObservableCollection<Sign>  signsFiltered;
		ObservableCollection<Grouping<string, Sign>> signsGrouped;

		private string pageLabel;
		public string PageLabel
		{
			get { return pageLabel; }
			set 
			{
				if (pageLabel == value)
					return;

				pageLabel = value;
				OnPropertyChanged ("PageLabel");
			}
		}

		public ListView CurrentList { set; get;}

		public ObservableCollection<Grouping<string, Sign>> SignsGrouped { 
			set
			{
				if (signsGrouped != value)
				{
					signsGrouped = value;

					if (PropertyChanged != null)
					{
						PropertyChanged(this, 
							new PropertyChangedEventArgs("SignsGrouped"));
												}
				}
			}
			get
			{
				return signsGrouped;
			}
		}

		public ObservableCollection<Sign> SignsFiltered { 
			set
			{
				if (signsFiltered != value)
				{
					signsFiltered = value;

					if (PropertyChanged != null)
					{
						PropertyChanged(this, 
							new PropertyChangedEventArgs("SignsFiltered"));
					}
				}
			}
			get
			{
				return signsFiltered;
			}
		}
		public ObservableCollection<Sign> SignsPopular { get; set; }
		private double progress;
		public double Progress
		{
			get { return progress; }
			set 
			{
				if (Progress == value)
					return;

				progress = value;
				OnPropertyChanged ("Progress");
			}
		}

		private bool isBusy;
		public bool IsBusy
		{
			get { return isBusy; }
			set 
			{
				if (isBusy == value)
					return;

				isBusy = value;
				OnPropertyChanged ("IsBusy");
			}
		}

		public void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged == null)
				return;

			PropertyChanged (this, new PropertyChangedEventArgs (propertyName));
		}

		public SignsViewModel()
		{
			IsLoadingDictionary = true;
		}
	
		private bool isLoadingDictionary;
		public bool IsLoadingDictionary
		{
			get { return isLoadingDictionary; }
			set 
			{
				if (isLoadingDictionary == value)
					return;

				isLoadingDictionary = value;
				OnPropertyChanged ("IsLoadingDictionary");
			}
		}

		public async Task InitSignsGroupAsync()
		{
			IsLoadingDictionary = true;
			SignsGrouped = await App.SignServer.InitSignsGroupCache ();
			if (SignsGrouped.Count == 0) {
				var syncedSignsGrouped = await App.SignServer.InitSignsGroupAsync ();
				if (syncedSignsGrouped.Count > 0)
					SignsGrouped = syncedSignsGrouped;
			}
			IsLoadingDictionary = false;
		}

		public async Task GetSignsByKeyword(string word)
		{
			IsBusy = true; 
			SignsFiltered = await App.SignServer.GetSignsByWordCache (word);
			var syncedSignsFiltered = await App.SignServer.GetSignsByWordAsync (word);
			if (syncedSignsFiltered.Count > 0)
				SignsFiltered = syncedSignsFiltered;
			IsBusy = false;
		}

		public async Task GetSignsByKeywordAndTag(string type)
		{
			IsBusy = true; 
			SignsFiltered = await App.SignServer.GetSignsByWordAndTagAsync (type);
			IsBusy = false;
		}



		public async Task<ObservableCollection<Sign>> GetSign(string type)
		{
			type = type.Replace ("#", "").Trim();
			IsBusy = true; 

			ObservableCollection<Sign> temp = new ObservableCollection<Sign> ();

			temp = await App.SignServer.GetASignByWordAsync (type);

			IsBusy = false;

			return temp;
		}
			
		public async void InitDB()
		{
			ObservableCollection<Sign> Signs =  new ObservableCollection<Sign>();

			//Initiate Sign Database
			foreach (Sign mon in Signs) {
				await App.SignServer.SaveSignAsync (mon);
			}
		}
			
	}
}

