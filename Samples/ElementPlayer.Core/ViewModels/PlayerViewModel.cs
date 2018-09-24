using System;
using System.Collections.Generic;
using System.Text;
using MediaManager;
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
    }
}
