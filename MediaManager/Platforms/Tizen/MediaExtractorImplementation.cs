using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Plugin.MediaManager.Abstractions;
using Plugin.MediaManager.Abstractions.Enums;
using Tizen.Multimedia;

namespace Plugin.MediaManager
{
    public class MediaExtractorImplementation : IMediaExtractor
    {
        internal Player _player;

        public async Task<IMediaFile> ExtractMediaInfo(IMediaFile mediaFile)
        {
            try
            {
                if (mediaFile.Availability != ResourceAvailability.Remote)
                {
                    MetadataExtractor extractor = new MetadataExtractor(mediaFile.Url);
                    SetMetadata(mediaFile, extractor);
                }
                else
                {
                    StreamInfo streamInfo;
                    CancellationTokenSource tokenSource = new CancellationTokenSource();
                    int timeout = 30000;

                    var task = Task.Run(async () =>
                    {
                        try
                        {
                            while (_player?.State == PlayerState.Preparing)
                            {
                                if (tokenSource.Token.IsCancellationRequested)
                                    tokenSource.Token.ThrowIfCancellationRequested();
                                Log.Info("waitting for ready to extract metadata...");
                                await Task.Delay(1000);
                            }
                        }
                        catch (OperationCanceledException)
                        {
                            Log.Info("Task Aborted.");
                        }
                    }, tokenSource.Token);

                    if (await Task.WhenAny(task, Task.Delay(timeout)) == task)
                    {
                        streamInfo = _player.StreamInfo;
                        SetMetadata(mediaFile, streamInfo);
                        mediaFile.MetadataExtracted = true;
                    }
                    else
                    {
                        Log.Info("Failt to extract media metadata.");
                        tokenSource.Cancel();
                        task.Wait();
                    }
                }
                return await Task.FromResult(mediaFile);
            }
            catch (Exception)
            {
                return mediaFile;
            }
        }

        private void SetMetadata(IMediaFile mediaFile, MetadataExtractor extractor)
        {
            Metadata metadata = extractor.GetMetadata();
            mediaFile.Metadata.Title = metadata.Title;
            mediaFile.Metadata.Artist = metadata.Artist;
            mediaFile.Metadata.Album = metadata.Album;
            mediaFile.Metadata.AlbumArtist = metadata.AlbumArtist;
            mediaFile.Metadata.Author = metadata.Author;
            mediaFile.Metadata.Duration = metadata.Duration ?? 0;
            mediaFile.Metadata.Genre = metadata.Genre;
            if (int.TryParse(metadata.TrackNumber, out var year))
            {
                mediaFile.Metadata.TrackNumber = year;
                mediaFile.Metadata.NumTracks = year;
            }

            var buffer = mediaFile.Type == MediaFileType.Video ? extractor.GetVideoThumbnail() : extractor.GetArtwork().Data;
            if (buffer.Length > 0)
            {
                Stream st = new MemoryStream(buffer);
                mediaFile.Metadata.AlbumArt = st;
            }
        }

        private void SetMetadata(IMediaFile mediaFile, StreamInfo streamInfo)
        {
            mediaFile.Metadata.Title = streamInfo.GetMetadata(StreamMetadataKey.Title);
            mediaFile.Metadata.Artist = streamInfo.GetMetadata(StreamMetadataKey.Artist);
            mediaFile.Metadata.AlbumArtist = streamInfo.GetMetadata(StreamMetadataKey.Album);
            mediaFile.Metadata.Author = streamInfo.GetMetadata(StreamMetadataKey.Author);
            mediaFile.Metadata.Duration = streamInfo.GetDuration();
            mediaFile.Metadata.Genre = streamInfo.GetMetadata(StreamMetadataKey.Genre);
            if (long.TryParse(streamInfo.GetMetadata(StreamMetadataKey.Year), out var year))
            {
                mediaFile.Metadata.Year = year;
            }
            mediaFile.Metadata.AlbumArt = streamInfo.GetAlbumArt();
        }
    }
}