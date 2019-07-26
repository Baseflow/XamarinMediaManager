using System;
using MediaManager.Media;
using MediaManager.Playback;
using MediaManager.Player;
using MediaManager.Queue;
using MediaManager.Video;
using Xamarin.Forms;

namespace MediaManager.Forms
{
    public class VideoView : View, IDisposable
    {
        protected static IMediaManager MediaManager => CrossMediaManager.Current;
        protected static IVideoView PlayerView => MediaManager.MediaPlayer.VideoView;

        public VideoView()
        {
            MediaManager.BufferedChanged += MediaManager_BufferedChanged;
            MediaManager.PositionChanged += MediaManager_PositionChanged;
            MediaManager.StateChanged += MediaManager_StateChanged;
            MediaManager.MediaItemChanged += MediaManager_MediaItemChanged;

            MediaManager.PropertyChanged += MediaManager_PropertyChanged;
        }

        private void MediaManager_MediaItemChanged(object sender, MediaItemEventArgs e)
        {
            if (Source != e.MediaItem)
                Source = e.MediaItem;
        }

        private void MediaManager_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                //TODO: This seems not to be triggered
                case nameof(MediaManager.Duration):
                    Duration = MediaManager.Duration;
                    break;
                default:
                    break;
            }
        }

        private void MediaManager_StateChanged(object sender, StateChangedEventArgs e)
        {
            State = e.State;
        }

        private void MediaManager_PositionChanged(object sender, PositionChangedEventArgs e)
        {
            Position = e.Position;
        }

        private void MediaManager_BufferedChanged(object sender, BufferedChangedEventArgs e)
        {
            Buffered = e.Buffered;
        }

        public static readonly BindableProperty VideoAspectProperty =
            BindableProperty.Create(nameof(VideoAspect), typeof(VideoAspectMode), typeof(VideoView), VideoAspectMode.AspectFit, propertyChanged: OnVideoAspectPropertyChanged);

        public static readonly BindableProperty AutoPlayProperty =
            BindableProperty.Create(nameof(AutoPlay), typeof(bool), typeof(VideoView), true);

        public static readonly BindableProperty BufferedProperty =
            BindableProperty.Create(nameof(Buffered), typeof(TimeSpan), typeof(VideoView), TimeSpan.Zero, defaultValueCreator: x => MediaManager.Buffered);

        public static readonly BindableProperty StateProperty =
            BindableProperty.Create(nameof(State), typeof(MediaPlayerState), typeof(VideoView), MediaPlayerState.Stopped, defaultValueCreator: x => MediaManager.State);

        public static readonly BindableProperty DurationProperty =
            BindableProperty.Create(nameof(Duration), typeof(TimeSpan), typeof(VideoView), null, defaultValueCreator: x => MediaManager.Duration);

        public static readonly BindableProperty PositionProperty =
            BindableProperty.Create(nameof(Position), typeof(TimeSpan), typeof(VideoView), TimeSpan.Zero, defaultValueCreator: x => MediaManager.Position);

        public static readonly BindableProperty ShowControlsProperty =
            BindableProperty.Create(nameof(ShowControls), typeof(bool), typeof(VideoView), false, propertyChanged: OnShowControlsPropertyChanged);

        public static readonly BindableProperty SourceProperty =
            BindableProperty.Create(nameof(Source), typeof(object), typeof(VideoView), propertyChanged: OnSourcePropertyChanged);

        public static readonly BindableProperty RepeatProperty =
            BindableProperty.Create(nameof(Repeat), typeof(RepeatMode), typeof(VideoView), RepeatMode.Off, propertyChanged: OnRepeatPropertyChanged, defaultValueCreator: x => MediaManager.RepeatMode);

        public static readonly BindableProperty ShuffleProperty =
            BindableProperty.Create(nameof(Shuffle), typeof(ShuffleMode), typeof(VideoView), ShuffleMode.Off, propertyChanged: OnShufflePropertyChanged, defaultValueCreator: x => MediaManager.ShuffleMode);

        public static readonly BindableProperty VideoHeightProperty =
            BindableProperty.Create(nameof(VideoHeight), typeof(int), typeof(VideoView));

        public static readonly BindableProperty VideoWidthProperty =
            BindableProperty.Create(nameof(VideoWidth), typeof(int), typeof(VideoView));

        public static readonly BindableProperty VolumeProperty =
            BindableProperty.Create(nameof(Volume), typeof(int), typeof(VideoView), 1, defaultValueCreator: x => MediaManager.VolumeManager.CurrentVolume);

        public static readonly BindableProperty SpeedProperty =
            BindableProperty.Create(nameof(Speed), typeof(float), typeof(VideoView), 1.0f, defaultValueCreator: x => MediaManager.Speed);

        public VideoAspectMode VideoAspect
        {
            get => (VideoAspectMode)GetValue(VideoAspectProperty);
            set => SetValue(VideoAspectProperty, value);
        }

        public RepeatMode Repeat
        {
            get => (RepeatMode)GetValue(RepeatProperty);
            set => SetValue(RepeatProperty, value);
        }

        public ShuffleMode Shuffle
        {
            get => (ShuffleMode)GetValue(ShuffleProperty);
            set => SetValue(ShuffleProperty, value);
        }

        public bool AutoPlay
        {
            get { return (bool)GetValue(AutoPlayProperty); }
            set { SetValue(AutoPlayProperty, value); }
        }

        public TimeSpan Buffered
        {
            get { return (TimeSpan)GetValue(BufferedProperty); }
            internal set { SetValue(BufferedProperty, value); }
        }

        public MediaPlayerState State
        {
            get { return (MediaPlayerState)GetValue(StateProperty); }
            internal set { SetValue(StateProperty, value); }
        }

        public TimeSpan Duration
        {
            get { return (TimeSpan)GetValue(DurationProperty); }
            internal set { SetValue(DurationProperty, value); }
        }

        public bool ShowControls
        {
            get { return (bool)GetValue(ShowControlsProperty); }
            set { SetValue(ShowControlsProperty, value); }
        }

        public TimeSpan Position
        {
            get { return (TimeSpan)GetValue(PositionProperty); }
            internal set
            {
                SetValue(PositionProperty, value);
            }
        }

        public object Source
        {
            get { return (object)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public int VideoHeight
        {
            get { return (int)GetValue(VideoHeightProperty); }
        }

        public int VideoWidth
        {
            get { return (int)GetValue(VideoWidthProperty); }
        }

        public int Volume
        {
            get { return (int)GetValue(VolumeProperty); }
            set { SetValue(VolumeProperty, value); }
        }

        public float Speed
        {
            get { return (float)GetValue(SpeedProperty); }
            set { SetValue(SpeedProperty, value); }
        }

        private static async void OnSourcePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            //Prevent loop with MediaManager_MediaItemChanged
            if (MediaManager.MediaQueue.Current == newValue)
                return;

            //TODO: Check for AutoPlay
            await CrossMediaManager.Current.Play(newValue);
        }

        private static void OnShowControlsPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (PlayerView != null)
                PlayerView.ShowControls = (bool)newValue;
        }

        private static void OnVideoAspectPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (PlayerView != null)
                PlayerView.VideoAspect = (VideoAspectMode)newValue;
        }

        private static void OnRepeatPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            MediaManager.RepeatMode = (RepeatMode)newValue;
        }

        private static void OnShufflePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            MediaManager.ShuffleMode = (ShuffleMode)newValue;
        }

        public void Dispose()
        {
            MediaManager.BufferedChanged -= MediaManager_BufferedChanged;
            MediaManager.PositionChanged -= MediaManager_PositionChanged;
            MediaManager.StateChanged -= MediaManager_StateChanged;
            MediaManager.MediaItemChanged -= MediaManager_MediaItemChanged;

            MediaManager.PropertyChanged -= MediaManager_PropertyChanged;
        }
    }
}
