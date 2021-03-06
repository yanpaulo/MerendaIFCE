﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using Firebase.Messaging;

namespace MerendaIFCE.UserApp.Droid
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class MyFirebaseMessagingService : FirebaseMessagingService
    {
        const string TAG = "MyFirebaseMsgService";

        public override void OnMessageReceived(RemoteMessage message)
        {
            Log.Debug(TAG, "From: " + message.From);
            if (message.GetNotification() != null)
            {
                //These is how most messages will be received
                Log.Debug(TAG, "Notification Message Body: " + message.GetNotification().Body);
                SendNotification(message.GetNotification().Body);
            }
            else
            {
                //Only used for debugging payloads sent from the Azure portal
                SendNotification(message.Data.Values.First());

            }

        }

        void SendNotification(string messageBody)
        {
            var intent = new Intent(this, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.ClearTop);
            var pendingIntent = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.OneShot);

            var soundUri = RingtoneManager.GetDefaultUri(RingtoneType.Ringtone);
            var bigTextStyle = new NotificationCompat.BigTextStyle();
            bigTextStyle.SetSummaryText("Almoço");
            bigTextStyle.BigText(messageBody);

            const string ChannelId = "{204A0CA8-0AA3-4427-B8CC-25F8F5A5A6AA}";

            var notificationBuilder = new NotificationCompat.Builder(this, ChannelId)
                        .SetTicker("Almoço")
                        .SetContentTitle("Almoço")
                        .SetContentText(messageBody)
                        .SetSmallIcon(Resource.Mipmap.icon)
                        .SetStyle(bigTextStyle)
                        .SetAutoCancel(true)
                        .SetContentIntent(pendingIntent)
                        .SetPriority(NotificationCompat.PriorityMax)
                        .SetDefaults((int)NotificationDefaults.All);

            var notificationManager = NotificationManager.FromContext(this);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channel = notificationManager.GetNotificationChannel(ChannelId);
                if (channel == null)
                {
                    channel = new NotificationChannel(ChannelId, "Alertas", NotificationImportance.High);
                    notificationManager.CreateNotificationChannel(channel);
                }
            }

            notificationManager.Notify(0, notificationBuilder.Build());
        }
    }
}