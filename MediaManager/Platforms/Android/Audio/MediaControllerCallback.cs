using System;
using Android.OS;
using Android.Support.V4.Media;
using Android.Support.V4.Media.Session;

namespace MediaManager.Platforms.Android.Audio
{
    public class MediaControllerCallback : MediaControllerCompat.Callback
    {
        public Action<PlaybackStateCompat> OnPlaybackStateChangedImpl { get; set; }

        public Action<MediaMetadataCompat> OnMetadataChangedImpl { get; set; }

        public Action<string, Bundle> OnSessionEventChangedImpl { get; set; }

        public override void OnPlaybackStateChanged(PlaybackStateCompat state)
        {
            base.OnPlaybackStateChanged(state);
            OnPlaybackStateChangedImpl?.Invoke(state);
        }

        public override void OnMetadataChanged(MediaMetadataCompat metadata)
        {
            base.OnMetadataChanged(metadata);
            OnMetadataChangedImpl?.Invoke(metadata);
        }

        public override void OnSessionEvent(string @event, Bundle extras)
        {
            base.OnSessionEvent(@event, extras);
            OnSessionEventChangedImpl?.Invoke(@event, extras);
        }
    }
}
