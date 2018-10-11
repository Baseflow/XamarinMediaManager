namespace MediaManager.Video
{
    public interface IVideoPlayer<TPlayer> : IMediaPlayer<TPlayer> where TPlayer : class
    {
    }
}
