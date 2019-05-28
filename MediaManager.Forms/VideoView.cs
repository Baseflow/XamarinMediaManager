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
        public static readonly BindableProperty VideoAspectProperty =
            BindableProperty.Create(nameof(VideoView),
                typeof(VideoAspectMode),
                typeof(VideoView),
                VideoAspectMode.AspectFit,
                propertyChanged: OnVideoAspectChanged);

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

        public VideoAspectMode VideoAspect
        {
            get { return (VideoAspectMode)GetValue(VideoAspectProperty); }
            set { SetValue(VideoAspectProperty, value); }
        }

        public bool ShowControls
        {
            get { return (bool)GetValue(ShowControlsProperty); }
            set { SetValue(ShowControlsProperty, value); }
        }

        private static void OnShowControlsChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if(PlayerView != null)
                PlayerView.ShowControls = (bool)newValue;
        }

        private static void OnVideoAspectChanged(BindableObject bindable, object oldvalue, object newValue)
        {
            if (PlayerView != null)
                PlayerView.VideoAspect = (VideoAspectMode)newValue;
        }

        private static void OnSourceChanged(BindableObject bindable, object oldvalue, object newValue)
        {
            _ = CrossMediaManager.Current.Play(newValue);
        }
    }
}
