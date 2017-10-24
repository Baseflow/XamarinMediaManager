using System;
using Android.OS;
using Android.Support.V4.Media.Session;

namespace Plugin.MediaManager.Audio
{
    public class MediaSessionCallback : MediaSessionCompat.Callback
    {
        private AudioPlayerService _audioPlayerService;

        public MediaSessionCallback(AudioPlayerService audioPlayerService)
        {
            _audioPlayerService = audioPlayerService;
        }

        public Action OnPlayImpl { get; set; }

        public Action<long> OnSkipToQueueItemImpl { get; set; }

        public Action<long> OnSeekToImpl { get; set; }

        public Action<string, Bundle> OnPlayFromMediaIdImpl { get; set; }

        public Action OnPauseImpl { get; set; }

        public Action OnStopImpl { get; set; }

        public Action OnSkipToNextImpl { get; set; }

        public Action OnSkipToPreviousImpl { get; set; }

        public Action<string, Bundle> OnCustomActionImpl { get; set; }

        public Action<string, Bundle> OnPlayFromSearchImpl { get; set; }

        public override void OnPlay()
        {
            OnPlayImpl();
        }

        public override void OnSkipToQueueItem(long id)
        {
            OnSkipToQueueItemImpl(id);
        }

        public override void OnSeekTo(long pos)
        {
            OnSeekToImpl(pos);
        }

        public override void OnPlayFromMediaId(string mediaId, Bundle extras)
        {
            OnPlayFromMediaIdImpl(mediaId, extras);
        }

        public override void OnPause()
        {
            OnPauseImpl();
        }

        public override void OnStop()
        {
            OnStopImpl();
        }

        public override void OnSkipToNext()
        {
            OnSkipToNextImpl();
        }

        public override void OnSkipToPrevious()
        {
            OnSkipToPreviousImpl();
        }

        public override void OnCustomAction(string action, Bundle extras)
        {
            OnCustomActionImpl(action, extras);
        }

        public override void OnPlayFromSearch(string query, Bundle extras)
        {
            OnPlayFromSearchImpl(query, extras);
        }

        public override void OnAddQueueItem(Android.Support.V4.Media.MediaDescriptionCompat description)
        {
            base.OnAddQueueItem(description);
        }

        public override void OnAddQueueItem(Android.Support.V4.Media.MediaDescriptionCompat description, int index)
        {
            base.OnAddQueueItem(description, index);
        }
    }
}
