using System;
using Plugin.MediaManager.Abstractions;
using Plugin.MediaManager.Abstractions.Enums;
using Tizen.Multimedia;

namespace Plugin.MediaManager
{
    public class VideoPlayerImplementation : MediaPlayerBase, IVideoPlayer
    {
        private VideoSurface _renderSurface;
        private VideoAspectMode _aspectMode;

        public VideoPlayerImplementation(IVolumeManager volumeManager, IMediaExtractor mediaExtractor) : base(volumeManager, mediaExtractor)
        {
        }

        public IVideoSurface RenderSurface
        {
            get { return _renderSurface; }
            set
            {
                if (!(value is VideoSurface))
                    throw new ArgumentException("Not a valid video surface");

                _renderSurface = (VideoSurface)value;
                _renderSurface.Player = Player;
            }
        }

        public VideoAspectMode AspectMode
        {
            get { return _aspectMode;  }
            set
            {
                PlayerDisplayMode mode;
                switch (value)
                {
                    case VideoAspectMode.None:
                        mode = PlayerDisplayMode.FullScreen;
                        break;
                    case VideoAspectMode.AspectFit:
                        mode = PlayerDisplayMode.LetterBox;
                        break;
                    case VideoAspectMode.AspectFill:
                        mode = PlayerDisplayMode.CroppedFull;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
                Player.DisplaySettings.Mode = mode;
            }
        }

        public bool IsReadyRendering => RenderSurface != null && !RenderSurface.IsDisposed;

        public bool IsMuted
        {
            get { return Player.Muted; }
            set { Player.Muted = value; }
        }

        public void SetVolume(float leftVolume, float rightVolume)
        {
            float volume = Math.Max(leftVolume, rightVolume);
            Player.Volume = volume;
        }

        protected override void PlayerInitialize()
        {
            base.PlayerInitialize();
            if (_renderSurface.MediaView is MediaView mediaView)
            {
                    Player.Display = new Display(mediaView);
            }
            else
            {
                throw new InvalidCastException("Only the MediaView object can be used.");
            }
        }
    }
}