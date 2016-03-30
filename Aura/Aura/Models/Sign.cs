using System;
using Newtonsoft.Json;
using Xamarin.Forms;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Acr.UserDialogs;

namespace Aura
{
	public class Sign: INotifyPropertyChanged
	{
		private string loveText;
		private string word;
		public event PropertyChangedEventHandler PropertyChanged;
		private Boolean isActive;


		[JsonProperty(PropertyName = "id")]
		public string ID { get; set; }

		[JsonProperty(PropertyName = "location")]
		public string Location {get;set;}

		[JsonProperty(PropertyName = "user")]
		public string User { get; set; }

		[JsonProperty(PropertyName = "word")]
		public string Word { 
			get { 
				Int64 number;
				if (Int64.TryParse (this.word, out number))
					return string.Format ("{0:#,#}", number);
				else
					return FirstLetterToUpper(this.word);
			} 
			set { this.word = value; } 
		}

		public string FirstLetterToUpper(string str)
		{
			if (str == null)
				return null;

			if (str.Length > 1)
				return char.ToUpper(str[0]) + str.Substring(1);

			return str.ToUpper();
		}

		[JsonProperty(PropertyName = "vote")]
		public int Vote { get; set; }

		[JsonProperty(PropertyName = "video")]
		public string Video { get; set; }

		[JsonProperty(PropertyName = "language")]
		public string Language { get; set; }

		[JsonProperty(PropertyName = "image")]
		public string Image { get; set; }

		[JsonProperty(PropertyName = "explanation")]
		public string Explanation { get; set; }

		[JsonProperty(PropertyName = "reportedby")]
		public string ReportedBy { get; set; }

		[JsonProperty(PropertyName = "love")]
		public string Love { get; set;}


		[JsonIgnore]
		public string LoveText {
			get{ return this.loveText;}  
			set
			{
				if (this.loveText != value) {;
					this.loveText = value;
					if (PropertyChanged != null) {
						{
							PropertyChanged (this, 
								new PropertyChangedEventArgs ("LoveText"));
						}
					}				
				}
			}
		}

		[JsonIgnore]
		public Boolean IsActive {
			get{ return this.isActive; }  
			set {
				if (this.isActive != value) {
					this.isActive = value;
					if (PropertyChanged != null) {
						{
							PropertyChanged (this, 
								new PropertyChangedEventArgs ("IsActive"));
							PropertyChanged (this, 
								new PropertyChangedEventArgs ("IsNotActive"));				
						}
					}
				}
			}
		}

		[JsonIgnore]
		public Boolean IsNotActive {
			get{ return !isActive; }  
		}

		[JsonIgnore]
		public string UserText { 
			get{ return "by " + User;
			} 
		}

		[JsonIgnore]
		public string WordSort
		{
			get
			{
				if (string.IsNullOrWhiteSpace(Word) || Word.Length == 0)
					return "?";

				return Word[0].ToString().ToUpper();
			}
		}
			
			
		public string LoveCount()
		{
			int count = 0;

			foreach (char c in Love) {
				if (c == '#') count++;
			}

			if (count > 0)
				return  count.ToString() ;
			else
				return String.Empty;
		}

		public int PopularityCount()
		{
			int countR = 0;
			int countL = 0;

			foreach (char c in ReportedBy) {
				if (c == '#') countR++;
			}

			foreach (char c in Love) {
				if (c == '#') countL++;
			}

			return (countL*2)-countR;
		}


		[JsonIgnore]
		public string TagsText { 
			get {
				string tagsText = Explanation.Replace ("#Status", "").Replace ("#Example", "").Replace ("#Ask", "").Replace ("#Comment", "");
				tagsText = tagsText.Replace(" ","").Replace("#"," #").ToUpper();
				return tagsText;
			}
		}

		[JsonIgnore]
		private Command playCommand;
		[JsonIgnore]
		public Command PlayCommand 
		{
			get
			{

				return playCommand ?? (playCommand = new Command(
					() => Play(),
					() => true)); 
			}
		}

		public void Play()
		{
			Debug.WriteLine ("is Triggered");
			IsActive = true;
		}
//		[JsonIgnore]
//		ObservableCollection<MoreType> moreButtons;
//		[JsonIgnore]
//		public ObservableCollection<MoreType> MoreButtons { 
//			set
//			{
//				if (moreButtons != value)
//				{
//					moreButtons = value;
//
//					if (PropertyChanged != null)
//					{
//						PropertyChanged(this, 
//							new PropertyChangedEventArgs("MoreButtons"));
//					}
//				}
//			}
//			get
//			{
//				return moreButtons;
//			}
			
		[JsonIgnore]
		ObservableCollection<string> tags;
		[JsonIgnore]
		public ObservableCollection<string> Tags { 
			set {
				if (tags != value) {
					tags = value;

					if (PropertyChanged != null) {
						PropertyChanged (this, 
							new PropertyChangedEventArgs ("Tags"));
					}
				}
			}
			get {
				return tags;
			}
		}
	}
}

