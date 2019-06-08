using AppKit;
using Foundation;
using MvvmCross.Forms.Platforms.Mac.Core;

namespace ElementPlayer.Forms.Mac
{
    [Register("AppDelegate")]
    public class AppDelegate : MvxFormsApplicationDelegate<MvxFormsMacSetup<Core.App, FormsApp>, Core.App, FormsApp>
    {
    }
}
