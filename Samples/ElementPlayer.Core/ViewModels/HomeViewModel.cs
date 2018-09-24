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
        public IMvxAsyncCommand AddRandomToQueueCommand { get; set; }

        public override string Title => "Home";

        public HomeViewModel(IMvxLogProvider logProvider, IMvxNavigationService navigationService, IMediaManager mediaManager) : base(logProvider, navigationService)
        {
            MediaManager = mediaManager;
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

        private void Current_BufferingChanged(object sender, BufferingChangedEventArgs e)
        {
            Log.Debug($"Total buffered time is {e.Buffered};");
        }

        private void Current_PlayingChanged(object sender, PlayingChangedEventArgs e)
        {
            Log.Debug($"Total played is {e.Position} of {e.Duration};");
        }
    }
}
