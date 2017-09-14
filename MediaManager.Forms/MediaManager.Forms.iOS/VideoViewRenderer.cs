using System;
using Foundation;
using Plugin.MediaManager.Forms;
using Plugin.MediaManager.Forms.iOS;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(VideoView), typeof(VideoViewRenderer))]
namespace Plugin.MediaManager.Forms.iOS
{
    [Preserve(AllMembers = true)]
    public class VideoViewRenderer : ViewRenderer<VideoView, VideoSurface> 
    {
        private VideoSurface _videoSurface;

        /// <summary>
        /// Used for registration with dependency service
        /// </summary>
        public async static void Init()
        {
            var temp = DateTime.Now;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<VideoView> e)
        {
            base.OnElementChanged(e);
            if (Control == null)
            {                
                _videoSurface = new VideoSurface();
                SetNativeControl(_videoSurface);
                CrossMediaManager.Current.VideoPlayer.RenderSurface = _videoSurface;
            }

            //ltang: Walk-around for https://github.com/martijn00/XamarinMediaManager/issues/250
            if (e.OldElement != null)
            {
                // Unsubscribe from event handlers and cleanup any resources
                RemoveGestureTap();
            }

            if (e.NewElement != null)
            {
                // Configure the control and subscribe to event handlers
                AttachGestureTap(e.NewElement);
            }
        }

        #region Walk-around for https://github.com/martijn00/XamarinMediaManager/issues/250
        private UITapGestureRecognizer _tapGesture = null;
        private void AttachGestureTap(VideoView view)
        {
            if (_videoSurface == null || _tapGesture != null)
                return;

            TapGestureRecognizer tapGestureToTransfer = null;
            foreach (var gesture in view.GestureRecognizers)
            {
                if (gesture is TapGestureRecognizer)
                {
                    tapGestureToTransfer = gesture as TapGestureRecognizer;
                    break;
                }
            }

            if (tapGestureToTransfer?.Command != null)
            {
                _tapGesture = new UITapGestureRecognizer(() =>
                {
                    tapGestureToTransfer.Command.Execute(tapGestureToTransfer.CommandParameter);
                });
                _tapGesture.NumberOfTapsRequired = (nuint)tapGestureToTransfer.NumberOfTapsRequired;

                _videoSurface.AddGestureRecognizer(_tapGesture);
            }
        }

        private void RemoveGestureTap()
        {
            if (_videoSurface == null || _tapGesture == null)
                return;

            _videoSurface.RemoveGestureRecognizer(_tapGesture);
            _tapGesture = null;
        }
        #endregion
    }
}
