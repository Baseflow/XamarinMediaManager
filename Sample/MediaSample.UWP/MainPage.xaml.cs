using System;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Plugin.MediaManager;
using Plugin.MediaManager.Abstractions;

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
            CrossMediaManager.Current.Playing += CurrentOnPlaying;
            CrossMediaManager.Current.StatusChanged += CurrentOnStatusChanged;
        }

        private async void CurrentOnStatusChanged(object sender, PlayerStatusChangedEventArgs eventArgs)
        {
            await
                CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () =>
                    {
                        PlayerState.Text = Enum.GetName(typeof(PlayerStatus), eventArgs.Status);
                        switch (CrossMediaManager.Current.Status)
                        {
                            case PlayerStatus.STOPPED:
                                Progress.Value = 0;
                                break;
                            case PlayerStatus.PAUSED:
                                break;
                            case PlayerStatus.PLAYING:
                                Progress.Maximum = CrossMediaManager.Current.Duration;
                                break;
                            case PlayerStatus.BUFFERING:
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    });
        }

        private async void CurrentOnPlaying(object sender, PlaybackPositionChangedEventArgs eventArgs)
        {
            await
                CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () =>
                    {
                        Progress.Value = eventArgs.Progress;
                    });
        }

        private async void PlayUrl(object sender, RoutedEventArgs e)
        {
            await CrossMediaManager.Current.Play(@"http://www.montemagno.com/sample.mp3");
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