using System;
using System.IO;
using System.Net.Http;
using Android.Content;
using Android.Runtime;
using Android.Util;
using AndroidX.Work;

namespace PeriodicBackgroundService.Android
{
    /// <summary>
    /// Doc: https://devblogs.microsoft.com/xamarin/getting-started-workmanager/
    /// </summary>
    public class SyncDataWorker : Worker
    {
        static readonly HttpClient client = new HttpClient();

        public SyncDataWorker(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public SyncDataWorker(Context context, WorkerParameters workerParams) : base(context, workerParams)
        {
        }

        public override Result DoWork()
        {
            string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            string fileName = Path.Combine(path, "backgroundfile.txt");
            Console.WriteLine(fileName);

            Log.Info("SyncDataWorker", "Start");

            using (var streamWriter = new StreamWriter(fileName, true))
            {
                streamWriter.WriteLine("[SyncDataWorker] Begin task at " + DateTime.Now);

                try
                {
                    try
                    {
                        streamWriter.WriteLine("[SyncDataWorker] Calling https://jsonplaceholder.typicode.com/todos/1 at " + DateTime.Now);

                        var getDataTask = client.GetAsync("https://jsonplaceholder.typicode.com/todos/1");
                        getDataTask.Wait();
                        HttpResponseMessage response = getDataTask.Result;

                        response.EnsureSuccessStatusCode();

                        var readFileTask = response.Content.ReadAsStringAsync();
                        readFileTask.Wait();
                        string responseBody = readFileTask.Result;

                        streamWriter.WriteLine("[SyncDataWorker] API response : " + responseBody);

                    }
                    catch (HttpRequestException e)
                    {
                        streamWriter.WriteLine("[SyncDataWorker] An error occured while calling API at " + DateTime.Now);
                    }
                    streamWriter.WriteLine("[SyncDataWorker] Task ended at " + DateTime.Now);
                    streamWriter.WriteLine("");

                    Log.Info("SyncDataWorker", "End");

                    return Result.InvokeSuccess();
                }
                catch (Exception ex)
                {
                    streamWriter.WriteLine("[SyncDataWorker] Exception task at " + DateTime.Now);
                    streamWriter.WriteLine(ex);

                    Log.Info("SyncDataWorker", "Fail");

                    return Result.InvokeFailure();
                }
            }
        }
    }
}
