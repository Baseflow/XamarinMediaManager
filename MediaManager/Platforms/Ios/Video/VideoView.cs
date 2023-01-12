using System.ComponentModel;
using AVKit;
using CoreGraphics;
using Foundation;
using MediaManager.Video;
using UIKit;

namespace MediaManager.Platforms.Ios.Video
{
    [DesignTimeVisible(true)]
    public partial class VideoView : UIView, IVideoView
    {
        protected MediaManagerImplementation MediaManager => CrossMediaManager.Ios;

        private AVPlayerViewController _playerViewController;
        public AVPlayerViewController PlayerViewController
        {
            get
            {
                if (_playerViewController == null)
                {
                    PlayerViewController = new PlayerViewController();
                }
                return _playerViewController;
            }

            set
            {
                _playerViewController = value;
                if (_playerViewController != null)
                {
                    _playerViewController.View.Frame = Bounds;

#if IOS16_0_OR_GREATER

                    var viewController = GetCurrentUIViewController() ?? throw new InvalidOperationException("ViewController can't be null.");

                    if (viewController.View is null)
                    {
                        throw new NullReferenceException($"{nameof(viewController.View)} cannot be null");
                    }

                    // Zero out the safe area insets of the AVPlayerViewController
                    UIEdgeInsets insets = viewController.View.SafeAreaInsets;
                    _playerViewController.AdditionalSafeAreaInsets =
                        new UIEdgeInsets(insets.Top * -1, insets.Left, insets.Bottom * -1, insets.Right);

                    // Add the View from the AVPlayerViewController to the parent ViewController
                    viewController.View.AddSubview(_playerViewController.View);
#endif
                    AddSubview(_playerViewController.View);
                    (Superview?.NextResponder as UIViewController)?.AddChildViewController(_playerViewController);
                }
            }
        }

        public VideoView()
        {
            InitView();
        }

        public VideoView(NSCoder coder) : base(coder)
        {
            InitView();
        }

        public VideoView(CGRect frame) : base(frame)
        {
            InitView();
        }

        protected VideoView(NSObjectFlag t) : base(t)
        {
        }

        protected internal VideoView(IntPtr handle) : base(handle)
        {
        }

        public virtual void InitView()
        {
            if (MediaManager.MediaPlayer.AutoAttachVideoView)
                MediaManager.MediaPlayer.VideoView = this;
        }

        public UIViewController? GetCurrentUIViewController()
        {
            UIViewController viewController = null;

            var window = GetKeyWindow();

            if (window != null && window.WindowLevel == UIWindowLevel.Normal)
                viewController = window.RootViewController;

            if (viewController == null)
            {
                window = GetWindows()?
                    .OrderByDescending(w => w.WindowLevel)
                    .FirstOrDefault(w => w.RootViewController != null && w.WindowLevel == UIWindowLevel.Normal);

                viewController = window?.RootViewController;
            }

            while (viewController?.PresentedViewController != null)
                viewController = viewController.PresentedViewController;

            return viewController;
        }

        public UIWindow? GetCurrentUIWindow()
        {
            var window = GetKeyWindow();

            if (window != null && window.WindowLevel == UIWindowLevel.Normal)
                return window;

            if (window == null)
            {
                window = GetWindows()?
                    .OrderByDescending(w => w.WindowLevel)
                    .FirstOrDefault(w => w.RootViewController != null && w.WindowLevel == UIWindowLevel.Normal);
            }

            return window;
        }

        static UIWindow? GetKeyWindow()
        {
            // if we have scene support, use that
            if (OperatingSystem.IsIOSVersionAtLeast(13) || OperatingSystem.IsMacCatalystVersionAtLeast(13))
            {
                var scenes = UIApplication.SharedApplication.ConnectedScenes;
                var windowScene = scenes.ToArray<UIWindowScene>().FirstOrDefault();
                return windowScene?.Windows.FirstOrDefault();
            }

            // use the windows property (up to 13.0)
            return UIApplication.SharedApplication.KeyWindow;
        }

        static UIWindow[]? GetWindows()
        {
            // if we have scene support, use that
            if (OperatingSystem.IsIOSVersionAtLeast(13) || OperatingSystem.IsMacCatalystVersionAtLeast(13))
            {
                var scenes = UIApplication.SharedApplication.ConnectedScenes;
                var windowScene = scenes.ToArray<UIWindowScene>().FirstOrDefault();
                return windowScene?.Windows;
            }

            // use the windows property (up to 15.0)
            return UIApplication.SharedApplication.Windows;
        }
    }
}
