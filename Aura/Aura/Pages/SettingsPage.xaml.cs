using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Aura.Helpers;

namespace Aura
{
	public partial class SettingsPage : ContentPage
	{
		public SettingsPage ()
		{
			NavigationPage.SetHasNavigationBar(this, false);
			InitializeComponent ();
			BindingContext = App.AuraVM;
		}

		public void OnConnectButtonClicked (object o, EventArgs e)
		{
			DependencyService.Get<IMyoSensor> ().Connect();
		}

		public async void OnLogoutBtnClicked (object o, EventArgs e)
		{
			var answer = await DisplayAlert (AppResources.LogOutLabel+"?", AppResources.LogOutConfLabel, AppResources.YesBtnLabel, AppResources.NoBtnLabel);
			if (answer) {
				Settings.CurrentUser = null;
				App.Instance.MainPage = new IntroPage ();
			}
		}

		public async void OnXBtnClicked (object o, EventArgs e)
		{
			await Navigation.PopModalAsync ();
		}
	}
}

