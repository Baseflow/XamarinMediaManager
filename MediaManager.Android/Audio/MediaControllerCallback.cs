using System;
using Android.Support.V4.Media.Session;

namespace Plugin.MediaManager.Audio
{
    public class MediaControllerCallback : MediaControllerCompat.Callback
    {
        private MediaManagerImplementation mediaManagerImplementation;

        public MediaControllerCallback()
        {
        }

        public MediaControllerCallback(MediaManagerImplementation mediaManagerImplementation)
        {
            this.mediaManagerImplementation = mediaManagerImplementation;
        }

        public override void OnPlaybackStateChanged(PlaybackStateCompat state)
        {
            base.OnPlaybackStateChanged(state);
        }

        public override void OnSessionEvent(string @event, Android.OS.Bundle extras)
        {
            base.OnSessionEvent(@event, extras);
        }
    }
}
