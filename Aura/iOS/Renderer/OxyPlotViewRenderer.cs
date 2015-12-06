using System;
using Xamarin.Forms;
using OxyPlot.Xamarin.Forms;

[assembly: ExportRenderer (typeof (PlotView), typeof (OxyPlot.Xamarin.iOS.PlotView))]
namespace Aura.iOS
{
	public class OxyPlotViewRenderer:OxyPlot.Xamarin.iOS.PlotView
	{
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.Model = null;
			}

			base.Dispose(disposing);
		}
	}
}

