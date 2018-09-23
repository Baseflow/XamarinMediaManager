using System.Timers;
using MediaManager.Media;
using MediaManager.Playback;

namespace MediaManager
{
    public interface INotifyMediaManager : IMediaManager
    {
        Timer Timer { get; }

        void OnStateChanged(object sender, StateChangedEventArgs e);
        void OnPlayingChanged(object sender, PlayingChangedEventArgs e);
        void OnBufferingChanged(object sender, BufferingChangedEventArgs e);
        void OnMediaItemFinished(object sender, MediaItemEventArgs e);
        void OnMediaItemChanged(object sender, MediaItemEventArgs e);
        void OnMediaItemFailed(object sender, MediaItemFailedEventArgs e);
    }
}
