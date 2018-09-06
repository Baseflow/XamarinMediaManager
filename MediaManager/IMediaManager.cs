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

        event MediaFinishedEventHandler MediaFinished;

        event MediaFailedEventHandler MediaFailed;

        event MediaItemChangedEventHandler MediaItemChanged;

        event MediaItemFailedEventHandler MediaItemFailed;

        void OnStatusChanged(object sender, StatusChangedEventArgs e);
        void OnPlayingChanged(object sender, PlayingChangedEventArgs e);
        void OnBufferingChanged(object sender, BufferingChangedEventArgs e);
        void OnMediaFinished(object sender, MediaFinishedEventArgs e);
        void OnMediaFailed(object sender, MediaFailedEventArgs e);
        void OnItemChanged(object sender, MediaFileChangedEventArgs e);
        void OnMediaItemFailed(object sender, MediaItemFailedEventArgs e);
        #endregion
    }
}
