using System;
using System.Collections.Generic;
using System.Text;
using MediaManager.Media;
using Windows.Media.Core;
using Windows.Storage;

namespace MediaManager.Platforms.Uap.Media
{
    public static class MediaItemExtensions
    {
        public static MediaSource ToMediaSource(this IMediaItem mediaItem)
        {
            /*
            switch (mediaItem.MediaLocation)
            {
                case MediaLocation.Default:
                case MediaLocation.Remote:
                    return MediaSource.CreateFromUri(new Uri(mediaItem.MediaUri));
                case MediaLocation.FileSystem:
                    var du = Player.SystemMediaTransportControls.DisplayUpdater;
                    var storageFile = await StorageFile.GetFileFromPathAsync(mediaItem.MediaUri);

                    var playbackType = (mediaItem.MediaType == MediaType.Audio ? Windows.Media.MediaPlaybackType.Music : Windows.Media.MediaPlaybackType.Video);
                    await du.CopyFromFileAsync(playbackType, storageFile);
                    du.Update();

                    return MediaSource.CreateFromStorageFile(storageFile);
            }*/

            return MediaSource.CreateFromUri(new Uri(mediaItem.MediaUri));
        }
    }
}
