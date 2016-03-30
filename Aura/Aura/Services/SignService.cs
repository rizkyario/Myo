using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Microsoft.WindowsAzure.MobileServices;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.CompilerServices;


namespace Aura
{
	public class SignService {

		IMobileServiceSyncTable<Sign> signTable;
		IMobileServiceClient client;
		List<Sign> Signs;

		public SignService (IMobileServiceClient client, IMobileServiceSyncTable<Sign> signTable) 
		{
			this.client = client;
			this.signTable = signTable;
		}

		public async Task<Sign> GetSignByIdAsync(string id)
		{
			try 
			{
//				await SyncAsync();
				return await signTable.LookupAsync(id);
			} 
			catch (MobileServiceInvalidOperationException msioe)
			{
				Debug.WriteLine(@"GetSignByIdAsync INVALID {0}", msioe.Message);
			}
			catch (Exception e) 
			{
				Debug.WriteLine(@"GetSignByIdAsync ERROR {0}", e.Message);
			}
			return null;
		}

		public async Task<ObservableCollection<Sign>> GetSignsAsync ()
		{
			try 
			{
//				await SyncAsync();
				return new ObservableCollection<Sign> (await signTable.ReadAsync());
			} 
			catch (MobileServiceInvalidOperationException msioe)
			{
				Debug.WriteLine(@"GetSignsAsync INVALID {0}", msioe.Message);
			}
			catch (Exception e) 
			{
				Debug.WriteLine(@"GetSignsAsync ERROR {0}", e.Message);
			}
			return null;
		}

		public async Task<ObservableCollection<Sign>> GetSignsPopular()
		{
			try 
			{
				await SyncAsync();
//
				List<Sign> Signs =  await signTable.ToListAsync();

				var shorted = Signs.OrderByDescending(item => item.PopularityCount()).Take(10);
//
//				App.signVM.SignsFiltered= new ObservableCollection<Sign> (shorted);
				return new ObservableCollection<Sign> (shorted);
			} 
			catch (MobileServiceInvalidOperationException msioe)
			{
				Debug.WriteLine(@"GetSignsPopular INVALID {0}", msioe.Message);
			}
			catch (Exception e) 
			{
				Debug.WriteLine(@"GetSignsPopular ERROR {0}", e.Message);
			}
			return null;
		}

		public async Task<ObservableCollection<Grouping<string, Sign>>> GetSignsGroupAsync ()
		{
			try 
			{
//				await SyncAsync();

				List<Sign> Signs =  await signTable.ToListAsync();
				var grouped = Signs.GroupBy(item => item.Word);
				var shortest = grouped.Select(grp => grp.OrderByDescending(item => item.Vote).First());

				var sorted = (from sign in shortest
				              orderby sign.Word
				              group sign by sign.WordSort into signGroup
							  select new Grouping<string, Sign> (signGroup.Key,signGroup));


				return new ObservableCollection<Grouping<string, Sign>>(sorted);
			}
			catch (MobileServiceInvalidOperationException msioe)
			{
				Debug.WriteLine(@"GetSignsGroupAsync INVALID {0}", msioe.Message);

			}
			catch (Exception e) 
			{
				Debug.WriteLine(@"GetSignsGroupAsync INVALID {0}", e.Message);

			}
			return null;
		}

		public async Task<ObservableCollection<Sign>> GetSignsByWordAndTagAsync (string word)
		{
			try 
			{
				await SyncAsync();

				List<Sign> Signs =  await signTable.ToListAsync ();	

				var filtered = Signs.Where(t => t.Word.ToLower()==word.ToLower()||t.Explanation.ToLower().Contains("#"+word.ToLower()));
				var shorted = filtered.OrderByDescending(item => item.Love);

				var located = shorted.OrderBy(x => x.Word);

				return new ObservableCollection<Sign>(located);
			}
			catch (MobileServiceInvalidOperationException msioe)
			{
				Debug.WriteLine(@"GetSignsByWordAsync INVALID {0}", msioe.Message);
			}
			catch (Exception e) 
			{
				Debug.WriteLine(@"GetSignsByWordAsync INVALID {0}", e.Message);

			}
			return null;
		}

		public async Task<ObservableCollection<Sign>> GetSignsByWordCache (string word)
		{
			try {
				List<Sign> Signs = await signTable.ToListAsync ();	
				var filtered = Signs.Where (t => t.Word.ToLower () == word.ToLower ());
				return new ObservableCollection<Sign> (filtered);
			} catch (Exception e) {
			}
			return null;
		}

		public async Task<ObservableCollection<Sign>> GetSignsByWordAsync (string word)
		{
			try 
			{
				await SyncAsync();
				List<Sign> Signs =  await signTable.ToListAsync ();	
				var filtered = Signs.Where(t => t.Word.ToLower()==word.ToLower());
				return new ObservableCollection<Sign>(filtered);
			}
			catch (MobileServiceInvalidOperationException msioe)
			{
				Debug.WriteLine(@"GetSignsByWordAsync INVALID {0}", msioe.Message);
			}
			catch (Exception e) 
			{
				Debug.WriteLine(@"GetSignsByWordAsync INVALID {0}", e.Message);

			}
			return null;
		}

		public async Task<ObservableCollection<Sign>> GetASignByWordAsync (string word)
		{
			try 
			{
				await SyncAsync();

				List<Sign> Signs =  await signTable.ToListAsync ();	
				var filtered = Signs.Where(t => t.Word.ToLower()==word.ToLower()||t.Explanation.ToLower().Contains("#"+word.ToLower()));

				var filtered2 = filtered.Take(1);

				return new ObservableCollection<Sign>(filtered2);
			}
			catch (MobileServiceInvalidOperationException msioe)
			{
				Debug.WriteLine(@"GetSignByWordAsync INVALID {0}", msioe.Message);
			}
			catch (Exception e) 
			{
				Debug.WriteLine(@"GetSignByWordAsync INVALID {0}", e.Message);

			}
			return null;
		}

		public async Task<ObservableCollection<Sign>> GetSignsOfTheDay ()
		{
			try 
			{
				await SyncAsync();

				List<Sign> Signs =  await signTable.ToListAsync ();	

				string s = System.DateTime.Now.Date.Date.ToString ("M/d/yyyy h:mm:ss tt").Replace("/","").Replace(" 12:00:00 AM","");
				int seed =0;
				int.TryParse(s, out seed);
				var random = new Random(seed);
				int randomNumber = random.Next(0, Signs.Count-1);
				ObservableCollection<Sign> SOTD = new ObservableCollection<Sign>();

				Sign result = Signs.ElementAt(randomNumber);
				SOTD.Add(result);
				return SOTD;
			}
			catch (MobileServiceInvalidOperationException msioe)
			{
				Debug.WriteLine(@"GetSignsOfTheDay INVALID {0}", msioe.Message);
			}
			catch (Exception e) 
			{
				Debug.WriteLine(@"GetSignsOfTheDay INVALID {0}", e.Message);

			}
			return null;
		}



		public async Task SaveSignAsync (Sign item)
		{
			if (item.ID == null)
				await signTable.InsertAsync(item);
			else
				await signTable.UpdateAsync(item);
			await SyncAsync ();
		}

		public async Task SyncAsync([CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "")
		{
			try
			{
				await this.client.SyncContext.PushAsync().ContinueWith (
					t => {
						Debug.Track ("Service",sourceFilePath, memberName, t.Status.ToString());
						if(t.Status == TaskStatus.Faulted)
							Debug.WriteLine ((t.Exception.InnerExceptions[0]).Message);
					});
//				await this.signTable.PurgeAsync(true);
//
////				for(int i=0;i<5;i++)
////				{
//				var query = signTable.Where (t => t.Location == "Indonesia");
//
//				await this.signTable.PullAsync(null, query);
//				}
					
			}
			catch (MobileServiceInvalidOperationException e)
			{
				Debug.WriteLine(@"SignService Sync Failed: {0}", e.Message);
			}
		}

		public async Task<ObservableCollection<Grouping<string, Sign>>> InitSignsGroupCache ()
		{
			Signs =  await signTable.Where(t => t.Word.ToLower().Contains("Saya") || t.Word.ToLower().Contains("Dia") || t.Word.ToLower().Contains("Kamu")).ToListAsync();

			var merged = from sign in Signs
				group sign by sign.Word into signGroup
				select new Sign
			{
				Word = signGroup.Key,
				Image = ((from s in signGroup select s.Image).FirstOrDefault()).ToString() ,
				Location = string.Join(" · ", signGroup.OrderBy(s => s.Word)
					.Select(s => s.Location).Distinct())
			};

			var sorted = (from sign in merged
				orderby sign.Word
				group sign by sign.WordSort into signGroup
				select new Grouping<string, Sign> (signGroup.Key, signGroup));


			
			return new ObservableCollection<Grouping<string, Sign>>(sorted);
		}

		public async Task<ObservableCollection<Grouping<string, Sign>>> InitSignsGroupAsync ()
		{
			try 
			{
				await this.client.SyncContext.PushAsync();

				Signs = await signTable.Where(t => t.Word.ToLower().Contains("Saya") || t.Word.ToLower().Contains("Dia") || t.Word.ToLower().Contains("Kamu")).ToListAsync();

				var query = signTable.Where (t => t.Location == "Jakarta").IncludeTotalCount();
				await this.signTable.PullAsync(null, query);

				query = signTable.Where (t => t.Location == "Bandung");
				await this.signTable.PullAsync(null, query);

				query = signTable.Where (t => t.Location == "Jogja");
				await this.signTable.PullAsync(null, query);

				query = signTable.Where (t => t.Location == "Solo");
				await this.signTable.PullAsync(null, query);

				query = signTable.Where (t => t.Location == "Magelang");
				await this.signTable.PullAsync(null, query);

				Signs =  await signTable.ToListAsync();
					
				var merged = from sign in Signs
					group sign by sign.Word into signGroup
					select new Sign
					{
					Word = signGroup.Key,
					Image = ((from s in signGroup select s.Image).FirstOrDefault()).ToString() ,
					Location = string.Join(" · ", signGroup.OrderBy(s => s.Word)
						.Select(s => s.Location).Distinct())
					};

				var sorted = (from sign in merged
					orderby sign.Word
					group sign by sign.WordSort into signGroup
					select new Grouping<string, Sign> (signGroup.Key, signGroup));
					

				Signs =  merged.ToList();

				return new ObservableCollection<Grouping<string, Sign>>(sorted);
				
			}
			catch (MobileServiceInvalidOperationException msioe)
			{
				Debug.WriteLine(@"InitSignsGroupAsync INVALID {0}", msioe.Message);

			}
			catch (Exception e) 
			{
				Debug.WriteLine(@"InitSignsGroupAsync INVALID {0}", e.Message);

			}
			return new ObservableCollection<Grouping<string, Sign>>();
		}
	}
}

