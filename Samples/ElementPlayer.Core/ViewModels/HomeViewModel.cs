using System.Threading.Tasks;
using MediaManager;
using MediaManager.Media;
using MediaManager.Playback;
using MediaManager.Queue;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using MvvmCross.Logging;
using ElementPlayer.Core.Assets;

namespace ElementPlayer.Core.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        public readonly IMediaManager MediaManager;
        public IMvxAsyncCommand PlayPauseCommand { get; set; }
        public IMvxAsyncCommand StopCommand { get; set; }
        public IMvxAsyncCommand PlayNextCommand { get; set; }
        public IMvxAsyncCommand PlayPreviousCommand { get; set; }
        public IMvxAsyncCommand AddRandomToQueueCommand { get; set; }

        public override string Title => "Home";

        public HomeViewModel(IMvxLogProvider logProvider, IMvxNavigationService navigationService, IMediaManager mediaManager) : base(logProvider, navigationService)
        {
            MediaManager = mediaManager;
            PlayPauseCommand = new MvxAsyncCommand(PlayPause);
            StopCommand = new MvxAsyncCommand(MediaManager.Stop);
            PlayNextCommand = new MvxAsyncCommand(MediaManager.PlayNext);
            PlayPreviousCommand = new MvxAsyncCommand(MediaManager.PlayPrevious);
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
            Log.Debug(string.Format("Media item failed: {0}, Message: {1}, Exception: {2};", e.MediaItem.Title, e.Message, e.Exeption?.ToString()));

        }

        private void Current_MediaItemFinished(object sender, MediaItemEventArgs e)
        {
            Log.Debug(string.Format("Media item finished: {0};", e.MediaItem.Title));

        }

        private void Current_MediaItemChanged(object sender, MediaItemEventArgs e)
        {
            Log.Debug(string.Format("Media item changed, new item title: {0};", e.MediaItem.Title));

        }

        private void Current_StatusChanged(object sender, StateChangedEventArgs e)
        {
            Log.Debug(string.Format("Status changed: {0};", System.Enum.GetName(typeof(MediaPlayerState), e.State)));

        }

        private void Current_BufferingChanged(object sender, BufferingChangedEventArgs e)
        {

            //Log.Debug(string.Format("{0:0.##}% Total buffered time is {1:mm\\:ss};", e.BufferProgress, e.BufferedTime));

        }

        private void Current_PlayingChanged(object sender, PlayingChangedEventArgs e)
        {
            //Log.Debug(string.Format("{0:0.##}% Total played is {1:mm\\:ss} of {2:mm\\:ss};", e.Position, e.Duration));
        }
    }
}
