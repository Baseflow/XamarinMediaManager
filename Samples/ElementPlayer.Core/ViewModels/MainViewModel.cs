using MediaManager;
using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;

namespace ElementPlayer.Core.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public MainViewModel(IMvxLogProvider logProvider, IMvxNavigationService navigationService) : base(logProvider, navigationService)
        {
            CrossMediaManager.Current.StateChanged += Current_StateChanged;
        }

        private void Current_StateChanged(object sender, MediaManager.Playback.StateChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(IsPlaying));
        }

        public IMvxAsyncCommand PlayerCommand => new MvxAsyncCommand(() => NavigationService.Navigate<PlayerViewModel>());

        public bool IsPlaying => CrossMediaManager.Current.IsPlaying();
    }
}
