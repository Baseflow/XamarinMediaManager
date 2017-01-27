using System;
using System.Threading.Tasks;
using Plugin.MediaManager.Abstractions;
using UIKit;

namespace Plugin.MediaManager
{
    public class MediaRemoteControl : IMediaRemoteControl
    {
        private readonly IPlaybackController _playbackController;

        private readonly TimeSpan _seekInterval = TimeSpan.FromMilliseconds(1000);

        private bool _seeking;

        public MediaRemoteControl(IPlaybackController playbackController)
        {
            _playbackController = playbackController;
        }

        public void RemoteControlReceived(UIEvent uiEvent)
        {
            HandleRemoteControlEvent(uiEvent.Subtype);
        }

        private void HandleRemoteControlEvent(UIEventSubtype eventType)
        {
            switch (eventType)
            {
                case UIEventSubtype.RemoteControlPlay:
                    _playbackController.Play();
                    break;

                case UIEventSubtype.RemoteControlPause:
                    _playbackController.Pause();
                    break;

                case UIEventSubtype.RemoteControlTogglePlayPause:
                    _playbackController.PlayPause();
                    break;

                case UIEventSubtype.RemoteControlStop:
                    _playbackController.Stop();
                    break;

                case UIEventSubtype.RemoteControlNextTrack:
                    _playbackController.PlayNext();
                    break;

                case UIEventSubtype.RemoteControlPreviousTrack:
                    _playbackController.PlayPreviousOrSeekToStart();
                    break;

                case UIEventSubtype.RemoteControlBeginSeekingForward:
                    StartSeekingForward();
                    break;

                case UIEventSubtype.RemoteControlBeginSeekingBackward:
                    StartSeekingBackward();
                    break;

                case UIEventSubtype.RemoteControlEndSeekingForward:
                case UIEventSubtype.RemoteControlEndSeekingBackward:
                    _seeking = false;
                    break;
            }
        }

        private void StartSeekingForward()
        {
            if(TryStartSeek())
            {
                Task.Run(() => ExecuteAndWaitLoop(_playbackController.StepForward));
            }
        }

        private void StartSeekingBackward()
        {
            if (TryStartSeek())
            {
                Task.Run(() => ExecuteAndWaitLoop(_playbackController.StepBackward));
            }
        }

        private bool TryStartSeek()
        {
            if (!_seeking) {
                _seeking = true;
                return true;
            }

            return false;
        }

        private async Task ExecuteAndWaitLoop(Func<Task> taskToExecute)
        {
            while (_seeking)
            {
                await taskToExecute();
                await Task.Delay(_seekInterval);
            }
        }
    }
}