using AppKit;
using Foundation;
using MvvmCross.Core.ViewModels;
using MvvmCross.Mac.Platform;
using MvvmCross.Mac.Views.Presenters;
using MvvmCross.Platform.Platform;

namespace MyMediaPlayer.MacOS
{
    public class Setup : MvxMacSetup
    {
        public Setup(MvxApplicationDelegate applicationDelegate, NSWindow window)
            : base(applicationDelegate, window)
        {
        }

        protected override IMvxApplication CreateApp ()
        {
            return new Core.App();
        }
        
        protected override IMvxTrace CreateDebugTrace()
        {
            return new DebugTrace();
        }
    }
}
