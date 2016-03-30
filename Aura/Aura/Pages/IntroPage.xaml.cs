using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Aura.Helpers;

namespace Aura
{
	public partial class IntroPage : CarouselPage
	{
		public IntroPage ()
		{
			InitializeComponent ();
			NavigationPage.SetHasNavigationBar(this, false);
		}
		protected override void OnAppearing()
		{
			base.OnAppearing();

			//#if ENABLE_TEST_CLOUD
			Settings.CurrentUser = Settings.GetDummyUser ();
			App.Instance.FinishLogin.Invoke ();
			//#endif
		}
		public void OnFBLoginBtnClicked (object o, EventArgs e)
		{
			LoginPage a = new LoginPage ("Facebook");
			this.Navigation.PushModalAsync (a);
		}

		public void OnGoogleLoginBtnClicked (object o, EventArgs e)
		{
			LoginPage a = new LoginPage ("Google");
			this.Navigation.PushModalAsync (a);
		}


		public void Logingin()
		{
			App.Instance.MainPage = new NavigationPage (new MainMenu ());
		}
	}
}

