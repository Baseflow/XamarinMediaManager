using Android.OS;
using Android.Runtime;
using Android.Support.V4.Media;
using Android.Support.V4.Media.Session;
using Java.Lang;

namespace MediaManager.Platforms.Android.MediaSession
{
    public class MediaControllerCallback : MediaControllerCompat.Callback
    {
        public MediaControllerCallback()
        {
        }

        protected MediaControllerCallback(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public Action<PlaybackStateCompat> OnPlaybackStateChangedImpl { get; set; }

        public Action<MediaMetadataCompat> OnMetadataChangedImpl { get; set; }

        public Action<string, Bundle> OnSessionEventChangedImpl { get; set; }

        public Action BinderDiedImpl { get; set; }

        public Action<MediaControllerCompat.PlaybackInfo> OnAudioInfoChangedImpl { get; set; }

        public Action<bool> OnCaptioningEnabledChangedImpl { get; set; }

        public Action<Bundle> OnExtrasChangedImpl { get; set; }

        public Action<IList<MediaSessionCompat.QueueItem>> OnQueueChangedImpl { get; set; }

        public Action<ICharSequence> OnQueueTitleChangedImpl { get; set; }

        public Action<int> OnRepeatModeChangedImpl { get; set; }

        public Action OnSessionDestroyedImpl { get; set; }

        public Action OnSessionReadyImpl { get; set; }

        public Action<int> OnShuffleModeChangedImpl { get; set; }

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

        public override void BinderDied()
        {
            base.BinderDied();
            BinderDiedImpl?.Invoke();
        }

        public override void OnAudioInfoChanged(MediaControllerCompat.PlaybackInfo info)
        {
            base.OnAudioInfoChanged(info);
            OnAudioInfoChangedImpl?.Invoke(info);
        }

        public override void OnCaptioningEnabledChanged(bool enabled)
        {
            base.OnCaptioningEnabledChanged(enabled);
            OnCaptioningEnabledChangedImpl?.Invoke(enabled);
        }

        public override void OnExtrasChanged(Bundle extras)
        {
            base.OnExtrasChanged(extras);
            OnExtrasChangedImpl?.Invoke(extras);
        }

        public override void OnQueueChanged(IList<MediaSessionCompat.QueueItem> queue)
        {
            base.OnQueueChanged(queue);
            OnQueueChangedImpl?.Invoke(queue);
        }

        public override void OnQueueTitleChanged(ICharSequence title)
        {
            base.OnQueueTitleChanged(title);
            OnQueueTitleChangedImpl?.Invoke(title);
        }

        public override void OnRepeatModeChanged(int repeatMode)
        {
            base.OnRepeatModeChanged(repeatMode);
            OnRepeatModeChangedImpl?.Invoke(repeatMode);
        }

        public override void OnSessionDestroyed()
        {
            base.OnSessionDestroyed();
            OnSessionDestroyedImpl?.Invoke();
        }

        public override void OnSessionReady()
        {
            base.OnSessionReady();
            OnSessionReadyImpl?.Invoke();
        }

        public override void OnShuffleModeChanged(int shuffleMode)
        {
            base.OnShuffleModeChanged(shuffleMode);
            OnShuffleModeChangedImpl?.Invoke(shuffleMode);
        }
    }
}
