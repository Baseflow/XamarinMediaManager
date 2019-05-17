using System.Collections.Generic;
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
        public IList<string> Mp3UrlList => new[]{
            "https://ia800806.us.archive.org/15/items/Mp3Playlist_555/AaronNeville-CrazyLove.mp3",
            "https://ia800605.us.archive.org/32/items/Mp3Playlist_555/CelineDion-IfICould.mp3",
            "https://ia800605.us.archive.org/32/items/Mp3Playlist_555/Daughtry-Homeacoustic.mp3",
            "https://storage.googleapis.com/uamp/The_Kyoto_Connection_-_Wake_Up/01_-_Intro_-_The_Way_Of_Waking_Up_feat_Alan_Watts.mp3",
            "https://aphid.fireside.fm/d/1437767933/02d84890-e58d-43eb-ab4c-26bcc8524289/d9b38b7f-5ede-4ca7-a5d6-a18d5605aba1.mp3"
            };


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
