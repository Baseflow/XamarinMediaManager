using System;
using System.Diagnostics;
using System.Linq;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Plugin.MediaManager;
using Plugin.MediaManager.Abstractions.Enums;
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
            CrossMediaManager.Current.BufferingChanged += OnBufferingChanged;
            CrossMediaManager.Current.StatusChanged += OnStatusChanged;
            CrossMediaManager.Current.VideoPlayer.RenderSurface = VideoCanvas;
            CrossMediaManager.Current.MediaFileChanged += CurrentOnMediaFileChanged;
        }

        private void OnBufferingChanged(object sender, BufferingChangedEventArgs bufferingChangedEventArgs)
        {
            var bufferingProgress = bufferingChangedEventArgs?.BufferProgress ?? 0;
            var bufferingTime = bufferingChangedEventArgs?.BufferedTime;
            Debug.WriteLine($"buffering progress: {bufferingProgress}, buffering time: {bufferingTime}");
        }

        private void CurrentOnMediaFileChanged(object sender, MediaFileChangedEventArgs mediaFileChangedEventArgs)
        {
            var mediaFile = mediaFileChangedEventArgs.File;
            Title.Text = mediaFile.Metadata.Title ?? "";
            Artist.Text = mediaFile.Metadata.Artist ?? "";
            Album.Text = mediaFile.Metadata.Album ?? "";
            switch (mediaFile.Type)
            {
                case MediaFileType.Audio:
                    if (mediaFile.Metadata.AlbumArt!= null)
                    {
                        CoverArt.Source = (ImageSource) mediaFile.Metadata.AlbumArt;
                    }
                    break;
                case MediaFileType.Video:
                    break;
            }
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

        private double CurrentStreamingPosition = 0;
        private async void OnPlayingChanged(object sender, PlayingChangedEventArgs e)
        {
            await
                CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () =>
                    {
                        Progress.Value = e.Progress;
                        CurrentStreamingPosition = e.Position.TotalSeconds;
                    });
        }

        private MediaFile mediaFile;
        private async void PlayUrl(object sender, RoutedEventArgs e)
        {
            if (mediaFile == null)
            {
                mediaFile = new MediaFile("http://www.montemagno.com/sample.mp3", MediaFileType.Audio);
            }
            await CrossMediaManager.Current.Play(mediaFile);
            //var file = await KnownFolders.VideosLibrary.GetFileAsync("big_buck_bunny.mp4");
            //await CrossMediaManager.Current.Play(file.Path, MediaFileType.VideoFile);
            //await CrossMediaManager.Current.Play(@"http://clips.vorwaerts-gmbh.de/big_buck_bunny.mp4", MediaFileType.VideoUrl);
        }

        private async void Pause(object sender, RoutedEventArgs e)
        {
            await CrossMediaManager.Current.Pause();
        }

        private async void Stop(object sender, RoutedEventArgs e)
        {
            await CrossMediaManager.Current.Stop();
        }

        private async void Skip10Seconds(object sender, RoutedEventArgs e)
        {
            if (CurrentStreamingPosition > 0)
            {
                Debug.WriteLine($"Before Skipping 10 seconds, Current Position: {CurrentStreamingPosition}");
                CurrentStreamingPosition += 10;
                await CrossMediaManager.Current.PlaybackController.SeekTo(CurrentStreamingPosition);
                await CrossMediaManager.Current.PlaybackController.Play();
                Debug.WriteLine($"After Skipping 10 seconds, Current Position: {CurrentStreamingPosition}");
            }
        }

        private async void Skip30Seconds(object sender, RoutedEventArgs e)
        {
            if (CurrentStreamingPosition > 0)
            {
                Debug.WriteLine($"Before Skipping 30 seconds, Current Position: {CurrentStreamingPosition}");
                CurrentStreamingPosition += 30;
                await CrossMediaManager.Current.PlaybackController.SeekTo(CurrentStreamingPosition);
                await CrossMediaManager.Current.PlaybackController.Play();
                Debug.WriteLine($"After Skipping 30 seconds, Current Position: {CurrentStreamingPosition}");
            }
        }
    }
}