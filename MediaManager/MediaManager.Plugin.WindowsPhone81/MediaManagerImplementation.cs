using MediaManager.Plugin.Abstractions;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation.Collections;
using Windows.Media.Playback;
using Windows.UI.Xaml;


namespace MediaManager.Plugin
{
  /// <summary>
  /// Implementation for MediaManager
  /// </summary>
  public class MediaManagerImplementation : IMediaManager
  {

      public PlayerStatus Status
      {
          get { throw new NotImplementedException(); }
      }

      public event StatusChangedEventHandler StatusChanged;

      public event CoverReloadedEventHandler CoverReloaded;

      public event PlayingEventHandler Playing;

      public event BufferingEventHandler Buffering;
        public event TrackFinishedEventHandler TrackFinished;

        public async Task Play(string url)
      {
          await Play();
      }

      public async Task Play()
      {
          // Start playing
          if (!this.backgroundTaskRunning)
          {
              var backgroundTaskInitializationResult = this.backgroundTaskInitialized.WaitOne(2000);

              if (backgroundTaskInitializationResult)
              {
                  this.backgroundTaskRunning = true;

                  BackgroundMediaPlayer.Current.Play();
              }
              else
              {
                  // TODO: Show error message to user
                  Debug.WriteLine("Background Audio Task didn't start in the expected time");
              }
          }
          else
          {
              BackgroundMediaPlayer.Current.Play();
          }
      }

      public Task Stop()
      {
          throw new NotImplementedException();
      }

      public Task Pause()
      {
          throw new NotImplementedException();
      }

      public int Position
      {
          get { throw new NotImplementedException(); }
      }

      public int Duration
      {
          get { throw new NotImplementedException(); }
      }

      public int Buffered
      {
          get { throw new NotImplementedException(); }
      }

      public object Cover
      {
          get { throw new NotImplementedException(); }
      }

      public Task Seek(int position)
      {
          throw new NotImplementedException();
      }

      public Task PlayNext(string url)
      {
          throw new NotImplementedException();
      }

      public Task PlayPause()
      {
          throw new NotImplementedException();
      }

      public Task PlayPrevious(string url)
      {
          throw new NotImplementedException();
      }

              /// <summary>
        /// The view model
        /// </summary>
        //private readonly PlayerViewModel viewModel;

        /// <summary>
        /// Indicates whether the background task has been initialized
        /// </summary>
        private readonly AutoResetEvent backgroundTaskInitialized = new AutoResetEvent(false);

        /// <summary>
        /// Indicates whether the background task is running or not
        /// </summary>
        private bool backgroundTaskRunning;

        /// <summary>
        /// The timer that synchronizes the background player position with the viewmodel
        /// </summary>
        private readonly DispatcherTimer positionTimer;

        /// <summary>
        /// The media player
        /// </summary>
        public MediaManagerImplementation mediaPlayer
        {
            get
            {
                return new MediaManagerImplementation();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaPlayerService"/> class.
        /// </summary>
        public MediaManagerImplementation(/*PlayerViewModel viewModel*/)
        {
            //this.viewModel = viewModel;

            this.mediaPlayer.StatusChanged += this.OnStatusChanged;

            this.positionTimer = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(0.1) };
            this.positionTimer.Tick += this.OnPositionTimerTick;

            // Configure background player
            BackgroundMediaPlayer.Current.MediaOpened += this.OnBackgroundMediaPlayerMediaOpened;
            BackgroundMediaPlayer.Current.MediaEnded += this.OnBackgroundMediaPlayerMediaEnded;
            BackgroundMediaPlayer.Current.MediaFailed += this.OnBackgroundMediaPlayerMediaFailed;
            BackgroundMediaPlayer.Current.CurrentStateChanged += this.OnBackgroundMediaPlayerCurrentStateChanged;
            BackgroundMediaPlayer.MessageReceivedFromBackground += this.OnBackgroundMediaPlayerMessageReceivedFromBackground;
            BackgroundMediaPlayer.Current.AutoPlay = false;
        }

        #region Local Events

        /// <summary>
        /// Called when [position timer tick].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void OnPositionTimerTick(object sender, object e)
        {
            if (BackgroundMediaPlayer.Current.CurrentState == MediaPlayerState.Playing)
            {
                //this.viewModel.Position = (int)BackgroundMediaPlayer.Current.Position.TotalSeconds;
            }
        }

        #endregion

        #region ViewModel Events

        /// <summary>
        /// Handles the <see cref="E:ViewModelPropertyChanged" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
        }

        #endregion

        #region IMediaPlayer Events

        /// <summary>
        /// Called when IMediaPlayer [status changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArgs">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void OnStatusChanged(object sender, EventArgs eventArgs)
        {
            if (this.mediaPlayer.Status == PlayerStatus.PLAYING)
            {
                //ThreadPool.RunOnUIThread(
                   // async () =>
                   // {
                        if (BackgroundMediaPlayer.Current.CurrentState == MediaPlayerState.Paused)
                        {
                            BackgroundMediaPlayer.Current.Play();
                        }
                        else
                        {
                            //var file = this.trackMediaHelper.GetFileByMediaType(this.viewModel.CurrentTrack, TrackMediaType.AUDIO);

                            //this.mediaPlayer.Duration = file.Duration;
                            //this.viewModel.RaisePropertyChanged("Duration");

                            var fileUri = ""; //await this.trackMediaHelper.GetAuthenticatedUri(file);

                            BackgroundMediaPlayer.SendMessageToBackground(new ValueSet { { "SetSource", fileUri.ToString() } });
                        }
                   // });
            }
            else if (this.mediaPlayer.Status == PlayerStatus.PAUSED)
            {
                //ThreadPool.RunOnUIThread(
                //    async () =>
                 //   {
                        BackgroundMediaPlayer.Current.Pause();
                 //   });
            }
        }

        #endregion

        #region BackgroundMediaPlayer Events

        /// <summary>
        /// Called when [background media player current state changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The arguments.</param>
        private void OnBackgroundMediaPlayerCurrentStateChanged(MediaPlayer sender, object args)
        {
            //ThreadPool.RunOnUIThread(
             //   () =>
             //   {
                    if (!this.positionTimer.IsEnabled && sender.CurrentState == MediaPlayerState.Playing)
                    {
                        this.positionTimer.Start();
                    }
              //  });
        }

        /// <summary>
        /// Handles the <see cref="E:BackgroundMediaPlayerMediaFailed" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="MediaPlayerFailedEventArgs"/> instance containing the event data.</param>
        private void OnBackgroundMediaPlayerMediaFailed(MediaPlayer sender, MediaPlayerFailedEventArgs args)
        {
            Debug.WriteLine("Failed with error code " + args.ExtendedErrorCode);
        }

        /// <summary>
        /// Handles the <see cref="E:BackgroundMediaPlayerMessageReceivedFromBackground" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="MediaPlayerDataReceivedEventArgs"/> instance containing the event data.</param>
        private void OnBackgroundMediaPlayerMessageReceivedFromBackground(object sender, MediaPlayerDataReceivedEventArgs e)
        {
            foreach (string key in e.Data.Keys)
            {
                switch (key)
                {
                    case "BackgroundTaskStarted":
                        Debug.WriteLine("Background Task started");
                        this.backgroundTaskInitialized.Set();
                        break;
                }
            }
        }

        /// <summary>
        /// Called when [background media player media ended].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The arguments.</param>
        private void OnBackgroundMediaPlayerMediaEnded(MediaPlayer sender, object args)
        {
            Debug.WriteLine("Background Media Player ended playback");
        }

        /// <summary>
        /// Called when [background media player media opened].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The arguments.</param>
        private void OnBackgroundMediaPlayerMediaOpened(MediaPlayer sender, object args)
        {
            Debug.WriteLine("Background Media Player opened the source");

            //ThreadPool.RunOnUIThread(
            this.Play();
            //    );
        }

        #endregion
  }
}