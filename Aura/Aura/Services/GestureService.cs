using System;

using Xamarin.Forms;
using Microsoft.WindowsAzure.MobileServices.Sync;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Generic;

namespace Aura
{
	public class GestureService
	{
		IMobileServiceSyncTable<Gesture> gestureTable;
		IMobileServiceClient client;
		public List<Gesture> Gestures;


		public GestureService (IMobileServiceClient client, IMobileServiceSyncTable<Gesture> gestureTable)
		{
			this.client = client;
			this.gestureTable = gestureTable;
		}

		public async Task<Gesture> GetGestureByIDAsync(string id)
		{
			try {
				await SyncAsync ();
				Gesture a = await gestureTable.LookupAsync (id);
				return a;
			} catch (MobileServiceInvalidOperationException msioe) {
				Debug.WriteLine (@"INVALID {0}", msioe.Message);
			} catch (Exception e) {
				Debug.WriteLine (@"ERROR {0}", e.Message);
			}
			return null;
		}

		public async Task<ObservableCollection<Gesture>> GetGesturesByUserCache (Sign sign)
		{

			Gestures = (await gestureTable.ToListAsync ());
			return new ObservableCollection<Gesture> (Gestures.Where (x => x.Sign == sign.ID));
		}

		public async Task<ObservableCollection<Gesture>> GetGesturesByUserAsync (Sign sign)
		{
			try {
				await SyncAsync ();

				var Gestures = (await gestureTable.ToListAsync ());
			
				return new ObservableCollection<Gesture> (Gestures.Where (x => x.Sign == sign.ID));
//				return new ObservableCollection<Gesture> (Gestures);
			} catch (MobileServiceInvalidOperationException msioe) {
				Debug.WriteLine (@"GetGesturesByUserAsync INVALID {0}", msioe.Message);
			} catch (Exception e) {
				Debug.WriteLine (@"GetGesturesByUserAsync ERROR {0}", e.Message);
			}
			return null;
		}

		public async Task<ObservableCollection<Gesture>> GetGesturesByWordCache (String word)
		{
			try {

				var Gestures = (await gestureTable.ToListAsync ());

				return new ObservableCollection<Gesture> (Gestures.Where (x => x.Word == word));
				//				return new ObservableCollection<Gesture> (Gestures);
			} catch (MobileServiceInvalidOperationException msioe) {
				Debug.WriteLine (@"GetGesturesByUserAsync INVALID {0}", msioe.Message);
			} catch (Exception e) {
				Debug.WriteLine (@"GetGesturesByUserAsync ERROR {0}", e.Message);
			}
			return null;
		}

		public async Task<ObservableCollection<Gesture>> GetGesturesAllCache ()
		{
			try {

				var Gestures = (await gestureTable.ToListAsync ());

				return new ObservableCollection<Gesture> (Gestures);
				//				return new ObservableCollection<Gesture> (Gestures);
			} catch (MobileServiceInvalidOperationException msioe) {
				Debug.WriteLine (@"GetGesturesByUserAsync INVALID {0}", msioe.Message);
			} catch (Exception e) {
				Debug.WriteLine (@"GetGesturesByUserAsync ERROR {0}", e.Message);
			}
			return null;
		}



		public async Task SaveGestureAsync (Gesture item)
		{
			if (item.ID == null)
				await gestureTable.InsertAsync (item);
			else
				await gestureTable.UpdateAsync (item);

			await SyncAsync ();
		}

		public async Task SaveGesture (Gesture item)
		{
			if (item.ID == null)
				await gestureTable.InsertAsync (item);
			else
				await gestureTable.UpdateAsync (item);
		}

		public async Task Sync ()
		{
			await SyncAsync ();
		}

		public async Task DeleteTaskAsync (Gesture item)
		{
			try {
				await gestureTable.DeleteAsync (item);
				await SyncAsync ();
			} catch (MobileServiceInvalidOperationException msioe) {
				Debug.WriteLine (@"INVALID {0}", msioe.Message);
			} catch (Exception e) {
				Debug.WriteLine (@"ERROR {0}", e.Message);
			}
		}

		public async Task SyncAsync()
		{
			try {

				try {
					await this.client.SyncContext.PushAsync ().ContinueWith (
						t => {
							Debug.WriteLine ("Gesture Sync Process is "+t.Status);
							if(t.Status == TaskStatus.Faulted)
								Debug.WriteLine ((t.Exception.InnerExceptions[0]).Message);
						});
				} catch (Exception e) {
					Debug.WriteLine (e.Message);

				}

				await this.gestureTable.PullAsync ("ActNowAllGet" + App.debugNumber, gestureTable.CreateQuery ());

				//Gestures =  await gestureTable.ToListAsync ();

			} catch (MobileServiceInvalidOperationException e) {
				Debug.WriteLine (@"Sync Failed: {0}", e.Message);
			}
		}
	}
}


