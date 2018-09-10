using System;
using System.Collections.Generic;
using MediaManager.Abstractions.EventArguments;
using MediaManager.Audio;
using MediaManager.Media;
using MediaManager.Playback;
using MediaManager.Video;
using MediaManager.Volume;

namespace MediaManager
{
    public interface IMediaManager : IPlaybackManager
    {
        /// <summary>
        /// Gets or sets the request headers.
        /// </summary>
        Dictionary<string, string> RequestHeaders { get; set; }

        IAudioPlayer AudioPlayer { get; set; }

        IVideoPlayer VideoPlayer { get; set; }

        //INotificationManager NotificationManager { get; set; }

        IMediaExtractor MediaExtractor { get; set; }

        IVolumeManager VolumeManager { get; set; }

        IMediaQueue MediaQueue { get; set; }

        //IPlaybackManager PlaybackManager { get; set; }

        #region Events
        event StatusChangedEventHandler StatusChanged;

        event PlayingChangedEventHandler PlayingChanged;

        event BufferingChangedEventHandler BufferingChanged;

        event MediaItemFinishedEventHandler MediaItemFinished;

        event MediaItemChangedEventHandler MediaItemChanged;

        event MediaItemFailedEventHandler MediaItemFailed;

        void OnStatusChanged(object sender, StatusChangedEventArgs e);
        void OnPlayingChanged(object sender, PlayingChangedEventArgs e);
        void OnBufferingChanged(object sender, BufferingChangedEventArgs e);
        void OnMediaItemFinished(object sender, MediaItemEventArgs e);
        void OnMediaItemChanged(object sender, MediaItemEventArgs e);
        void OnMediaItemFailed(object sender, MediaItemFailedEventArgs e);
        #endregion
    }
}
