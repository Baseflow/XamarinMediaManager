using MediaManager.Player;

namespace MediaManager.Playback
{
    public class StateChangedEventArgs : EventArgs
    {
        public StateChangedEventArgs(MediaPlayerState state)
        {
            State = state;
        }

        public MediaPlayerState State { get; }
    }
}
