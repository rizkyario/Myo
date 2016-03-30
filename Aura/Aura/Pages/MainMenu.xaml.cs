using System;
using System.Collections.Generic;

using Xamarin.Forms;
using System.Collections.ObjectModel;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;
using XLabs.Platform.Services;
using System.Threading.Tasks;
using Aura.Helpers;

namespace Aura
{
	public partial class MainMenu : MasterDetailPage
	{
		public DashboardPage dashboardPage;
		public WordListPage signsPage;

		public MainMenu ()
		{
			NavigationPage.SetHasNavigationBar(this, false);
			App.AuraVM = new AuraViewModel (Settings.CurrentUser);
			dashboardPage = new DashboardPage (){ Parent = this };
			signsPage = new WordListPage (){ Parent = this };

			NavigateTo ("Train Gestures");

			InitializeComponent ();
			InitializeVM ();

			//DependencyService.Get<IMyoSensor> ().Initialize();
			App.AuraVM.InitializeEMGModel ();
			App.AuraVM.InitializeAccelarationModel ();

			BindingContext = App.AuraVM;
//			App.AuraVM.GesturesVM.RecognizeGestureTest ();
		}

		public void NavigateTo(String CommandParameter)
		{
			if (CommandParameter == "Dashboard") {
				App.AuraVM.IsTraining = false;
				this.Detail = dashboardPage;
			}

			if (CommandParameter == "Train Gestures") {
				App.AuraVM.IsTraining = true;
				this.Detail = signsPage;
			}
		}

		public async Task InitializeVM()
		{
//			BindingContext = App.AuraVM;

			//Load Necessary User Data and Class Data
			if ((await App.UserServer.GetUserCache ()).Count == 0)
				await App.UserServer.GetUserAsync ();
			
			await App.AuraVM.SignVM.InitSignsGroupAsync ();
		}

			
		public void OnMenuBtnClicked (object o, EventArgs e)
		{
			NavigateTo (((Button)o).CommandParameter.ToString ());
			this.IsPresented = false;
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

//		public void OnStartButtonClicked (object o, EventArgs e)
//		{
//			scrollView.ScrollToAsync (0, 0, true);
//		}
	}
}

