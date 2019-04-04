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
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIProgressView progressPlayer { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView vwPlayer { get; set; }

        void ReleaseDesignerOutlets ()
        {
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