using System;
using Aura;
using System.IO;
using Xamarin.Forms;
using System.Runtime.Serialization.Formatters.Binary;


[assembly: Dependency (typeof (SaveAndLoad))]
namespace Aura {
	public class SaveAndLoad : ISaveAndLoad {
		public void SaveObject (string filename, Object obj) {
			var documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
			var filePath = Path.Combine (documentsPath, filename);

			using (FileStream fs = new FileStream(filePath, FileMode.Create))
			{
				BinaryFormatter b = new BinaryFormatter();
				b.Serialize(fs, obj);
			}
		}

		public Object LoadObject (string filename) {
			var documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
			var filePath = Path.Combine (documentsPath, filename);

			using (FileStream fs = new FileStream(filePath, FileMode.Open))
			{
				BinaryFormatter b = new BinaryFormatter();
				return (Object)b.Deserialize(fs);
			}
		}
	}
}

