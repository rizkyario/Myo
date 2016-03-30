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

