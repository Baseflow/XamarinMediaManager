namespace MediaManager.Audio
{
    public interface IAudioPlayer<TPlayer> : IMediaPlayer<TPlayer> where TPlayer : class
    {
    }
}
