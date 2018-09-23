using System.Threading.Tasks;
using MediaManager;
using MediaManager.Media;
using MediaManager.Playback;
using MediaManager.Queue;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using MvvmCross.Logging;

namespace ElementPlayer.Core
{
    public class HomeViewModel : MvxViewModel
    {
        private IMvxNavigationService _navigationService;
        private readonly IMvxLog _log;
        public IMediaManager MediaManager;
        public IMvxCommand PlayPauseCommand { get; set; }
        public IMvxCommand StopCommand { get; set; }
        public IMvxCommand PlayNextCommand { get; set; }
        public IMvxCommand PlayPreviousCommand { get; set; }
        public IMvxCommand AddRandomToQueueCommand { get; set; }

        public HomeViewModel(IMvxLog log)
        {
            _log = log;
            MediaManager = CrossMediaManager.Current;
            PlayPauseCommand = new MvxAsyncCommand(PlayPause);
            StopCommand = new MvxAsyncCommand(Stop);
            PlayNextCommand = new MvxAsyncCommand(PlayNext);
            PlayPreviousCommand = new MvxAsyncCommand(PlayPrevious);
            AddRandomToQueueCommand = new MvxAsyncCommand(AddRandomToQueue);

            MediaManager.PlayingChanged += Current_PlayingChanged;
            MediaManager.BufferingChanged += Current_BufferingChanged;
            MediaManager.StateChanged += Current_StatusChanged;
            MediaManager.MediaItemChanged += Current_MediaItemChanged;
            MediaManager.MediaItemFinished += Current_MediaItemFinished;
            MediaManager.MediaItemFailed += Current_MediaItemFailed;
        }

        private async Task AddRandomToQueue()
        {
            MediaManager.MediaQueue.Add(
                await MediaManager.MediaExtractor.CreateMediaItem(MediaPlaybackAssets.RandomMp3Url));
        }

        public IMediaQueue MediaQueue => MediaManager.MediaQueue;

        private async Task PlayPrevious()
        {
            await MediaManager.PlayPrevious();
        }

        private async Task PlayNext()
        {
            await MediaManager.PlayNext();
        }

        private async Task Stop()
        {
            await MediaManager.Stop();
        }

        private async Task PlayPause()
        {
            if (MediaQueue.Count == 0)
                await MediaManager.Play(MediaPlaybackAssets.Mp3UrlList);
            else
                await MediaManager.PlayPause();
        }

        /*
        protected override void OnPause()
        {
            base.OnPause();

            CrossMediaManager.Current.PlayingChanged -= Current_PlayingChanged;
            CrossMediaManager.Current.BufferingChanged -= Current_BufferingChanged;
            CrossMediaManager.Current.StateChanged -= Current_StatusChanged;
            CrossMediaManager.Current.MediaItemChanged -= Current_MediaItemChanged;
            CrossMediaManager.Current.MediaItemFinished -= Current_MediaItemFinished;
            CrossMediaManager.Current.MediaItemFailed -= Current_MediaItemFailed;
        }
        */

        private void Current_MediaItemFailed(object sender, MediaItemFailedEventArgs e)
        {
            _log.Debug(string.Format("Media item failed: {0}, Message: {1}, Exception: {2};", e.MediaItem.Title, e.Message, e.Exeption?.ToString()));

        }

        private void Current_MediaItemFinished(object sender, MediaItemEventArgs e)
        {
            _log.Debug(string.Format("Media item finished: {0};", e.MediaItem.Title));

        }

        private void Current_MediaItemChanged(object sender, MediaItemEventArgs e)
        {
            _log.Debug(string.Format("Media item changed, new item title: {0};", e.MediaItem.Title));

        }

        private void Current_StatusChanged(object sender, StateChangedEventArgs e)
        {
            _log.Debug(string.Format("Status changed: {0};", System.Enum.GetName(typeof(MediaPlayerState), e.State)));

        }

        private void Current_BufferingChanged(object sender, BufferingChangedEventArgs e)
        {

            _log.Debug(string.Format("{0:0.##}% Total buffered time is {1:mm\\:ss};", e.BufferProgress, e.BufferedTime));

        }

        private void Current_PlayingChanged(object sender, PlayingChangedEventArgs e)
        {
            _log.Debug(string.Format("{0:0.##}% Total played is {1:mm\\:ss} of {2:mm\\:ss};", e.Progress, e.Position, e.Duration));
        }

    }
}


