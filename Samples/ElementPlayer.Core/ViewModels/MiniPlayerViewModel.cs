using System;
using MediaManager;
using MediaManager.Library;
using MediaManager.Media;
using MediaManager.Playback;
using MediaManager.Player;
using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;

namespace ElementPlayer.Core.ViewModels
{
    public class MiniPlayerViewModel : BaseViewModel
    {
        public readonly IMediaManager MediaManager;

        public MiniPlayerViewModel(IMvxLogProvider logProvider, IMvxNavigationService navigationService, IMediaManager mediaManager) : base(logProvider, navigationService)
        {
            MediaManager = mediaManager;
            PlayPauseCommand = new MvxAsyncCommand(MediaManager.PlayPause);

            MediaManager.PositionChanged += Current_PositionChanged;
        }

        public override string Title => "Player";

        public IMvxAsyncCommand PlayPauseCommand { get; }

        public IMediaItem Current => MediaManager.MediaQueue.Current;

        public string CurrentTitle => Current.GetTitle();
        public string CurrentSubtitle => Current.GetContentTitle();

        public int Buffered => Convert.ToInt32(MediaManager.Buffered.TotalSeconds);
        public int Duration => Convert.ToInt32(MediaManager.Duration.TotalSeconds);
        public int Position => Convert.ToInt32(MediaManager.Position.TotalSeconds);

        public float FloatedPosition => (float)Position / (float)Duration;

        public string TotalDuration => MediaManager.Duration.ToString(@"mm\:ss");
        public string TotalPlayed => MediaManager.Position.ToString(@"mm\:ss");

        private void Current_MediaItemFailed(object sender, MediaItemFailedEventArgs e)
        {
            Log.Debug($"Media item failed: {e.MediaItem.Title}, Message: {e.Message}, Exception: {e.Exeption?.ToString()};");
        }

        private void Current_MediaItemFinished(object sender, MediaItemEventArgs e)
        {
            Log.Debug($"Media item finished: {e.MediaItem.Title};");
        }

        private void Current_MediaItemChanged(object sender, MediaItemEventArgs e)
        {
            Log.Debug($"Media item changed, new item title: {e.MediaItem.Title};");
        }

        private void Current_StatusChanged(object sender, StateChangedEventArgs e)
        {
            Log.Debug($"Status changed: {System.Enum.GetName(typeof(MediaPlayerState), e.State)};");
        }

        private void Current_BufferingChanged(object sender, BufferedChangedEventArgs e)
        {
            Log.Debug($"Total buffered time is {e.Buffered};");
        }

        private void Current_PositionChanged(object sender, PositionChangedEventArgs e)
        {
            Log.Debug($"Current position is {e.Position};");
            RaisePropertyChanged(() => Position);
        }
    }
}
