using Android;
using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using AndroidX.Work;
using System;

namespace PeriodicBackgroundService.Android
{
    [Activity(Label = "PeriodicBackgroundService", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MainActivity : Activity
    {
        int count = 1;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button>(Resource.Id.myButton);

            button.Click += delegate
            {
                button.Text = string.Format("{0} clicks!", count++);
            };

            //SetAlarmForBackgroundServices(this);
            this.StartBackgroundServices();
        }

        private void StartBackgroundServices()
        {
            var constraints = new Constraints.Builder()
                        .SetRequiredNetworkType(NetworkType.Connected)
                        .Build();

            // Cannot set a period inferior to 15 minutes
            // source: https://medium.com/@yonatanvlevin/the-minimum-interval-for-periodicwork-is-15-minutes-same-as-jobscheduler-periodic-job-eb2d63716d1f#:~:text=The%20minimum%20interval%20for%20PeriodicWork,same%20as%20JobScheduler%20Periodic%20Job).
            var periodicWork = PeriodicWorkRequest.Builder
                .From<SyncDataWorker>(TimeSpan.FromMinutes(15))
                .SetConstraints(constraints)
                .Build();

            WorkManager
                .GetInstance(this.ApplicationContext)
                .Enqueue(periodicWork);
        }

        //public static void SetAlarmForBackgroundServices(Context context)
        //{
        //	var alarmIntent = new Intent(context.ApplicationContext, typeof(AlarmReceiver));
        //	var broadcast = PendingIntent.GetBroadcast(context.ApplicationContext, 0, alarmIntent, PendingIntentFlags.NoCreate);
        //	if (broadcast == null)
        //	{
        //		var pendingIntent = PendingIntent.GetBroadcast(context.ApplicationContext, 0, alarmIntent, 0);
        //		var alarmManager = (AlarmManager)context.GetSystemService(Context.AlarmService);
        //		alarmManager.SetRepeating(AlarmType.ElapsedRealtimeWakeup, 300, 10000, pendingIntent);
        //	}
        //}
    }
}


