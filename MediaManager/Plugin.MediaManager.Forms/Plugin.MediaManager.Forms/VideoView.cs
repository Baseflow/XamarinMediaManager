using Plugin.MediaManager.Abstractions.Implementations;
using Xamarin.Forms;

namespace Plugin.MediaManager.Forms
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

        public string Source
        {
            get { return (string) GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public VideoAspectMode AspectMode
        {
            get { return (VideoAspectMode) GetValue(AspectModeProperty); }
            set { SetValue(AspectModeProperty, value); }
        }

        private static void OnAspectModeChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            CrossMediaManager.Current.VideoPlayer.AspectMode = ((VideoAspectMode) newvalue);
        }

        private static void OnSourceChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            CrossMediaManager.Current.Play((string) newvalue, MediaFileType.VideoUrl);
        }
    }
}