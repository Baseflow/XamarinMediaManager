using System.Threading.Tasks;
using ElementPlayer.Core.Assets;
using MediaManager;
using MediaManager.Media;
using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;

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
        //new MvxAsyncCommand<IMediaItem>(async (item) => await this.NavigationService.Navigate<PlayerViewModel, IMediaItem>(item));

        public override Task Initialize()
        {
            var json = ExoPlayerSamples.GetEmbeddedResourceString("media.exolist.json");
            var list = ExoPlayerSamples.FromJson(json);

            foreach (var item in list)
            {
                foreach (var sample in item.Samples)
                {
                    if (!string.IsNullOrEmpty(sample.Uri))
                    {
                        var mediaItem = new MediaItem(sample.Uri)
                        {
                            Title = sample.Name,
                            Album = item.Name,
                            FileExtension = sample.Extension ?? ""
                        };
                        if (mediaItem.FileExtension == "mpd" || mediaItem.MediaUri.EndsWith(".mpd"))
                            mediaItem.MediaType = MediaType.Dash;
                        else if (mediaItem.FileExtension == "ism" || mediaItem.MediaUri.EndsWith(".ism"))
                            mediaItem.MediaType = MediaType.SmoothStreaming;
                        else if (mediaItem.FileExtension == "m3u8" || mediaItem.MediaUri.EndsWith(".m3u8"))
                            mediaItem.MediaType = MediaType.Hls;

                        Items.Add(mediaItem);
                    }
                }
            }

            return base.Initialize();
        }

        private async Task SelectItem(IMediaItem mediaItem)
        {
            MediaManager.MediaQueue.Clear();

            await this.NavigationService.Navigate<PlayerViewModel, IMediaItem>(mediaItem);

            await MediaManager.Play(mediaItem);

            /*foreach (var item in Items.Except<string>(new[] { url }))
            {
                MediaManager.MediaQueue.Add(new MediaItem(item));
            }*/
        }
    }
}
