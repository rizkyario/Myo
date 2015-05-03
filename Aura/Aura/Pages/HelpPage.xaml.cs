using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace Aura
{
	public partial class HelpPage : ContentPage
	{
		public HelpPage ()
		{
			InitializeComponent ();
		}

		public async void OnXBtnClicked (object o, EventArgs e)
		{
			await Navigation.PopModalAsync ();
		}
	}
}

