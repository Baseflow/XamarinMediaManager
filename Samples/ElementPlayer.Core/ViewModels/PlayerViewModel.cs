using System;
using System.Collections.Generic;
using System.Text;
using MediaManager;
using MediaManager.Media;
using MediaManager.Playback;
using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;

namespace ElementPlayer.Core.ViewModels
{
    public class PlayerViewModel : BaseViewModel
    {
        public PlayerViewModel(IMvxLogProvider logProvider, IMvxNavigationService navigationService, IMediaManager mediaManager) : base(logProvider, navigationService)
        {
        }

        public readonly IMediaManager MediaManager;
        public IMvxAsyncCommand PlayPauseCommand { get; set; }
        public IMvxAsyncCommand StopCommand { get; set; }
        public IMvxAsyncCommand PlayNextCommand { get; set; }
        public IMvxAsyncCommand PlayPreviousCommand { get; set; }

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
