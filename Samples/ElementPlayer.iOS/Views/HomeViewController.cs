
using System;
using System.Drawing;
using ElementPlayer.Core.ViewModels;
using Foundation;
using MvvmCross.Platforms.Ios.Views;
using UIKit;

namespace ElementPlayer.iOS.Views
{
    [MvxFromStoryboard(nameof(HomeViewController))]
    public partial class HomeViewController : MvxViewController<HomeViewModel>
    {
        public HomeViewController(IntPtr handle) : base(handle)
        {
        }
    }
}
