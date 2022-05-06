using System.Threading.Tasks;
using MediaManager;
using MvvmCross.Commands;
using MvvmCross.Navigation;

namespace ElementPlayer.Core.ViewModels
{
    public class PlaylistsViewModel : BaseViewModel
    {

        public PlaylistsViewModel(IMvxLogProvider logProvider, IMvxNavigationService navigationService, IMediaManager mediaManager) : base(logProvider, navigationService)
        {
            MediaManager = mediaManager;
        }

        public IMediaManager MediaManager { get; }

        public override string Title => "Playlists";

        public IMvxAsyncCommand<string> ItemSelected => new MvxAsyncCommand<string>(SelectItem);

        private async Task SelectItem(string url)
        {
            //TODO: Play Index
        }
    }
}
