using System;
using System.Collections.Generic;

using Xamarin.Forms;
using System.Collections.ObjectModel;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;

namespace Aura
{
	public partial class MainMenu : MasterDetailPage
	{
		public MainMenu ()
		{
			NavigationPage.SetHasNavigationBar(this, false);
			InitializeComponent ();
			NavigateTo ("Aura");
			DependencyService.Get<IMyoSensor> ().Initialize();
			App.MyoDataStream.InitializeEMGModel ();
			App.MyoDataStream.InitializeAccelarationModel ();

			BindingContext = App.MyoDataStream;

		}
			
		public void OnMenuBtnClicked (object o, EventArgs e)
		{
			NavigateTo (((Button)o).CommandParameter.ToString ());
			this.IsPresented = false;
		}

		public void NavigateTo(String CommandParameter)
		{
			titleLbl.Text = CommandParameter;
		}
			
		public void OnBurgerMenuBtnClicked (object o, EventArgs e)
		{
			this.IsPresented = true;
		}

		public void OnSettingsBtnClicked (object o, EventArgs e)
		{
			Navigation.PushModalAsync (new SettingsPage ());
		}

		public void OnHelpBtnClicked (object o, EventArgs e)
		{
			Navigation.PushModalAsync (new HelpPage ());
		}
	}
}

