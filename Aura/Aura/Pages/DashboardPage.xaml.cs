using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace Aura
{
	public partial class DashboardPage : ContentPage
	{
		public MainMenu Parent;

		public DashboardPage ()
		{
			InitializeComponent ();

		}

		public void OnBurgerMenuBtnClicked (object o, EventArgs e)
		{
			Parent.IsPresented = true;
		}
	}
}

