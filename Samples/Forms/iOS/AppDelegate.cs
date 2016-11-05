using Foundation;
using Plugin.MediaManager.Forms.iOS;
using UIKit;

namespace MediaForms.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            VideoViewRenderer.Init();
            global::Xamarin.Forms.Forms.Init();

            LoadApplication(new App());
            
            return base.FinishedLaunching(app, options);
        }
    }
}
