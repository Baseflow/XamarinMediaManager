using System;
using ElementPlayer.Core.ViewModels;
using MediaManager;
using MediaManager.Media;
using MediaManager.Platforms.Ios.Video;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using MvvmCross.Platforms.Ios.Views;
using UIKit;

namespace ElementPlayer.iOS.Views
{
    [MvxChildPresentation]
    [MvxFromStoryboard]
    public partial class PlayerViewController : MvxViewController<PlayerViewModel>
    {
        private VideoSurface _videoSurface;

        public PlayerViewController(IntPtr handle) : base(handle)
        {
        }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            CrossMediaManager.Current.Init();

            _videoSurface = new VideoSurface(vwPlayer);
            CrossMediaManager.Current.MediaPlayer.SetPlayerView(_videoSurface);
            CrossMediaManager.Current.Play(ViewModel.MediaItemToPlay);

            var set = this.CreateBindingSet<PlayerViewController, PlayerViewModel>();
            set.Bind(progressPlayer).To(vm => vm.FloatedPosition);
            set.Bind(btnRepeat).To(vm => vm.ToggleRepeatCommand);
            set.Bind(btnPrevious).To(vm => vm.PlayPreviousCommand);
            set.Bind(btnPlayPause).To(vm => vm.PlayPauseCommand);
            set.Bind(btnNext).To(vm => vm.PlayNextCommand);
            set.Bind(btnShuffle).To(vm => vm.ToggleShuffleCommand);
            set.Bind(btnStepBackwards).To(vm => vm.StepBackwardCommand);
            set.Bind(btnStepForward).To(vm => vm.StepForwardCommand);
            set.Apply();
        }
    }
}

