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
        public HomeViewModel(IMvxLogProvider logProvider, IMvxNavigationService navigationService, IMediaManager mediaManager) : base(logProvider, navigationService)
        {
            MediaManager = mediaManager;
        }

        public override string Title => "Home";
        public readonly IMediaManager MediaManager;
        public MvxObservableCollection<string> Items { get; set; } = new MvxObservableCollection<string>(MediaPlaybackAssets.Mp3UrlList);

        public IMvxAsyncCommand<string> ItemSelected => new MvxAsyncCommand<string>(SelectItem);

        private async Task SelectItem(string url)
        {
            MediaManager.MediaQueue.Clear();
            foreach (var item in Items)
            {
                MediaManager.MediaQueue.Add(new MediaItem(item));
            }
            
            await MediaManager.Play(url);
        }
    }
}
