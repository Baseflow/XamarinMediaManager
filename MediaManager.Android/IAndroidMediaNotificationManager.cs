using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Media.Session;
using Android.Views;
using Android.Widget;
using Plugin.MediaManager.Abstractions;
using Plugin.MediaManager.Abstractions.Enums;

namespace Plugin.MediaManager
{
    public interface IAndroidMediaNotificationManager : INotificationManager
    {
        IMediaQueue MediaQueue { get; set; }
        MediaSessionCompat.Token SessionToken { get; set; }


    }
}