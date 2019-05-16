using System;
using System.Threading.Tasks;
using MediaManager.Media;
using MediaManager.Platforms.Uap.Player;
using MediaManager.Playback;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;

namespace MediaManager.Platforms.Uap.Media
{
    public class WindowsMediaPlayer : IMediaPlayer<MediaPlayer, MediaPlayerSurface>
    {
        public WindowsMediaPlayer()
        {

        }

        protected MediaManagerImplementation MediaManager = CrossMediaManager.Windows;

        public MediaPlayerSurface PlayerView { get; set; }
        public MediaPlayer Player { get; set; }

        public Playback.MediaPlayerState State => Player.PlaybackSession.PlaybackState.ToMediaPlayerState();

        public RepeatMode RepeatMode { get; set; }

        public event BeforePlayingEventHandler BeforePlaying;
        public event AfterPlayingEventHandler AfterPlaying;

        public void Initialize()
        {
            if (Player != null)
                return;

            Player = new MediaPlayer();
            Player.MediaEnded += Player_MediaEnded;
            Player.MediaFailed += Player_MediaFailed;
            Player.PlaybackSession.PlaybackStateChanged += PlaybackSession_PlaybackStateChanged;
        }

        private void PlaybackSession_PlaybackStateChanged(MediaPlaybackSession sender, object args)
        {
            MediaManager.OnStateChanged(this, new StateChangedEventArgs(sender.PlaybackState.ToMediaPlayerState()));
        }

        private void Player_MediaFailed(MediaPlayer sender, MediaPlayerFailedEventArgs args)
        {
            MediaManager.OnMediaItemFailed(this, new MediaItemFailedEventArgs(MediaManager.MediaQueue.Current, new Exception(args.ErrorMessage), args.ErrorMessage));
        }

        private void Player_MediaEnded(MediaPlayer sender, object args)
        {
            MediaManager.OnMediaItemFinished(this, new MediaItemEventArgs(MediaManager.MediaQueue.Current));
        }

        public Task Pause()
        {
            Player.Pause();
            return Task.CompletedTask;
        }

        public async Task Play(IMediaItem mediaItem)
        {
            var mediaPlaybackList = new MediaPlaybackList();
            var mediaSource = await CreateMediaSource(mediaItem);
            var item = new MediaPlaybackItem(mediaSource);
            mediaPlaybackList.Items.Add(item);
            Player.Source = mediaPlaybackList;
            Player.Play();
        }

        public Task Play()
        {
            Player.Play();
            return Task.CompletedTask;
        }

        public Task SeekTo(TimeSpan position)
        {
            Player.PlaybackSession.Position = position;
            return Task.CompletedTask;
        }

        public Task Stop()
        {
            Player.Pause();
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            Player = null;
        }

        //TODO: Refactor this
        private async Task<MediaSource> CreateMediaSource(IMediaItem mediaItem)
        {
            switch (mediaItem.MediaLocation)
            {
                case MediaLocation.Remote:
                    return MediaSource.CreateFromUri(new Uri(mediaItem.MediaUri));
                case MediaLocation.FileSystem:
                    var du = Player.SystemMediaTransportControls.DisplayUpdater;
                    var storageFile = await StorageFile.GetFileFromPathAsync(mediaItem.MediaUri);

                    var playbackType = (mediaItem.MediaType == MediaType.Audio ? Windows.Media.MediaPlaybackType.Music : Windows.Media.MediaPlaybackType.Video);
                    await du.CopyFromFileAsync(playbackType, storageFile);
                    du.Update();

                    return MediaSource.CreateFromStorageFile(storageFile);
            }

            return MediaSource.CreateFromUri(new Uri(mediaItem.MediaUri));
        }
    }
}
