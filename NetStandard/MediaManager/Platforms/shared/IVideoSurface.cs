namespace Plugin.MediaManager.Abstractions
{
    /// <summary>
    /// Used to pass through the native view
    /// </summary>
    public interface IVideoSurface
    {
        bool IsDisposed { get; }
    }
}
