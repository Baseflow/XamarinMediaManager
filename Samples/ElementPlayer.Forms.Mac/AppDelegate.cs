using Foundation;
using MediaManager;
using MvvmCross.Forms.Platforms.Mac.Core;

namespace ElementPlayer.Forms.Mac
{
    [Register("AppDelegate")]
    public class AppDelegate : MvxFormsApplicationDelegate<MvxFormsMacSetup<Core.App, FormsApp>, Core.App, FormsApp>
    {
        protected override void RegisterSetup()
        {
            CrossMediaManager.Current.Init();

            base.RegisterSetup();
        }
    }
}
