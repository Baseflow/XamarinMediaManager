using Com.Google.Android.Exoplayer2;

namespace MediaManager.Platforms.Android;

public class LoadControlSettings
{
    public int MinBufferMs { get; set; } = DefaultLoadControl.DefaultMinBufferMs;

    public int MaxBufferMs { get; set; } = DefaultLoadControl.DefaultMaxBufferMs;

    public int DefaultBufferForPlaybackMs { get; set; } = DefaultLoadControl.DefaultBufferForPlaybackMs;

    public int DefaultBufferForPlaybackAfterRebufferMs { get; set; } =
        DefaultLoadControl.DefaultBufferForPlaybackAfterRebufferMs;

    public int TargetBufferBytes { get; set; } = DefaultLoadControl.DefaultTargetBufferBytes;

    public bool PrioritizeTimeOverSizeTresholds { get; set; } = DefaultLoadControl.DefaultPrioritizeTimeOverSizeThresholds;

}
