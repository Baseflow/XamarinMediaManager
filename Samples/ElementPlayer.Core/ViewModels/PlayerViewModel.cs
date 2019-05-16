using System;
using MediaManager;
using MediaManager.Media;
using MediaManager.Playback;
using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;

namespace ElementPlayer.Core.ViewModels
{
    public class PlayerViewModel : BaseViewModel<IMediaItem>
    {
        public PlayerViewModel(IMvxLogProvider logProvider, IMvxNavigationService navigationService, IMediaManager mediaManager) : base(logProvider, navigationService)
        {
            MediaManager = mediaManager;
            PlayPauseCommand = new MvxAsyncCommand(MediaManager.PlayPause);
            PlayNextCommand = new MvxAsyncCommand(MediaManager.PlayNext);
            PlayPreviousCommand = new MvxAsyncCommand(MediaManager.PlayPrevious);
            ToggleShuffleCommand = new MvxCommand(MediaManager.ToggleShuffle);
            ToggleRepeatCommand = new MvxCommand(MediaManager.ToggleRepeat);
            StepForwardCommand = new MvxAsyncCommand(async () => await MediaManager.StepForward());
            StepBackwardCommand = new MvxAsyncCommand(async () => await MediaManager.StepBackward());
            StopCommand = new MvxAsyncCommand(async () => await MediaManager.Stop());

            MediaManager.PlayingChanged += MediaManager_PlayingChanged;
            MediaManager.PositionChanged += Current_PositionChanged;
            mediaManager.StateChanged += MediaManager_StateChanged;
        }

        public IMediaItem MediaItemToPlay;

        public override void Prepare(IMediaItem mediaItem)
        {
            MediaItemToPlay = mediaItem;
        }

        private void MediaManager_PlayingChanged(object sender, PlayingChangedEventArgs e)
        {
            RaiseAllPropertiesChanged();
        }

        public override string Title => "Player";

        public readonly IMediaManager MediaManager;

        public IMvxAsyncCommand StopCommand { get; }
        public IMvxAsyncCommand PlayPauseCommand { get; }
        public IMvxAsyncCommand PlayNextCommand { get; }
        public IMvxAsyncCommand PlayPreviousCommand { get; }
        public IMvxCommand ToggleShuffleCommand { get; }
        public IMvxCommand ToggleRepeatCommand { get; }
        public IMvxCommand StepForwardCommand { get; }
        public IMvxCommand StepBackwardCommand { get; }

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

        private void MediaManager_StateChanged(object sender, StateChangedEventArgs e)
        {
            Log.Debug($"Status changed: {System.Enum.GetName(typeof(MediaPlayerState), e.State)};");
        }

        private void Current_BufferingChanged(object sender, BufferingChangedEventArgs e)
        {
            Log.Debug($"Total buffered time is {e.Buffered};");
        }

        private void Current_PlayingChanged(object sender, PlayingChangedEventArgs e)
        {
            Log.Debug($"Total played is {e.Position} of {e.Duration};");
        }

        private void Current_PositionChanged(object sender, PositionChangedEventArgs e)
        {
            Log.Debug($"Current position is {e.Position};");
            RaisePropertyChanged(() => Position);
        }
    }
}
