// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace ElementPlayer.iOS.Views
{
    [Register ("PlayerViewController")]
    partial class PlayerViewController
    {
        [Outlet]
        UIKit.UIButton btnNext { get; set; }


        [Outlet]
        UIKit.UIButton btnPlayPause { get; set; }


        [Outlet]
        UIKit.UIButton btnPrevious { get; set; }


        [Outlet]
        UIKit.UIButton btnRepeat { get; set; }


        [Outlet]
        UIKit.UIButton btnShuffle { get; set; }


        [Outlet]
        UIKit.UIButton btnStepBackwards { get; set; }


        [Outlet]
        UIKit.UIButton btnStepForward { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIProgressView progressPlayer { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView vwPlayer { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (btnNext != null) {
                btnNext.Dispose ();
                btnNext = null;
            }

            if (btnPlayPause != null) {
                btnPlayPause.Dispose ();
                btnPlayPause = null;
            }

            if (btnPrevious != null) {
                btnPrevious.Dispose ();
                btnPrevious = null;
            }

            if (btnRepeat != null) {
                btnRepeat.Dispose ();
                btnRepeat = null;
            }

            if (btnShuffle != null) {
                btnShuffle.Dispose ();
                btnShuffle = null;
            }

            if (btnStepBackwards != null) {
                btnStepBackwards.Dispose ();
                btnStepBackwards = null;
            }

            if (btnStepForward != null) {
                btnStepForward.Dispose ();
                btnStepForward = null;
            }

            if (progressPlayer != null) {
                progressPlayer.Dispose ();
                progressPlayer = null;
            }

            if (vwPlayer != null) {
                vwPlayer.Dispose ();
                vwPlayer = null;
            }
        }
    }
}