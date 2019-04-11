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
    [Register ("HomeViewController")]
    partial class HomeViewController
    {
        [Outlet]
        UIKit.UITableView tblItems { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (tblItems != null) {
                tblItems.Dispose ();
                tblItems = null;
            }
        }
    }
}