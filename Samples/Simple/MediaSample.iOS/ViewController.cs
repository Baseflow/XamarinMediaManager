using System;
using MediaManager.Sample.Core;
using UIKit;

namespace MediaSample.iOS
{
	public partial class ViewController : UIViewController
	{
		private readonly MediaPlayerViewModel _viewModel;
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
		            if (hasChanged(nameof(MediaPlayerViewModel.Cover)))
		            {
		                QueueLabel.Text = _viewModel.PlayingText;
		            }

		            if (hasChanged(nameof(MediaPlayerViewModel.Cover)))
		            {
		                TrackCoverImageView.Image = (UIImage) _viewModel.Cover;
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

		    _viewModel.MediaPlayer.PlayingChanged += (sender, args) =>
		    {
				if (!_viewModel.IsSeeking)
				{
					InvokeOnMainThread(() =>
					{
						TimePlayedLabel.Text = GetTimeString(args.Position);
					});
				}
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
				_viewModel.Position = (int) PlayingProgressSlider.Value;
			};

			PlayingProgressSlider.Continuous = true;

		    PlayPauseButton.TouchUpInside += async (sender, e) =>
			{
				await _viewModel.MediaPlayer.PlayPause();
				PlayPauseButton.Selected = _viewModel.IsPlaying;
			};

			NextButton.TouchUpInside += async (sender, e) =>
			{
			    await _viewModel.MediaPlayer.PlayNext();
			};

			PreviousButton.TouchUpInside += async (sender, e) =>
			{
				await _viewModel.MediaPlayer.PlayPrevious();
			};

		    ShuffleButton.TouchUpInside += (sender, args) =>
		    {
				ShuffleButton.Selected = _viewModel.MediaPlayer.MediaQueue.IsShuffled;
                _viewModel.MediaPlayer.MediaQueue.ToggleShuffle();
		    };

		    _viewModel.Init();
		}

	    private string GetTimeString(TimeSpan timeSpan)
	    {
	        return $"{timeSpan.Minutes:00}:{timeSpan.Seconds:00}";
	    }
	}
}
