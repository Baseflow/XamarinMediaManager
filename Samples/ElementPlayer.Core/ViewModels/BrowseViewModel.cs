using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ElementPlayer.Core.Assets;
using MediaManager;
using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;

namespace ElementPlayer.Core.ViewModels
{
    public class BrowseViewModel : BaseViewModel
    {
        private readonly IMediaManager mediaManager;

        public BrowseViewModel(IMvxLogProvider logProvider, IMvxNavigationService navigationService, IMediaManager mediaManager) : base(logProvider, navigationService)
        {
            this.mediaManager = mediaManager;
        }

        public override string Title => "Browse";

        public MvxObservableCollection<string> Items { get; set; } = new MvxObservableCollection<string>(MediaPlaybackAssets.Mp3UrlList);

        public IMvxAsyncCommand<string> ItemSelected => new MvxAsyncCommand<string>(SelectItem);

        private async Task SelectItem(string url)
        {
            await mediaManager.Play(url);
        }
    }
}
