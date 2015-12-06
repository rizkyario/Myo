using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Microsoft.WindowsAzure.MobileServices;
using System.Collections.ObjectModel;

using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Aura
{
	public class UserService
	{

		IMobileServiceSyncTable<User> userTable;
		IMobileServiceClient client;
		private List<User> Users;

		public UserService (IMobileServiceClient client, IMobileServiceSyncTable<User> userTable) 
		{
			this.client = client;
			this.userTable = userTable;
			this.Users = new List<User> ();
		}

		public User GetUserById(string id)
		{
			List<User> users = Users.Where (t => t.ID == id).ToList();
			User u = null;
			if (users.Count != 0)
				u = users.First ();

			return u;
		}

		public async Task<User> GetUserByIdAsync(string id)
		{
			await SyncAsync ();
			User u = await userTable.LookupAsync (id);
			return u;
		}

		public  ObservableCollection<User> GetUsers ()
		{
			return new ObservableCollection<User>(Users);
		}

		public User GetUserByEmail(string email)
		{
			List<User> users = Users.Where (t => t.Email.ToLower() == email.ToLower()).ToList();
			User u = null;
			if (users.Count != 0)
				u = users.First ();

			return u;
		}

		public async Task<ObservableCollection<User>> GetUserCache ()
		{
			Users = (await userTable.ToListAsync ());

			var shorted = Users.OrderBy (d => d.Name);
			return new ObservableCollection<User> (shorted);

		}

		public async Task<ObservableCollection<User>> GetUserAsync ()
		{
			try {
				await SyncAsync ();

				Users = (await userTable.ToListAsync ());

				var shorted = Users.OrderBy (d => d.Name);
				return new ObservableCollection<User> (shorted);
			} catch (MobileServiceInvalidOperationException msioe) {
				Debug.WriteLine (@"GetPupilsByUserAsync INVALID {0}", msioe.Message);
			} catch (Exception e) {
				Debug.WriteLine (@"GetPupilsByUserAsync ERROR {0}", e.Message);
			}
			return null;
		}

		public async Task<User> GetUserByEmailAsync(User item)
		{

			await SyncAsync ();

			ObservableCollection<User> temp = new ObservableCollection<User>(Users.Where (t => t.Email == item.Email));

			if (temp.Count > 0) {
				return temp.First ();
			} else {
				await SaveUserAsync (item);
				return  Users.Where (t => t.Email.ToLower() == item.Email.ToLower()).First ();
			}
		}


		public async Task SaveUserAsync (User item)
		{
			if (item.ID == null)
				await userTable.InsertAsync (item);
			else
				await userTable.UpdateAsync (item);
			await SyncAsync ();
		}

		public async Task DeleteTaskAsync (User item)
		{
			await userTable.DeleteAsync (item);
			await SyncAsync ();
		}

		public async Task SyncAsync([CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "")
		{
			Users = await userTable.ToListAsync ();
			try {
				await this.client.SyncContext.PushAsync ().ContinueWith (
					t => {
						Debug.Track ("Service",sourceFilePath, memberName, t.Status.ToString());
						if(t.Status == TaskStatus.Faulted)
						{
							Debug.WriteLine ((t.Exception.InnerExceptions[0]).Message);
							//							this.userTable.PurgeAsync();
						}
					}
				);
				await this.userTable.PullAsync ("SelectRelatedUser" + App.debugNumber, userTable.CreateQuery ());
				Users = await userTable.ToListAsync ();
			}
			catch (MobileServiceInvalidOperationException msioe)
			{
				Debug.WriteLine(@"SyncAsync INVALID {0}", msioe.Message);

			}
			catch (Exception e) 
			{
				Debug.WriteLine(@"SyncAsync INVALID {0}", e.Message);
			}
		}
	}
}

