using System;
using Foundation;
using MediaManager.Library;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding.Views;
using UIKit;

namespace ElementPlayer.iOS.Views.Cells
{
    public partial class MediaCell : MvxTableViewCell
    {
        public static readonly NSString Key = new NSString(nameof(MediaCell));
        public static readonly UINib Nib = UINib.FromName(nameof(MediaCell), NSBundle.MainBundle);

        public MediaCell(IntPtr handle) : base(handle)
        {
            SelectionStyle = UITableViewCellSelectionStyle.None;

            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<MediaCell, MediaItem>();
                set.Bind(lblTitle).To(vm => vm.Title);
                set.Apply();
            });
        }
    }
}

