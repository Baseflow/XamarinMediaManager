using System;
using ElementPlayer.Core.ViewModels;
using ElementPlayer.iOS.Views.Cells;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding.Views;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using MvvmCross.Platforms.Ios.Views;

namespace ElementPlayer.iOS.Views
{
    [MvxRootPresentation(WrapInNavigationController = true)]
    [MvxFromStoryboard]
    public partial class HomeViewController : MvxViewController<HomeViewModel>
    {
        public HomeViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.

            var source = new MvxSimpleTableViewSource(tblItems, MediaCell.Key, MediaCell.Key);
            source.DeselectAutomatically = true;
            tblItems.Source = source;

            var set = this.CreateBindingSet<HomeViewController, HomeViewModel>();
            set.Bind(source).To(vm => vm.Items);
            set.Bind(source).For(v => v.SelectionChangedCommand).To(vm => vm.ItemSelected);
            set.Apply();
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}

