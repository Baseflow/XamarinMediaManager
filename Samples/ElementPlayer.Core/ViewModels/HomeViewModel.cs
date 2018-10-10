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
using System.Linq;

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
        public MvxObservableCollection<IMediaItem> Items { get; set; } = new MvxObservableCollection<IMediaItem>();

        public IMvxAsyncCommand<IMediaItem> ItemSelected => new MvxAsyncCommand<IMediaItem>(SelectItem);

        public override Task Initialize()
        {
            var json = ExoPlayerSamples.GetEmbeddedResourceString("media.exolist.json");
            var list = ExoPlayerSamples.FromJson(json);

            foreach (var item in list)
            {
                foreach (var sample in item.Samples)
                {
                    Items.Add(new MediaItem(sample.Uri)
                    {
                        Title = sample.Name,
                        Album = item.Name,
                        FileExtension = sample.Extension
                    });
                }                
            }

            return base.Initialize();
        }

        private async Task SelectItem(IMediaItem mediaItem)
        {
            MediaManager.MediaQueue.Clear();
            await MediaManager.Play(mediaItem);

            /*foreach (var item in Items.Except<string>(new[] { url }))
            {
                MediaManager.MediaQueue.Add(new MediaItem(item));
            }*/
        }
    }
}
