using MediaManager;
using MediaManager.Platforms.Ios.Video;

namespace IosPlayerSample;

[Register ("AppDelegate")]
public class AppDelegate : UIApplicationDelegate {
    public override UIWindow? Window {
        get;
        set;
    }

    public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
    {

        CrossMediaManager.Current.Init();
        CrossMediaManager.Current.MediaPlayer.ShowPlaybackControls = true;
        CrossMediaManager.Current.AutoPlay = true;

        // create a new window instance based on the screen size
        Window = new UIWindow (UIScreen.MainScreen.Bounds);

        // create a UIViewController with a single UILabel
        var vc = new UIViewController ();
        Window.RootViewController = vc;

        var videoView = new VideoView(Window!.Frame)
        {
            AutoresizingMask = UIViewAutoresizing.All,
        };

        vc.View!.AddSubview(videoView);
        //Window.RootViewController = vc;

        // make the window visible
        Window.MakeKeyAndVisible ();

        CrossMediaManager.Current.MediaPlayer.VideoView = videoView;
        CrossMediaManager.Current.Play("http://clips.vorwaerts-gmbh.de/big_buck_bunny.mp4");

        return true;
    }
}

