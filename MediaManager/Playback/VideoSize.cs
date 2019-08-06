namespace MediaManager.Playback
{
    public class VideoSize
    {
        public VideoSize(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public int Width { get; }
        public int Height { get; }
        public float AspectRatio => (float)Width/(float)Height;
    }
}
