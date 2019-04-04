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
    [MvxRootPresentation(WrapInNavigationController = false)]
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

            _videoSurface = new VideoSurface(vwPlayer);
            CrossMediaManager.Current.MediaPlayer.SetPlayerView(_videoSurface);
            CrossMediaManager.Current.Play(ViewModel.MediaItemToPlay);

            var set = this.CreateBindingSet<PlayerViewController, PlayerViewModel>();
            set.Bind(progressPlayer).To(vm => vm.FloatedPosition);
            set.Apply();
        }
    }
}

