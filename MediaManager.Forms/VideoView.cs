using System;
using System.Collections.Generic;
using MediaManager.Media;
using MediaManager.Video;
using Xamarin.Forms;

namespace MediaManager.Forms
{
    public class VideoView : View
    {
        protected static IVideoView PlayerView => CrossMediaManager.Current.MediaPlayer?.VideoView;

        /// <summary>
        ///     Sets the aspect mode of the current video view
        /// </summary>
        public static readonly BindableProperty AspectModeProperty =
            BindableProperty.Create(nameof(VideoView),
                typeof(VideoAspectMode),
                typeof(VideoView),
                VideoAspectMode.AspectFill,
                propertyChanged: OnAspectModeChanged);

        /// <summary>
        ///     Sets the aspect mode of the current video view
        /// </summary>
        public static readonly BindableProperty SourceProperty =
            BindableProperty.Create(nameof(VideoView),
                typeof(object),
                typeof(VideoView),
                "",
                propertyChanged: OnSourceChanged);

        /// <summary>
        ///     Sets the aspect mode of the current video view
        /// </summary>
        public static readonly BindableProperty ShowControlsProperty =
            BindableProperty.Create(nameof(VideoView),
                typeof(bool),
                typeof(VideoView),
                true,
                propertyChanged: OnShowControlsChanged);

        public object Source
        {
            get { return (object)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public VideoAspectMode AspectMode
        {
            get { return (VideoAspectMode)GetValue(AspectModeProperty); }
            set { SetValue(AspectModeProperty, value); }
        }

        public bool ShowControls
        {
            get { return (bool)GetValue(AspectModeProperty); }
            set { SetValue(AspectModeProperty, value); }
        }

        private static void OnShowControlsChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if(PlayerView != null)
                PlayerView.ShowControls = (bool)newValue;
        }

        private static void OnAspectModeChanged(BindableObject bindable, object oldvalue, object newValue)
        {
            if (PlayerView != null)
                PlayerView.VideoAspect = (VideoAspectMode)newValue;
        }

        private static void OnSourceChanged(BindableObject bindable, object oldvalue, object newValue)
        {
            switch (newValue)
            {
                case string url:
                    CrossMediaManager.Current.Play(url);
                    break;
                case IEnumerable<string> urls:
                    CrossMediaManager.Current.Play(urls);
                    break;
                case IMediaItem mediaItem:
                    CrossMediaManager.Current.Play(mediaItem);
                    break;
                case IEnumerable<IMediaItem> mediaItems:
                    CrossMediaManager.Current.Play(mediaItems);
                    break;
                default:
                    break;
            }
        }
    }
}
