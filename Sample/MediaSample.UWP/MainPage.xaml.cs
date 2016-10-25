using System;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Plugin.MediaManager;
using Plugin.MediaManager.Abstractions.EventArguments;
using Plugin.MediaManager.Abstractions.Implementations;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MediaSample.UWP
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            CrossMediaManager.Current.PlayingChanged += OnPlayingChanged;
            CrossMediaManager.Current.StatusChanged += OnStatusChanged;
            CrossMediaManager.Current.VideoPlayer.SetVideoSurface(VideoCanvas);
        }

        private async void OnStatusChanged(object sender, StatusChangedEventArgs e)
        {
            await
                CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () =>
                    {
                        PlayerState.Text = Enum.GetName(typeof(MediaPlayerStatus), e.Status);
                        switch (CrossMediaManager.Current.Status)
                        {
                            case MediaPlayerStatus.Stopped:
                                Progress.Value = 0;
                                break;
                            case MediaPlayerStatus.Paused:
                                break;
                            case MediaPlayerStatus.Playing:
                                Progress.Maximum = 1;
                                break;
                            case MediaPlayerStatus.Buffering:
                                break;
                                case MediaPlayerStatus.Loading:
                                break;
                            case MediaPlayerStatus.Failed:
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    });
        }

        private async void OnPlayingChanged(object sender, PlayingChangedEventArgs e)
        {
            await
                CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () =>
                    {
                        Progress.Value = e.Progress;
                    });
        }

        private async void PlayUrl(object sender, RoutedEventArgs e)
        {
            //await CrossMediaManager.Current.Play(@"http://www.montemagno.com/sample.mp3", MediaFileType.AudioUrl);
            await CrossMediaManager.Current.Play(@"C:\Users\erlend\Videos\IMG_0140.mov", MediaFileType.VideoFile);
        }

        private async void Pause(object sender, RoutedEventArgs e)
        {
            await CrossMediaManager.Current.Pause();
        }

        private async void Stop(object sender, RoutedEventArgs e)
        {
            await CrossMediaManager.Current.Stop();
        }
    }
}