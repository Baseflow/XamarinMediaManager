﻿using ElementPlayer.Core.ViewModels;
using MediaManager;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using MvvmCross.Platforms.Ios.Views;
using System;

namespace ElementPlayer.iOS.Views
{
    [MvxChildPresentation]
    [MvxFromStoryboard]
    public partial class PlayerViewController : MvxViewController<PlayerViewModel>
    {
        public PlayerViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            CrossMediaManager.Current.Init();

            CrossMediaManager.Current.MediaPlayer.VideoView = vwPlayer;

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

