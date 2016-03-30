using System;
using System.Collections.Generic;

using Xamarin.Forms;
using System.Threading.Tasks;
using Acr.UserDialogs;

namespace Aura
{
	public partial class WordListPage : ContentPage
	{
		public MainMenu Parent;

		public WordListPage ()
		{
			NavigationPage.SetHasNavigationBar(this, false);
			InitializeComponent ();
			this.BindingContext = App.AuraVM.SignVM;
		}

		void HandleItemAppearing (object sender, ItemVisibilityEventArgs e)
		{
			App.AuraVM.SignVM.IsLoadingDictionary = false;
		}

		protected override void OnAppearing ()
		{
			base.OnAppearing ();
		}

		public async void OnItemSelected(object sender, ItemTappedEventArgs args)
		{
			var sign = args.Item as Sign;
			if (sign == null)
				return;

			Page page = new Page ();
			UserDialogs.Instance.ShowLoading("Loading");
			await Task.Run (() => {
				page = new SignListPage(sign.Word);
			});
			UserDialogs.Instance.HideLoading ();

			// Reset the selected item
			list.SelectedItem = null;

			await this.Navigation.PushAsync(page);

		}

		public void OnBurgerMenuBtnClicked (object o, EventArgs e)
		{
			Parent.IsPresented = true;
		}
	}
}

