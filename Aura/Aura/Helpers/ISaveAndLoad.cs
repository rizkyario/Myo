using System;

using Xamarin.Forms;
using System.IO;

namespace Aura
{
	public interface ISaveAndLoad {
		void SaveObject (string filename, Object obj);
		Object LoadObject (string filename);
	}
}


