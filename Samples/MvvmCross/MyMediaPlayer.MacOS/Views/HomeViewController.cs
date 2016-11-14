using System;
using System.Collections.Generic;
using System.Linq;
using AppKit;
using Foundation;
using MvvmCross.Mac.Views;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Core.ViewModels;

namespace MyMediaPlayer.MacOS.Views
{
    [MvxViewFor(typeof(Core.ViewModels.HomeViewModel))]
    public partial class HomeViewController : MvxViewController
    {
        #region Constructors

        // Called when created from unmanaged code
        public HomeViewController(IntPtr handle) : base(handle)
        {
            Initialize();
        }

        // Called when created directly from a XIB file
        [Export("initWithCoder:")]
        public HomeViewController(NSCoder coder) : base(coder)
        {
            Initialize();
        }

        // Call to load from the XIB/NIB file
        public HomeViewController() : base()
        {
            Initialize();
        }

        // Shared initialization code
        void Initialize()
        {
        }

        #endregion

        // strongly typed view accessor
        public new FirstView View
        {
            get
            {
                return (FirstView)base.View;
            }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad ();

            var set = this.CreateBindingSet<HomeViewController, Core.ViewModels.HomeViewModel>();
            //set.Bind(textFirst).To(vm => vm.Hello);
            set.Apply();
        }
    }
}
