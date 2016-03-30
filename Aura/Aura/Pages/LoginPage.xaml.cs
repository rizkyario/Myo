using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace Aura
{
	public partial class LoginPage : ContentPage
	{
		public LoginPage (String oAuthType)
		{
			InitializeComponent ();
			OAuthType = oAuthType;
		}

		public static readonly BindableProperty OAuthTypeProperty = 
			BindableProperty.Create<LoginPage,string>(
				p => p.OAuthType,string.Empty);

		public string OAuthType {
			get { return (string)GetValue (OAuthTypeProperty); }
			set { SetValue (OAuthTypeProperty, value); }
		}
	}
}

