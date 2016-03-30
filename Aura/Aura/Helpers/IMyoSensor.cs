using System;
using Xamarin.Forms;

namespace Aura
{
	public interface IMyoSensor
	{
		void AttachToAdjacent();
		void Initialize();
		void Connect();
	}
}

