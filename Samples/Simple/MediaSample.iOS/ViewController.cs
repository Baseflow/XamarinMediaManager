using System;
using Plugin.MediaManager.Abstractions.Enums;
using MediaManager.Sample.Core;
using Plugin.MediaManager.Abstractions;
using UIKit;
using Plugin.MediaManager.Abstractions.Implementations;
using Foundation;

namespace MediaSample.iOS
{
    public partial class ViewController : UIViewController
    {
        private readonly MediaPlayerViewModel _viewModel;

        private IMediaManager MediaPlayer => _viewModel.MediaPlayer;

        private IPlaybackController PlaybackController => MediaPlayer.PlaybackController;

        public ViewController(IntPtr handle) : base(handle)
        {
            _viewModel = new MediaPlayerViewModel();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            _viewModel.PropertyChanged += (sender, args) =>
            {
                var propertyName = args.PropertyName;
                var allChanged = propertyName == null;

                Func<string, bool> hasChanged = property => allChanged || propertyName == property;

                InvokeOnMainThread(() =>
                {
                    if (hasChanged(nameof(MediaPlayerViewModel.PlayingText)))
                    {
                        QueueLabel.Text = _viewModel.PlayingText;
                    }

                    if (hasChanged(nameof(MediaPlayerViewModel.Cover)))
                    {
                        TrackCoverImageView.Image = (UIImage)_viewModel.Cover;
                    }

                    if (hasChanged(nameof(MediaPlayerViewModel.CurrentTrack)))
                    {
                        TitleLabel.Text = _viewModel.CurrentTrack.Metadata.Title;
                        SubtitleLabel.Text = _viewModel.CurrentTrack.Metadata.Artist;
                    }

                    if (hasChanged(nameof(MediaPlayerViewModel.IsPlaying)))
                    {
                        PlayPauseButton.Selected = _viewModel.IsPlaying;
                    }

                    if (hasChanged(nameof(MediaPlayerViewModel.Duration)))
                    {
                        PlayingProgressSlider.MaxValue = _viewModel.Duration;
                        BufferedProgressSlider.MaxValue = _viewModel.Duration;

                        var duration = TimeSpan.FromSeconds(_viewModel.Duration);

                        TimeTotalLabel.Text = GetTimeString(duration);
                    }

                    if (hasChanged(nameof(MediaPlayerViewModel.Position)))
                    {
                        PlayingProgressSlider.Value = _viewModel.Position;
                        TimePlayedLabel.Text = GetTimeString(TimeSpan.FromSeconds(_viewModel.Position));
                    }

                    if (hasChanged(nameof(MediaPlayerViewModel.Downloaded)))
                    {
                        BufferedProgressSlider.Value = _viewModel.Downloaded;
                    }
                });
            };

            MediaPlayer.PlayingChanged += (sender, args) =>
            {
                if (!_viewModel.IsSeeking)
                {
                    InvokeOnMainThread(() =>
                    {
                        TimePlayedLabel.Text = GetTimeString(args.Position);
                    });
                }
            };

            MediaPlayer.MediaQueue.PropertyChanged += (sender, e) =>
            {
                InvokeOnMainThread(() =>
                {
                    var propertyName = e.PropertyName;
                    var allChanged = propertyName == null;

                    Func<string, bool> hasChanged = property => allChanged || propertyName == property;

                    if (hasChanged(nameof(MediaPlayer.MediaQueue.IsShuffled)))
                    {
                        ShuffleButton.Selected = MediaPlayer.MediaQueue.IsShuffled;
                    }

                    if (hasChanged(nameof(MediaPlayer.MediaQueue.Repeat)))
                    {
                        var iconPrefix = "icon_repeat_";
                        var extension = ".png";

                        string iconState;

                        switch (MediaPlayer.MediaQueue.Repeat)
                        {
                            case RepeatMode.None:
                                iconState = "static";
                                break;

                            case RepeatMode.RepeatAll:
                                iconState = "active";
                                break;

                            case RepeatMode.RepeatOne:
                                iconState = "one_active";
                                break;

                            default:
                                iconState = "static";
                                break;
                        }

                        var imageUrl = iconPrefix + iconState + extension;

                        var image = new UIImage(imageUrl);
                        RepeatButton.SetImage(image, UIControlState.Normal);
                    }
                });
            };

            PlayingProgressSlider.TouchDown += (sender, e) =>
            {
                _viewModel.IsSeeking = true;
            };

            PlayingProgressSlider.TouchUpInside += (sender, e) =>
            {
                _viewModel.IsSeeking = false;
            };

            PlayingProgressSlider.TouchUpOutside += (sender, e) =>
            {
                _viewModel.IsSeeking = false;
            };

            PlayingProgressSlider.ValueChanged += (sender, e) =>
            {
                _viewModel.IsSeeking = true;
                _viewModel.Position = (int)PlayingProgressSlider.Value;
            };

            PlayingProgressSlider.Continuous = true;

            PlayPauseButton.TouchUpInside += async (sender, e) =>
            {
                await PlaybackController.PlayPause();
                PlayPauseButton.Selected = _viewModel.IsPlaying;
            };

            NextButton.TouchUpInside += async (sender, e) =>
            {
                await PlaybackController.PlayNext();
            };

            PreviousButton.TouchUpInside += async (sender, e) =>
            {
                await PlaybackController.PlayPrevious();
            };

            ShuffleButton.TouchUpInside += (sender, args) =>
            {
                PlaybackController.ToggleShuffle();
            };

            RepeatButton.TouchUpInside += (sender, e) =>
            {
                PlaybackController.ToggleRepeat();
            };

            _viewModel.Init();

            var sampleFilePath = NSBundle.MainBundle.PathForResource("local-sample", "mp3");

            _viewModel.Queue.Add(new MediaItem
            {
                Type = MediaItemType.Audio,
                Url = sampleFilePath,
                Availability = ResourceAvailability.Local
            });
        }

        private string GetTimeString(TimeSpan timeSpan)
        {
            return $"{timeSpan.Minutes:00}:{timeSpan.Seconds:00}";
        }
    }
}
