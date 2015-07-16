using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Foundation.Collections;
using Windows.Media;
using Windows.Media.Playback;

namespace MediaManager.Plugin
{
    public sealed class BackgroundMediaPlayerTask : IBackgroundTask
    {
        /// <summary>
        /// The deferral.
        /// </summary>
        private BackgroundTaskDeferral deferral;

        /// <summary>
        /// Indicates whether the task is running
        /// </summary>
        private bool backgroundTaskRunning;

        /// <summary>
        /// The system media transport control.
        /// </summary>
        private SystemMediaTransportControls systemMediaTransportControl;

        //private ForegroundAppStatus foregroundAppState = ForegroundAppStatus.Unknown;

        /// <summary>
        /// 
        /// </summary>
        private readonly AutoResetEvent backgroundTaskStarted = new AutoResetEvent(false);

        /// <summary>
        /// Performs the work of a background task. The system calls this method when the associated
        /// background task has been triggered.
        /// </summary>
        /// <param name="taskInstance">
        /// An interface to an instance of the background task. The system creates this instance when the
        /// task has been triggered to run.
        /// </param>
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            Debug.WriteLine("Background Audio Task " + taskInstance.Task.Name + " starting");

            this.systemMediaTransportControl = SystemMediaTransportControls.GetForCurrentView();
            this.systemMediaTransportControl.IsEnabled = true;
            this.systemMediaTransportControl.IsPauseEnabled = true;
            this.systemMediaTransportControl.IsPlayEnabled = true;
            this.systemMediaTransportControl.IsNextEnabled = true;
            this.systemMediaTransportControl.IsPreviousEnabled = true;

            // Wire up system media control events
            this.systemMediaTransportControl.ButtonPressed += this.OnSystemMediaTransportControlButtonPressed;
            this.systemMediaTransportControl.PropertyChanged += this.OnSystemMediaTransportControlPropertyChanged;

            // Wire up background task events
            taskInstance.Canceled += this.OnTaskCanceled;
            taskInstance.Task.Completed += this.OnTaskcompleted;

            // Initialize message channel 
            BackgroundMediaPlayer.MessageReceivedFromForeground += this.OnMessageReceivedFromForeground;

            // Notify foreground that we have started playing
            BackgroundMediaPlayer.SendMessageToForeground(new ValueSet() { { "BackgroundTaskStarted", "" } });
            this.backgroundTaskStarted.Set();
            this.backgroundTaskRunning = true;

            this.deferral = taskInstance.GetDeferral();
        }

        private void OnSystemMediaTransportControlPropertyChanged(SystemMediaTransportControls sender, SystemMediaTransportControlsPropertyChangedEventArgs args)
        {
            // TODO: Pause playback when muted
        }

        private void OnSystemMediaTransportControlButtonPressed(SystemMediaTransportControls sender, SystemMediaTransportControlsButtonPressedEventArgs args)
        {
            switch (args.Button)
            {
                case SystemMediaTransportControlsButton.Play:
                    Debug.WriteLine("UVC play button pressed");

                    if (!this.backgroundTaskRunning)
                    {
                        var result = this.backgroundTaskStarted.WaitOne(2000);

                        if (!result)
                        {
                            throw new Exception("Background Task did not initialize in time");
                        }
                    }

                    // TODO
                    BackgroundMediaPlayer.Current.Play();

                    break;
                case SystemMediaTransportControlsButton.Pause:
                    Debug.WriteLine("UVC pause button pressed");

                    try
                    {
                        // TODO
                        BackgroundMediaPlayer.Current.Pause();
                    }
                    catch (Exception ex)
                    {
                        // TODO: Must send exception to app and the app must report to Insights
                    }

                    break;
                case SystemMediaTransportControlsButton.Next:
                    Debug.WriteLine("UVC next button pressed");

                    break;
                case SystemMediaTransportControlsButton.Previous:
                    Debug.WriteLine("UVC previous button pressed");

                    break;
            }
        }

        private void OnMessageReceivedFromForeground(object sender, MediaPlayerDataReceivedEventArgs e)
        {
            foreach (var valuePair in e.Data)
            {
                switch (valuePair.Key)
                {
                    case "SetSource":
                        Debug.WriteLine("Setting source to '{0}'", valuePair.Value);

                        try
                        {
                            BackgroundMediaPlayer.Current.SetUriSource(new Uri(valuePair.Value.ToString()));
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine("Unable to set player source: {0}", ex.Message);
                            // TODO: Must send exception to app and the app must report to Insights
                        }

                        break;
                    case "StartPlayback": // Foreground App process has signalled that it is ready for playback
                        Debug.WriteLine("Starting Playback");

                        BackgroundMediaPlayer.Current.Play();

                        break;
                }
            }
        }

        private void OnTaskcompleted(BackgroundTaskRegistration sender, BackgroundTaskCompletedEventArgs args)
        {
            Debug.WriteLine("Background Audio Task " + sender.TaskId + " completed");

            this.deferral.Complete();
        }

        /// <summary>
        /// Executes the task canceled action.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="reason">The reason.</param>
        private void OnTaskCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            Debug.WriteLine("Background Audio Task " + sender.Task.TaskId + " cancelling");
            Debug.WriteLine("Background Audio Task cancel reason: {0}", reason);

            try
            {
                // Unsubscribe event handlers
                this.systemMediaTransportControl.ButtonPressed -= this.OnSystemMediaTransportControlButtonPressed;
                this.systemMediaTransportControl.PropertyChanged -= this.OnSystemMediaTransportControlPropertyChanged;

                BackgroundMediaPlayer.Shutdown();
            }
            catch (Exception ex)
            {
                // TODO: Must send exception to app and the app must report to Insights
            }

            this.deferral.Complete();

            Debug.WriteLine("Background Audio Task canceled");
        }
    }
}
