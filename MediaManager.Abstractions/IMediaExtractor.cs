using System.Threading.Tasks;

namespace Plugin.MediaManager.Abstractions
{
    public interface IMediaExtractor
    {
        /// <summary>
        /// Returns the same file with the extracted media information
        /// </summary>
        /// <param name="item">The media file</param>
        Task<IMediaItem> ExtractMediaInfo(IMediaItem item);
    }
}
