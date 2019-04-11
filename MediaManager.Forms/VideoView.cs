using System;
using System.Collections.Generic;
using System.Text;
using MediaManager.Media;
using MediaManager.Video;
using Xamarin.Forms;

namespace MediaManager.Forms
{
    public class VideoView : View
    {
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
                typeof(string),
                typeof(VideoView),
                "",
                propertyChanged: OnSourceChanged);

        //TODO: change to object so it can also take IMediaItem
        public string Source
        {
            get { return (string)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public VideoAspectMode AspectMode
        {
            get { return (VideoAspectMode)GetValue(AspectModeProperty); }
            set { SetValue(AspectModeProperty, value); }
        }

        private static void OnAspectModeChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            CrossMediaManager.Current.MediaPlayer.GetPlayerView().VideoAspect = ((VideoAspectMode)newvalue);
        }

        private static void OnSourceChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            switch (newvalue)
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
