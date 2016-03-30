using System;
using System.Collections.Generic;

using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Aura
{
	public partial class SignListPage : ContentPage
	{
		string word;
		public SignListPage (string word)
		{
			NavigationPage.SetHasNavigationBar (this, false);
			InitializeComponent ();
			this.word = word;
			Task.Run (async () => {
				await GetData ();
			});
			BindingContext = App.AuraVM.SignVM;
			PageLabel.Text = word;
		}

		public async Task GetData()
		{
			App.AuraVM.SignVM.CurrentList = list;
			App.AuraVM.SignVM.SignsFiltered = new ObservableCollection<Sign> ();
			await App.AuraVM.SignVM.GetSignsByKeyword (word);
		}

		protected override void OnAppearing ()
		{
			base.OnAppearing ();
		}

		public async void OnItemSelected(object sender, ItemTappedEventArgs args)
		{
			Sign sign = (Sign)list.SelectedItem;
			foreach (Sign s in list.ItemsSource) {
				s.IsActive = false;
			}
			list.SelectedItem = null;

			await App.Current.MainPage.Navigation.PushAsync (new TrainingPage (sign));
		}

		public void OnPlayClicked(object o, EventArgs e)
		{
			foreach (Sign s in list.ItemsSource) {
				s.IsActive = false;
			}
			((Sign)((Button)o).CommandParameter).Play();
		}



		public async void OnXBtnClicked (object o, EventArgs e)
		{
			await this.Navigation.PopAsync ();
		}
	}
}

