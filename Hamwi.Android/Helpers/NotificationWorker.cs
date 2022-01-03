using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;
using AndroidX.Work;
using Hamwi.Android.Activities;
using Hamwi.Shared.Services.Client;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Hamwi.Android.Helpers
{
    public class NotificationWorker : Worker
    {
        #region Variables
        private const string _channelId = "app_notification_channel_id";
        #endregion

        public NotificationWorker(Context context, WorkerParameters workerParams) : base(context, workerParams)
            => CreateNotificationChannel();

        public override Result DoWork()
        {
            CheckNewNotifications().Wait();
            return Result.InvokeSuccess();
        }

        private async Task CheckNewNotifications()
        {
            try
            {
                var Hamwi = new HamwiServices();
                var notification = (await Hamwi.NotificationService.GetAsync()).LastOrDefault();

                if (notification != null)
                {
                    var notificationId = notification.Id.ToString();

                    if (!LastNotificationId.Equals(notificationId))
                    {
                        var context = Application.Context;
                        var intent = new Intent(context, typeof(MainActivity));
                        intent.PutExtra("notification_id", notificationId);

                        NotificationManagerCompat.From(context).Notify(0,
                            new NotificationCompat.Builder(context, _channelId)
                                .SetAutoCancel(true)
                                .SetSmallIcon(Resource.Mipmap.ic_launcher)
                                .SetContentTitle(notification.Title)
                                .SetContentText(notification.Text)
                                .SetStyle(new NotificationCompat.BigTextStyle()
                                .SetBigContentTitle(notification.Title)
                                .BigText(notification.Text))
                                .SetPriority(NotificationCompat.PriorityHigh)
                                .SetCategory(NotificationCompat.CategoryMessage)
                                .SetContentIntent(PendingIntent.GetActivity(context, 0, intent, PendingIntentFlags.OneShot))
                                .Build());

                        notification.ReceivedCount++;
                        await Hamwi.NotificationService.UpdateAsync(notification);
                        LastNotificationId = notificationId;
                    }
                }
            }
            catch
            {
                return;
            }
        }

        private string LastNotificationId
        {
            get => Preferences.Get("last_notification_id", Guid.Empty.ToString());
            set => Preferences.Set("last_notification_id", value);
        }

        private void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                ((NotificationManager)Application.Context.GetSystemService(Context.NotificationService)).CreateNotificationChannel(new NotificationChannel(_channelId, Application.Context.GetString(Resource.String.notification_name), NotificationImportance.High)
                {
                    Description = Application.Context.GetString(Resource.String.notification_description)
                });
            }
        }
    }
}