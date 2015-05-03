using System;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Collections.Generic;

namespace Aura
{
	public class User: INotifyPropertyChanged
	{
		private int locationIndex;
		private string location;


		[JsonProperty(PropertyName = "id")]
		public string ID { get; set; }

		[JsonProperty(PropertyName = "name")]
		public string Name {get;set;}

		[JsonProperty(PropertyName = "email")]
		public string Email { get; set; }

		[JsonProperty(PropertyName = "facebook")]
		public string Facebook { get; set; }

		[JsonProperty(PropertyName = "image")]
		public string Image { get; set; }

		[JsonProperty(PropertyName = "language")]
		public string Language { get; set; }

		[JsonProperty(PropertyName = "token")]
		public string Token { get; set; }

		[JsonProperty(PropertyName = "location")]
		public string Location { 
			get{ return this.location; }  
			set {
				this.location = value;
				List<string> Locations = new List<string>
				{
					"Jakarta", "Jogja","Bandung", "Solo", "Magelang"
				};

				LocationIndex = Locations.IndexOf(value);
			}
		}

		[JsonProperty(PropertyName = "followers")]
		public string Followers {  get; set; }

		[JsonProperty(PropertyName = "following")]
		public string Following {  get; set; }

		[JsonIgnore]
		public int LocationIndex {
			get{ return this.locationIndex; }  
			set {
				if (this.locationIndex != value) {
					;
					this.locationIndex = value;
					if (PropertyChanged != null) {
						{
							PropertyChanged (this, 
								new PropertyChangedEventArgs ("LocationIndex"));
						}
					}
				}
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

	}
}

