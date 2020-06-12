using System;
using System.Threading.Tasks;
using Android.OS;
using Android.App;
using Android.Content;
using Android.Util;
using System.IO;
using System.Net;
using System.Net.Http;
using Java.Sql;

namespace PeriodicBackgroundService.Android
{
	[Service]
	class PeriodicBackgroundService : Service
	{
		private const string Tag = "[PeriodicBackgroundService]";

		private bool _isRunning;
		private Context _context;
		private Task _task;
		private int _calls;
		static readonly HttpClient client = new HttpClient();

		#region overrides

		public override IBinder OnBind(Intent intent)
		{
			return null;
		}

		public override void OnCreate()
		{
			_context = this;
			_isRunning = false;
			_task = new Task(DoWork);
			_calls = 0;
		}

		public override void OnDestroy()
		{
			_isRunning = false;

			if (_task != null && _task.Status == TaskStatus.RanToCompletion)
			{
				_task.Dispose();
			}
		}

		public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
		{
			if (!_isRunning)
			{
				_isRunning = true;
				_task.Start();
			}
			return StartCommandResult.Sticky;
		}

		#endregion

		private async void DoWork()
		{

			string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
			string fileName = Path.Combine(path, "backgroundfile.txt");
			Console.WriteLine(fileName);
			var streamWriter = new StreamWriter(fileName, true);
			streamWriter.WriteLine("Begin task at " + DateTime.Now);
			try
			{
				Log.WriteLine(LogPriority.Info, Tag, "Started at " + DateTime.Now);
				try
				{
					streamWriter.WriteLine("Calling https://jsonplaceholder.typicode.com/todos/1 at " + DateTime.Now);
					HttpResponseMessage response = await client.GetAsync("https://jsonplaceholder.typicode.com/todos/1");
					response.EnsureSuccessStatusCode();
					string responseBody = await response.Content.ReadAsStringAsync();
					streamWriter.WriteLine("API response : " + responseBody);

				}
				catch (HttpRequestException e)
				{
					streamWriter.WriteLine("An error occured while calling API at " + DateTime.Now);
				}
				streamWriter.WriteLine("Task ended at " + DateTime.Now);
				streamWriter.WriteLine("");
				streamWriter.Close();
			}
			catch (Exception e)
			{
				Log.WriteLine(LogPriority.Error, Tag, e.ToString());
			}
			finally
			{
				StopSelf();
			}
		}
	}
}

