namespace MediaManager.Video
{
    public interface IVideoPlayer<TPlayer, TPlayerView> : IMediaPlayer<TPlayer> where TPlayer : class where TPlayerView : class
    {
        TPlayerView PlayerView { get; set; }
    }
}
